using System.Text.Json;
using Microsoft.EntityFrameworkCore;

using Spellbox.Contexts;
using Spellbox.Model;


namespace Spellbox.Services
{
    public sealed class CardMarketMappingService
    {
        private readonly CardMarketDbContext _db;
        private readonly OracleDbContext _oracle;
        private readonly HttpClient _http;

        private Dictionary<int, CardMarketCatalogProduct>? _catalogCache;

        private const string CatalogUrl = "https://downloads.s3.cardmarket.com/productCatalog/productList/products_singles_1.json";

        public CardMarketMappingService(CardMarketDbContext db, OracleDbContext oracle, HttpClient http)
        {
            _db = db;
            _oracle = oracle;
            _http = http;
        }


        public async Task<int?> ResolveProductIdAsync(
            Guid variantId,
            CardFinish finish,
            CardLanguage language
        )
        {
            var existing = await _db.ProductMappings
                .FirstOrDefaultAsync(m =>
                    m.CardVariantId == variantId &&
                    m.Finish == finish &&
                    m.Language == language);

            if (existing != null)
                return existing.ProductId;

            await EnsureCatalogLoadedAsync();

            var variant = await _oracle.CardVariants
                .Include(v => v.OracleCard)
                .FirstAsync(v => v.ScryfallId == variantId);

            var normName = NormalizeName(variant.OracleCard.Name);

            var match = _catalogCache!.Values
                .Where(p =>
                    string.Equals(p.Expansion, variant.SetCode, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(p.Number, variant.CollNum, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(p => NormalizeName(p.Name) == normName);

            if (match == null)
                return null;

            var mapping = new CardMarketProductMapping
            {
                Id = Guid.NewGuid(),
                CardVariantId = variantId,
                ProductId = match.IdProduct,
                SetCode = variant.SetCode,
                CollNum = variant.CollNum,
                Name = variant.OracleCard.Name,
                Finish = finish,
                Language = language,
                CreatedAt = DateTime.UtcNow
            };

            _db.ProductMappings.Add(mapping);
            await _db.SaveChangesAsync();

            return match.IdProduct;
        }

        private async Task EnsureCatalogLoadedAsync()
        {
            if (_catalogCache != null)
                return;

            var json = await _http.GetStringAsync(CatalogUrl);

            var list = JsonSerializer.Deserialize<List<CardMarketCatalogProduct>>(json)!;

            _catalogCache = list.ToDictionary(p => p.IdProduct);
        }

        private static string NormalizeName(string name)
            => name.Trim().ToLowerInvariant()
                .Replace("’", "'")
                .Replace("-", "")
                .Replace(",", "");
    }


    public sealed class CardMarketCatalogProduct
    {
        public int IdProduct { get; set; }
        public string Name { get; set; } = null!;
        public string Expansion { get; set; } = null!;
        public string Number { get; set; } = null!;
    }
}