using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using EFCore.BulkExtensions;

using Spellbox.Contexts;
using Spellbox.Model;


namespace Spellbox.Services
{
    public sealed class CardMarketPriceGuideService
    {
        private readonly HttpClient _http;
        private readonly IDbContextFactory<CardMarketDbContext> _factory;

        private const string PriceGuideUrl = "https://downloads.s3.cardmarket.com/productCatalog/priceGuide/price_guide_1.json";

        public CardMarketPriceGuideService(HttpClient http, IDbContextFactory<CardMarketDbContext> factory)
        {
            _http = http;
            _factory = factory;
        }


        public async Task RefreshAsync(CancellationToken ct = default)
        {
            using var stream = await _http.GetStreamAsync(PriceGuideUrl, ct);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var resp = await JsonSerializer.DeserializeAsync<CardMarketPriceGuideResponse>(stream, options, ct);

            if (resp == null || resp.PriceGuides.Count == 0)
                throw new InvalidOperationException("Failed to parse CardMarket price guide JSON.");

            using var db = await _factory.CreateDbContextAsync(ct);
            db.ChangeTracker.AutoDetectChangesEnabled = false;

            const int batchSize = 2000;
            var buffer = new List<CardMarketPriceCache>(batchSize);

            var bulkConfig = new BulkConfig
            {
                PreserveInsertOrder = true,
                SetOutputIdentity = false,
                BatchSize = batchSize,
                UseTempDB = false,
                BulkCopyTimeout = 0
            };

            foreach (var entry in resp.PriceGuides)
            {
                ct.ThrowIfCancellationRequested();

                buffer.Add(new CardMarketPriceCache
                {
                    ProductId = entry.ProductId,
                    Low = entry.Low,
                    Avg = entry.Avg,
                    Trend = entry.Trend,
                    Avg1 = entry.Avg1,
                    Avg7 = entry.Avg7,
                    Avg30 = entry.Avg30,
                    FoilLow = entry.LowFoil,
                    FoilAvg = entry.AvgFoil,
                    FoilTrend = entry.TrendFoil,
                    FoilAvg1 = entry.Avg1Foil,
                    FoilAvg7 = entry.Avg7Foil,
                    FoilAvg30 = entry.Avg30Foil,
                    UpdatedAt = DateTime.UtcNow
                });

                if (buffer.Count == batchSize)
                {
                    await db.BulkInsertOrUpdateAsync(buffer, bulkConfig, cancellationToken:ct);

                    buffer.Clear();
                    await Task.Yield();
                }
            }

            if (buffer.Count > 0)
            {
                await db.BulkInsertOrUpdateAsync(buffer, bulkConfig, cancellationToken:ct);
            }
        }
    }


    public sealed class CardMarketPriceGuideResponse
    {
        [JsonPropertyName("version")]
        public int Version { get; set; }
        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; } = null!;

        [JsonPropertyName("priceGuides")]
        public List<CardMarketPriceGuideEntry> PriceGuides { get; set; } = new List<CardMarketPriceGuideEntry>();
    }

    public sealed class CardMarketPriceGuideEntry
    {
        [JsonPropertyName("idProduct")]
        public int ProductId { get; set; }
        [JsonPropertyName("idCategory")]
        public int CategoryId { get; set; }

        [JsonPropertyName("avg")]
        public decimal? Avg { get; set; }
        [JsonPropertyName("low")]
        public decimal? Low { get; set; }
        [JsonPropertyName("trend")]
        public decimal? Trend { get; set; }
        [JsonPropertyName("avg1")]
        public decimal? Avg1 { get; set; }
        [JsonPropertyName("avg7")]
        public decimal? Avg7 { get; set; }
        [JsonPropertyName("avg30")]
        public decimal? Avg30 { get; set; }
        [JsonPropertyName("avg-foil")]
        public decimal? AvgFoil { get; set; }
        [JsonPropertyName("low-foil")]
        public decimal? LowFoil { get; set; }
        [JsonPropertyName("trend-foil")]
        public decimal? TrendFoil { get; set; }
        [JsonPropertyName("avg1-foil")]
        public decimal? Avg1Foil { get; set; }
        [JsonPropertyName("avg7-foil")]
        public decimal? Avg7Foil { get; set; }
        [JsonPropertyName("avg30-foil")]
        public decimal? Avg30Foil { get; set; }
    }
}