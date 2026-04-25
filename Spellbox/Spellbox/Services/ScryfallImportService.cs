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
            "case", "saga", "adventure", "mutate", "prototype", "battle", "reversible_card",
            "prepare"
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
            var reversibleBatch = new List<JsonElement>();

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
                            reversibleBatch.Add(card.Clone());
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

            foreach (var card in reversibleBatch)
                variantBatch.AddRange(await ParseReversibleCard(card));

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
            request.Headers.Add("Accept", "application/json;q=0.9,*/*;q=0.8");
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
                    ? (int) (10 * cmc)
                    : 0,

                Colors = card.TryGetProperty("colors", out var colors) &&
                                colors.ValueKind == JsonValueKind.Array
                    ? CardColours.FromEnumerable(colors.EnumerateArray()
                        .Select(c => c.GetString())
                        .Where(c => !String.IsNullOrWhiteSpace(c))!).ToInt()
                    : 0,

                ColorIdentity = card.TryGetProperty("color_identity", out var colorIdentity) &&
                                colorIdentity.ValueKind == JsonValueKind.Array
                    ? CardColours.FromEnumerable(colorIdentity.EnumerateArray()
                        .Select(c => c.GetString())
                        .Where(c => !String.IsNullOrWhiteSpace(c))!).ToInt()
                    : 0
            };

            if (card.TryGetProperty("legalities", out var legalities) &&
                legalities.ValueKind == JsonValueKind.Object)
            {
                oracle.Legalities = new CardLegality
                {
                    Standard = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("standard")),
                    Modern = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("modern")),
                    Pioneer = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("pioneer")),
                    Legacy = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("legacy")),
                    Vintage = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("vintage")),
                    Pauper = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("pauper")),
                    Penny = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("penny")),
                    Commander = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("commander")),
                    Oathbreaker = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("oathbreaker")),
                    PauperCommander = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("paupercommander")),
                    DuelCommander = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("duel")),
                    OldSchool = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("oldschool")),
                    PreModern = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("premodern")),
                    PreDH = ParseCardLegalityType(legalities.GetPropertyOrEmptyString("predh")),
                }.ToInt();
            }

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

        private static CardLegalityType ParseCardLegalityType(
            string legalityType
        )
        {
            return legalityType switch
            {
                "not_legal" => CardLegalityType.NotLegal,
                "legal" => CardLegalityType.Legal,
                "restricted" => CardLegalityType.Restricted,
                "banned" => CardLegalityType.Banned,
                _ => CardLegalityType.NotLegal,
            };
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
                variant.FlavorTexts = faces
                    .EnumerateArray()
                    .Select(f => f.GetPropertyOrEmptyString("flavor_text"))
                    .ToList();
            }
            else
            {
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

                    var scryfallId = i == 0
                        ? Guid.Parse(scryfallIdStr)
                        : Guid.Parse(scryfallIdStr[..^2] + scryfallIdStr[^1] + scryfallIdStr[^2]);

                    var parent = await db.Oracles
                        .Where(o => o.OracleId == oracleId)
                        .SingleOrDefaultAsync();

                    if (parent is null)
                    {
                        _logger.LogInformation("Skipping reversible card {scryfallIdStr} {i}", scryfallId, i);
                        continue;
                    }

                    variants.Add(new CardVariant
                    {
                        IsReversed = i == 1,
                        ScryfallId = scryfallId,
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
                        FlavorTexts = new List<string> { face.GetPropertyOrEmptyString("flavor_text") }
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
            {
                try
                {
                    await SqliteUpserts.UpsertVariantAsync(db, variant, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Variant upsert error: {variant.ScryfallId} {variant.SearchName} {variant.SetCode} {variant.CollNum}");
                    continue;
                }
            }

            await tx.CommitAsync(ct);

            oracleBatch.Clear();
            faceBatch.Clear();
            variantBatch.Clear();
        }
    }
}