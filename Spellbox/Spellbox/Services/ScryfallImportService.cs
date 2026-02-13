using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

using Spellbox.Contexts;
using Spellbox.Model;
using Spellbox.Utilities;


namespace Spellbox.Services
{
    public sealed class ScryfallImportProgress
    {
        public string Stage { get; set; } = string.Empty;
        public int BatchesProcessed { get; set; }
    }


    public interface IScryfallImportService
    {
        Task ImportAsync(
            IProgress<ScryfallImportProgress>? progress = null,
            CancellationToken ct = default
        );
    }


    public sealed class ScryfallImportService : IScryfallImportService
    {
        private const int BatchSize = 1000;
        private const string BulkUri = "https://api.scryfall.com/bulk-data";
        private static readonly IEnumerable<string> DesiredLayouts = new HashSet<string> 
        {
            "normal", "split", "flip", "transform", "modal_dfc", "meld", "leveler", "class",
            "case", "saga", "adventure", "mutate", "prototype", "battle", "reversible_card"
        };

        private readonly HttpClient _http;
        private readonly IDbContextFactory<OracleDbContext> _factory;
        private readonly ILogger<ScryfallImportService> _logger;

        public ScryfallImportService(
            HttpClient http, 
            IDbContextFactory<OracleDbContext> factory,
            ILogger<ScryfallImportService> logger
        )
        {
            _http = http;
            _factory = factory;
            _logger = logger;
        }


        // Public methods

        public async Task ImportAsync(
            IProgress<ScryfallImportProgress>? progress = null, 
            CancellationToken ct = default
        )
        {
            progress?.Report(new ScryfallImportProgress
            {
                Stage = "Preparing..."
            });

            using var db = await _factory.CreateDbContextAsync(ct);
            await db.Database.MigrateAsync(ct);

            var sync = await db.SyncStates
                .SingleOrDefaultAsync(x => x.Key == "Scryfall", ct);
                
            if (sync is null)
            {
                sync = new ScryfallSyncState();

                db.SyncStates.Add(sync);
                await db.SaveChangesAsync(ct);
            }

            var downloadUrl = await GetDownloadUrlAsync(ct);

            progress?.Report(new ScryfallImportProgress
            {
                Stage = "Streaming data..."
            });

            var oracleProcessed = new HashSet<Guid>();
            var oracleBatch = new Dictionary<Guid, CardOracle>();
            var faceBatch = new List<CardFace>();
            var variantBatch = new List<CardVariant>();

            bool delta = sync.SyncedAt > DateTime.MinValue;
            int batches = 0;

            await foreach (var card in StreamScryfallCardsAsync(downloadUrl, ct))
            {
                string? oracleIdStr = null;

                try
                {
                    if (card.ValueKind == JsonValueKind.Object &&
                        card.TryGetProperty("oracle_id", out var pId))
                    {
                        oracleIdStr = pId.GetString();
                    }
                    else if (card.ValueKind == JsonValueKind.Object &&
                             card.TryGetProperty("layout", out var pLayout))
                    {
                        if (pLayout.GetString() == "reversible_card")
                        {
                            variantBatch.AddRange(await ParseReversibleCard(card));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Corrupt JSON. Skipping card...");
                    continue;
                }

                if (!Guid.TryParse(oracleIdStr, out var oracleId))
                    continue;

                if (!IsCardDesired(card))
                    continue;

                if (delta && !IsNewerThanSync(card, sync.SyncedAt))
                    continue;

                var scryfallId = Guid.Parse(card.GetProperty("id").GetString()!);

                if (!oracleProcessed.Contains(oracleId))
                {
                    oracleBatch[oracleId] = ParseCardOracle(card, oracleId, faceBatch);

                    oracleProcessed.Add(oracleId);
                }

                variantBatch.Add(ParseCardVariant(card, oracleId, scryfallId));

                if (oracleBatch.Count + variantBatch.Count + faceBatch.Count >= BatchSize)
                {
                    await FlushAsync(oracleBatch, faceBatch, variantBatch, ct);

                    batches++;

                    progress?.Report(new ScryfallImportProgress
                    {
                        Stage = "Processing...",
                        BatchesProcessed = batches
                    });
                }
            }

            await FlushAsync(oracleBatch, faceBatch, variantBatch, ct);

            sync.SyncedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);

            progress?.Report(new ScryfallImportProgress
            {
                Stage = "Done.",
                BatchesProcessed = batches + 1
            });

        }


        // Private methods

        private async Task<string> GetDownloadUrlAsync(CancellationToken ct)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, BulkUri);
            request.Headers.Add("Aceept", "application/json;q=0.9,*/*;q=0.8");
            request.Headers.Add("User-Agent", "SpellboxAPI");

            using var resp = await _http.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );

            resp.EnsureSuccessStatusCode();

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var jdoc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            return jdoc.RootElement
                .GetProperty("data")
                .EnumerateArray()
                .First(x => x.GetProperty("type").GetString() == "default_cards")
                .GetProperty("download_uri")
                .GetString()!;
        }

        private async IAsyncEnumerable<JsonElement> StreamScryfallCardsAsync(
            string url,
            [EnumeratorCancellation] CancellationToken ct
        )
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json;q=0.9,*/*;q=0.8");
            request.Headers.Add("User-Agent", "SpellboxAPI");

            using var resp = await _http.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );

            resp.EnsureSuccessStatusCode();

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var jdoc = await JsonDocument.ParseAsync(stream, new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            }, ct);

            foreach (var element in jdoc.RootElement.EnumerateArray())
            {
                ct.ThrowIfCancellationRequested();
                yield return element;
            }
        }


        private static bool IsCardDesired(JsonElement card)
        {
            if (!card.TryGetProperty("games", out var games) ||
                !games.EnumerateArray().Any(g => g.GetString() == "paper"))
                return false;

            if (!card.TryGetProperty("layout", out var layout) ||
                !DesiredLayouts.Contains(layout.GetString()!))
                return false;

            if (card.TryGetProperty("promo_types", out var promos) &&
                promos.EnumerateArray().Any(p => p.GetString() == "thick"))
                return false;

            return true;
        }

        private static bool IsNewerThanSync(JsonElement card, DateTime lastSync)
        {
            if (!card.TryGetProperty("updated_at", out var updated))
                return true;

            var updatedAt = DateTime.Parse(updated.GetString()!);
            return updatedAt > lastSync;
        }

        private static CardOracle ParseCardOracle(
            JsonElement card,
            Guid oracleId,
            List<CardFace> faceBatch
        )
        {
            var oracle = new CardOracle
            {
                OracleId = oracleId,

                Name = card.TryGetProperty("name", out var name)
                    ? name.GetString() ?? "UNKNOWN"
                    : "UNKNOWN",

                TypeLine = card.TryGetProperty("type_line", out var typeLine)
                    ? typeLine.GetString() ?? ""
                    : "",

                Keywords = card.TryGetProperty("keywords", out var keywords) &&
                           keywords.ValueKind == JsonValueKind.Array
                    ? keywords.EnumerateArray()
                        .Select(k => k.GetString())
                        .Where(k => !string.IsNullOrWhiteSpace(k))
                        .ToList()!
                    : new List<string>(),

                CMC = card.TryGetProperty("cmc", out var cmcElem) &&
                      cmcElem.ValueKind == JsonValueKind.Number &&
                      cmcElem.TryGetDecimal(out var cmc)
                    ? cmc
                    : 0m,

                ColorIdentity = card.TryGetProperty("color_identity", out var colorIdentity) &&
                                colorIdentity.ValueKind == JsonValueKind.Array
                    ? colorIdentity.EnumerateArray()
                        .Select(c => c.GetString())
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .ToList()!
                    : new List<string>()
            };

            if (card.TryGetProperty("card_faces", out var faces) &&
                faces.ValueKind == JsonValueKind.Array)
            {
                int i = 0;
                foreach (var face in faces.EnumerateArray())
                {
                    faceBatch.Add(ParseCardFace(face, oracleId, i++));
                }
            }
            else
            {
                faceBatch.Add(ParseCardFace(card, oracleId, 0));
            }

            return oracle;
        }

        private static CardFace ParseCardFace(
            JsonElement face,
            Guid oracleId,
            int order
        )
        {
            return new CardFace
            {
                Id = Guid.NewGuid(),
                OracleId = oracleId,
                Order = order,

                Name = face.TryGetProperty("name", out var name)
                    ? name.GetString() ?? "UNKNOWN"
                    : "UNKNOWN",

                ManaCost = face.GetPropertyOrEmptyString("mana_cost"),
                TypeLine = face.GetPropertyOrEmptyString("type_line"),
                OracleText = face.GetPropertyOrEmptyString("oracle_text"),
                Power = face.GetPropertyOrEmptyString("power"),
                Toughness = face.GetPropertyOrEmptyString("toughness"),
                Defense = face.GetPropertyOrEmptyString("defense"),
                Loyalty = face.GetPropertyOrEmptyString("loyalty")
            };
        }

        private static CardVariant ParseCardVariant(
            JsonElement card,
            Guid oracleId,
            Guid scryfallId
        )
        {
            var variant = new CardVariant
            {
                ScryfallId = scryfallId,
                OracleId = oracleId,
                SearchName = card.GetPropertyOrAlternative(
                    "flavor_name",
                    card.GetPropertyOrEmptyString("name")
                ),
                SetName = card.GetPropertyOrEmptyString("set_name"),
                SetCode = card.GetPropertyOrEmptyString("set"),
                CollNum = card.GetPropertyOrEmptyString("collector_number"),
                Finishes = card.TryGetProperty("finishes", out var finishes) &&
                           finishes.ValueKind == JsonValueKind.Array
                    ? finishes.EnumerateArray()
                        .Select(f => f.GetString())
                        .Where(f => !string.IsNullOrWhiteSpace(f))
                        .ToList()!
                    : new List<string>(),
                Artist = card.GetPropertyOrEmptyString("artist"),
                Released = card.GetPropertyOrEmptyString("released_at"),
                Rarity = card.GetPropertyOrEmptyString("rarity"),
                CardMarketProductId = card.GetPropertyOrNullInt("cardmarket_id")
            };

            if (card.TryGetProperty("card_faces", out var faces))
            {
                var thumbs = new List<string?>();
                var images = new List<string?>();

                foreach (var face in faces.EnumerateArray())
                {
                    if (face.TryGetProperty("image_uris", out var faceImg))
                    {
                        thumbs.Add(faceImg.GetPropertyOrEmptyString("small"));
                        images.Add(faceImg.GetPropertyOrEmptyString("normal"));
                    }
                    else
                    {
                        thumbs.Add(null);
                        images.Add(null);
                    }
                }

                if (images.All(string.IsNullOrEmpty) && card.TryGetProperty("image_uris", out var cardImg))
                {
                    thumbs = new List<string?> { cardImg.GetPropertyOrEmptyString("small") };
                    images = new List<string?> { cardImg.GetPropertyOrEmptyString("normal") };
                }

                variant.Thumbs = thumbs!;
                variant.Images = images!;

                variant.FlavorTexts = faces
                    .EnumerateArray()
                    .Select(f => f.GetPropertyOrEmptyString("flavor_text"))
                    .ToList();
            }
            else
            {
                if (card.TryGetProperty("image_uris", out var cardImg))
                {
                    variant.Thumbs = new List<string> { cardImg.GetPropertyOrEmptyString("small") };
                    variant.Images = new List<string> { cardImg.GetPropertyOrEmptyString("normal") };
                }
                else
                {
                    variant.Thumbs = new List<string> { "" };
                    variant.Images = new List<string> { "" };
                }

                variant.FlavorTexts = new List<string> { card.GetPropertyOrEmptyString("flavor_text") };
            }

            return variant;
        }

        private async Task<List<CardVariant>> ParseReversibleCard(
            JsonElement card
        )
        {
            using var db = await _factory.CreateDbContextAsync();

            var variants = new List<CardVariant>();

            if (card.TryGetProperty("reprint", out var reprint) &&
                !reprint.GetBoolean())
                return variants;

            if (card.TryGetProperty("card_faces", out var faces) &&
                faces.ValueKind == JsonValueKind.Array)
            {
                var scryfallIdStr = card.GetProperty("id").GetString()!;

                int i = 0;
                foreach (var face in faces.EnumerateArray())
                {
                    var oracleId = Guid.Parse(face.GetProperty("oracle_id").GetString()!);

                    var parent = await db.Oracles
                        .Where(o => o.OracleId == oracleId)
                        .SingleOrDefaultAsync();

                    if (parent is null)
                        continue;

                    var img = face.GetProperty("image_uris");

                    variants.Add(new CardVariant
                    {
                        ScryfallId = Guid.Parse(scryfallIdStr[..^1] + i.ToString()),
                        OracleId = oracleId,

                        SetName = card.GetPropertyOrEmptyString("set_name"),
                        SetCode = card.GetPropertyOrEmptyString("set"),
                        CollNum = card.GetPropertyOrEmptyString("collector_number") + "ab"[i],
                        Finishes = card.TryGetProperty("finishes", out var finishes) &&
                                finishes.ValueKind == JsonValueKind.Array
                            ? finishes.EnumerateArray()
                                .Select(f => f.GetString())
                                .Where(f => !string.IsNullOrWhiteSpace(f))
                                .ToList()!
                            : new List<string>(),
                        Released = card.GetPropertyOrEmptyString("released_at"),
                        Rarity = card.GetPropertyOrEmptyString("rarity"),
                        CardMarketProductId = card.GetPropertyOrNullInt("cardmarket_id"),

                        SearchName = face.GetPropertyOrEmptyString("name"),
                        Artist = face.GetPropertyOrEmptyString("artist"),
                        FlavorTexts = new List<string> { face.GetPropertyOrEmptyString("flavor_text") },
                        Thumbs = new List<string> { img.GetPropertyOrEmptyString("small").ToString() },
                        Images = new List<string> { img.GetPropertyOrEmptyString("normal").ToString() }
                    });

                    i++;
                }
            }

            return variants;
        }


        private async Task FlushAsync(
            Dictionary<Guid, CardOracle> oracleBatch,
            List<CardFace> faceBatch,
            List<CardVariant> variantBatch,
            CancellationToken ct
        )
        {
            if (oracleBatch.Count == 0 && faceBatch.Count == 0 && variantBatch.Count == 0)
                return;

            using var db = await _factory.CreateDbContextAsync(ct);
            await using var tx = await db.Database.BeginTransactionAsync(ct);

            foreach (var oracle in oracleBatch.Values)
                await SqliteUpserts.UpsertOracleAsync(db, oracle, ct);

            foreach (var face in faceBatch)
                await SqliteUpserts.UpsertFaceAsync(db, face, ct);

            foreach (var variant in variantBatch)
                await SqliteUpserts.UpsertVariantAsync(db, variant, ct);

            await tx.CommitAsync(ct);

            oracleBatch.Clear();
            faceBatch.Clear();
            variantBatch.Clear();
        }
    }
}