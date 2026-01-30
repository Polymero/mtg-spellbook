using System.Runtime.CompilerServices;
using System.Text.Json;
// using System.Threading.Channels;
// using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
// using Spellbox.Components.Pages;
using Spellbox.Contexts;
using Spellbox.Model;
using Spellbox.Utilities;


namespace Spellbox.Services
{

    public sealed class StagingScryfallCard
    {
        public Guid ScryfallId { get; set; }
        public string Json { get; set; } = null!;
        public DateTime ImportedAt { get; set; }
    }

    public sealed class ScryfallImportState
    {
        public string Key { get; set; } = "Scryfall";
        public int Phase { get; set; }
        public DateTime? CompletedAt { get; set; }
    }


    public interface IScryfallCardService
    {
        Task ImportAsync(CancellationToken ct);
    }

    public sealed class ScryfallCardService : IScryfallCardService
    {
        private readonly IDbContextFactory<OracleDbContext> _factory;
        private readonly HttpClient _http;
        private readonly ILogger<ScryfallCardService> _logger;

        private const int CommitInterval = 500;
        private const string BulkUri = "https://api.scryfall.com/bulk-data";
        private static readonly IEnumerable<string> DesiredLayouts = new HashSet<string> 
        {
            "normal", "split", "flip", "transform", "modal_dfc", "meld", "leveler", "class",
            "case", "saga", "adventure", "mutate", "prototype", "battle", "reversible_card"
        };

        public ScryfallCardService(
            IDbContextFactory<OracleDbContext> factory, 
            HttpClient http,
            ILogger<ScryfallCardService> logger
        )
        {
            _factory = factory;
            _http = http;
            _logger = logger;
        }


        public async Task ImportAsync(CancellationToken ct)
        {
            await StageAsync(ct);
            await ImportOraclesAsync(ct);
            await ImportFacesAsync(ct);
            await ImportVariantsAsync(ct);
            await MarkCompletedAsync(ct);
        }


        private async Task StageAsync(CancellationToken ct)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var state = await GetStateAsync(db, ct);

            if (state.Phase > 0)
                return;

            var downloadUrl = await GetBulkDownloadUrlAsync(ct);

            await foreach (var card in StreamCardsAsync(downloadUrl, ct))
            {
                var id = Guid.Parse(card.GetProperty("id").GetString()!);

                if (!IsCardDesired(card))
                    continue;

                db.StagingCards.Add(new StagingScryfallCard
                {
                    ScryfallId = id,
                    Json = card.GetRawText(),
                    ImportedAt = DateTime.UtcNow
                });

                if (db.ChangeTracker.Entries().Count() >= CommitInterval)
                    await db.SaveChangesAsync(ct);
            }

            await db.SaveChangesAsync(ct);
            state.Phase = 1;
            await db.SaveChangesAsync(ct);
        }

        private async Task ImportOraclesAsync(CancellationToken ct)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var state = await GetStateAsync(db, ct);

            if (state.Phase > 1)
                return;

            var cards = db.StagingCards
                .AsNoTracking()
                .AsEnumerable();

            int processed = 0;
            foreach (var row in cards)
            {
                using var doc = JsonDocument.Parse(row.Json);
                var card = doc.RootElement;

                if (!card.TryGetProperty("oracle_id", out var oracleIdProp))
                    continue;

                var oracleId = Guid.Parse(oracleIdProp.GetString()!);

                var oracle = ParseOracle(card, oracleId);

                await SqliteUpserts.UpsertOracleAsync(db, oracle, ct);

                processed++;
                if (processed % CommitInterval == 0)
                    await db.SaveChangesAsync(ct);
            }

            await db.SaveChangesAsync(ct);
            state.Phase = 2;
            await db.SaveChangesAsync(ct);
        }

        private async Task ImportFacesAsync(CancellationToken ct)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var state = await GetStateAsync(db, ct);

            if (state.Phase > 2)
                return;

            var cards = db.StagingCards
                .AsNoTracking()
                .AsEnumerable();

            int processed = 0;
            foreach (var row in cards)
            {
                using var doc = JsonDocument.Parse(row.Json);
                var card = doc.RootElement;

                if (!card.TryGetProperty("oracle_id", out var oracleIdProp))
                    continue;

                var oracleId = Guid.Parse(oracleIdProp.GetString()!);

                var faces = new List<CardFace>();

                if (card.TryGetProperty("card_faces", out var cardFaces) &&
                    cardFaces.ValueKind == JsonValueKind.Array)
                {
                    int i = 0;
                    foreach (var cardFace in cardFaces.EnumerateArray())
                    {
                        faces.Add(ParseFace(cardFace, oracleId, i++));
                    }
                }
                else
                {
                    faces.Add(ParseFace(card, oracleId, 0));
                }

                foreach (var face in faces)
                {
                    await SqliteUpserts.UpsertFaceAsync(db, face, ct);
                }

                processed++;
                if (processed % CommitInterval == 0)
                    await db.SaveChangesAsync(ct);
            }

            await db.SaveChangesAsync(ct);
            state.Phase = 3;
            await db.SaveChangesAsync(ct);
        }

        private async Task ImportVariantsAsync(CancellationToken ct)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var state = await GetStateAsync(db, ct);

            if (state.Phase > 3)
                return;

            var cards = db.StagingCards
                .AsNoTracking()
                .AsEnumerable();

            int processed = 0;
            foreach (var row in cards)
            {
                using var doc = JsonDocument.Parse(row.Json);
                var card = doc.RootElement;

                var oracleId = Guid.Parse(card.GetProperty("oracle_id").GetString()!);
                var scryfallId = Guid.Parse(card.GetProperty("id").GetString()!);

                var variant = ParseVariant(card, oracleId, scryfallId);

                await SqliteUpserts.UpsertVariantAsync(db, variant, ct);

                processed++;
                if (processed % CommitInterval == 0)
                    await db.SaveChangesAsync(ct);
            }

            await db.SaveChangesAsync(ct);
            state.Phase = 4;
            await db.SaveChangesAsync(ct);
        }

        private async Task MarkCompletedAsync(CancellationToken ct)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var state = await GetStateAsync(db, ct);

            state.CompletedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
        }


        private static async Task<ScryfallImportState> GetStateAsync(OracleDbContext db, CancellationToken ct)
        {
            var state = await db.ImportStates
                .SingleOrDefaultAsync(s => s.Key == "Scryfall", ct);

            if (state == null)
            {
                state = new ScryfallImportState();
                db.Add(state);
                await db.SaveChangesAsync(ct);
            }

            return state;
        }

        private async Task<string> GetBulkDownloadUrlAsync(CancellationToken ct)
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

        private async IAsyncEnumerable<JsonElement> StreamCardsAsync(
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

        private static CardOracle ParseOracle(
            JsonElement card,
            Guid oracleId
        )
        {
            return new CardOracle
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
        }

        private static CardFace ParseFace(
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

        private static CardVariant ParseVariant(
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
    }

}