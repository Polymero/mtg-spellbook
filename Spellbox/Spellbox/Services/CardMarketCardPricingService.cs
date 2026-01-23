using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Spellbox.Contexts;
using Spellbox.Model;


namespace Spellbox.Services
{
    public sealed class CardMarketCardPricingService
    {
        private readonly IDbContextFactory<OracleDbContext> _oracle;
        private readonly IDbContextFactory<CardMarketDbContext> _market;

        public CardMarketCardPricingService(IDbContextFactory<OracleDbContext> oracle, IDbContextFactory<CardMarketDbContext> market)
        {
            _oracle = oracle;
            _market = market;
        }


        public async Task<decimal?> GetPriceAsync(
            Guid variantId,
            CardFinish finish,
            CardLanguage language
        )
        {
            using var oracleDb = await _oracle.CreateDbContextAsync();

            var productId = await oracleDb.CardVariants
                .Where(v => v.ScryfallId == variantId)
                .Select(v => v.CardMarketProductId)
                .SingleOrDefaultAsync();

            if (!productId.HasValue || productId.Value <= 0)
                return null;
            
            using var marketDb = await _market.CreateDbContextAsync();

            var price = await marketDb.PriceCaches
                .SingleOrDefaultAsync(p => p.ProductId == productId.Value);

            if (price == null)
                return null;

            return finish == CardFinish.NonFoil
                ? price.PriceTrend
                : price.PriceFoilTrend;
        }

        public async Task<Dictionary<Guid, decimal?>> GetPriceBatchAsync(
            IEnumerable<CollectionAllocationDto> allocationDtos
        )
        {
            using var oracleDb = await _oracle.CreateDbContextAsync();
            using var marketDb = await _market.CreateDbContextAsync();

            var variantIds = allocationDtos
                .Select(a => a.VariantId)
                .Distinct()
                .ToList();

            var productIds = await oracleDb.CardVariants
                .Where(v => variantIds.Contains(v.ScryfallId))
                .Select(v => new
                {
                    v.ScryfallId,
                    v.CardMarketProductId
                })
                .ToListAsync();

            var priceMap = productIds
                .Where(p => p.CardMarketProductId != null)
                .ToDictionary(p => p.ScryfallId, p => p.CardMarketProductId!.Value);

            var prices = await marketDb.PriceCaches
                .Where(p => priceMap.Values.Contains(p.ProductId))
                .ToListAsync();

            var priceLookup = prices.ToDictionary(p => p.ProductId);

            var result = new Dictionary<Guid, decimal?>();

            foreach (var allocation in allocationDtos)
            {
                if (!priceMap.TryGetValue(allocation.VariantId, out var pid))
                {
                    result[allocation.Id] = null;
                    continue;
                }

                if (!priceLookup.TryGetValue(pid, out var price))
                {
                    result[allocation.Id] = null;
                    continue;
                }

                result[allocation.Id] = 
                    allocation.Finish == CardFinish.NonFoil
                        ? price.PriceTrend
                        : price.PriceFoilTrend;
            }

            return result;
        }
    }

}