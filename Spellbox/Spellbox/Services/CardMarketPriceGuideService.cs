using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using EFCore.BulkExtensions;

using Spellbox.Contexts;
using Spellbox.Model;
using System.Runtime.CompilerServices;
using Spellbox.Utilities;


namespace Spellbox.Services
{
    public sealed class CardMarketPriceGuideService
    {
        private const int BatchSize = 1000;
        private const string PriceGuideUrl = "https://downloads.s3.cardmarket.com/productCatalog/priceGuide/price_guide_1.json";

        private readonly HttpClient _http;
        private readonly IDbContextFactory<CardMarketDbContext> _factory;
        private readonly ILogger<CardMarketPriceGuideService> _logger;

        public CardMarketPriceGuideService(
            HttpClient http, 
            IDbContextFactory<CardMarketDbContext> factory,
            ILogger<CardMarketPriceGuideService> logger
        )
        {
            _http = http;
            _factory = factory;
            _logger = logger;
        }


        // Public methods

        public async Task RefreshAsync(CancellationToken ct = default)
        {
            var guideBatch = new List<CardMarketPriceCache>();

            await foreach (var guide in StreamPriceGuidesAsync(PriceGuideUrl, ct))
            {
                ct.ThrowIfCancellationRequested();
                
                int? productId = null;

                try
                {
                    if (guide.ValueKind == JsonValueKind.Object &&
                        guide.TryGetProperty("idProduct", out var id))
                    {
                        productId = id.GetInt32();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Corrupt JSON. Skipping guide...");
                    continue;
                }

                if (productId is null)
                {
                    _logger.LogWarning("Null idProduct. Skipping guide...");
                    continue;
                }

                try
                {
                    guideBatch.Add(new CardMarketPriceCache
                    {
                        ProductId = productId.Value,
                        Avg = guide.GetPropertyOrNullDecimal("avg"),
                        Low = guide.GetPropertyOrNullDecimal("low"),
                        Trend = guide.GetPropertyOrNullDecimal("trend"),
                        FoilAvg = guide.GetPropertyOrNullDecimal("avg-foil"),
                        FoilLow = guide.GetPropertyOrNullDecimal("low-foil"),
                        FoilTrend = guide.GetPropertyOrNullDecimal("trend-foil"),
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to project price guide, skipping...");
                    continue;
                }

                if (guideBatch.Count >= BatchSize)
                {
                    await FlushAsync(guideBatch, ct);
                }
            }

            if (guideBatch.Count > 0)
                await FlushAsync(guideBatch, ct);
        }


        // Private methods

        private async IAsyncEnumerable<JsonElement> StreamPriceGuidesAsync(
            string url,
            [EnumeratorCancellation] CancellationToken ct
        )
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await _http.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var jdoc = await JsonDocument.ParseAsync(stream, new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            }, ct);

            foreach (var element in jdoc.RootElement.GetProperty("priceGuides").EnumerateArray())
            {
                ct.ThrowIfCancellationRequested();
                yield return element;
            }
        }

        private async Task FlushAsync(
            List<CardMarketPriceCache> batch,
            CancellationToken ct
        )
        {
            if (batch.Count == 0)
                return;

            using var db = await _factory.CreateDbContextAsync(ct);
            await db.Database.MigrateAsync(ct);

            await using var tx = await db.Database.BeginTransactionAsync(ct);

            foreach (var cache in batch)
                await SqliteUpserts.UpsertCardMarketPriceCacheAsync(db, cache, ct);

            await tx.CommitAsync(ct);

            batch.Clear();
        }

    }


    // public sealed class CardMarketPriceGuideResponse
    // {
    //     [JsonPropertyName("version")]
    //     public int Version { get; set; }
    //     [JsonPropertyName("createdAt")]
    //     public string CreatedAt { get; set; } = null!;

    //     [JsonPropertyName("priceGuides")]
    //     public List<CardMarketPriceGuideEntry> PriceGuides { get; set; } = new List<CardMarketPriceGuideEntry>();
    // }

    // public sealed class CardMarketPriceGuideEntry
    // {
    //     [JsonPropertyName("idProduct")]
    //     public int ProductId { get; set; }
    //     [JsonPropertyName("idCategory")]
    //     public int CategoryId { get; set; }

    //     [JsonPropertyName("avg")]
    //     public decimal? Avg { get; set; }
    //     [JsonPropertyName("low")]
    //     public decimal? Low { get; set; }
    //     [JsonPropertyName("trend")]
    //     public decimal? Trend { get; set; }
    //     [JsonPropertyName("avg1")]
    //     public decimal? Avg1 { get; set; }
    //     [JsonPropertyName("avg7")]
    //     public decimal? Avg7 { get; set; }
    //     [JsonPropertyName("avg30")]
    //     public decimal? Avg30 { get; set; }
    //     [JsonPropertyName("avg-foil")]
    //     public decimal? AvgFoil { get; set; }
    //     [JsonPropertyName("low-foil")]
    //     public decimal? LowFoil { get; set; }
    //     [JsonPropertyName("trend-foil")]
    //     public decimal? TrendFoil { get; set; }
    //     [JsonPropertyName("avg1-foil")]
    //     public decimal? Avg1Foil { get; set; }
    //     [JsonPropertyName("avg7-foil")]
    //     public decimal? Avg7Foil { get; set; }
    //     [JsonPropertyName("avg30-foil")]
    //     public decimal? Avg30Foil { get; set; }
    // }
}