using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Spellbox.Contexts;
using Spellbox.Model;


namespace Spellbox.Services
{

    public interface IPricingService
    {
        PricingMarketplace Marketplace { get; }

        Task<(decimal?, decimal?)?> GetPriceByPriceIdAsync(
            int priceId,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        );

        Task<decimal?> GetPriceAsync(
            Guid variantId,
            CardFinish finish,
            CardLanguage language,
            CardCondition condition,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        );

        Task<Dictionary<Guid, decimal?>> GetPriceBatchAsync(
            IEnumerable<CollectionAllocationDto> allocationDtos,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        );

        Task<(decimal, int)> GetUnassignedValueAsync(
            PriceMetric nonFoildMetric,
            PriceMetric foilMetric
        );

        Task<(decimal, int)> GetBinderValueAsync(
            Guid binderId,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        );

        Task<(decimal, int)> GetDeckValueAsync(
            Guid activeSnapshotId,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        );

        Task<List<PricingEditDto>> GetPricingEditsAsync(
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        );
    }


    public sealed class CardMarketPricingService : IPricingService
    {        
        private readonly IDbContextFactory<OracleDbContext> _oracle;
        private readonly IDbContextFactory<CardMarketDbContext> _market;
        private readonly IDbContextFactory<CollectionDbContext> _collection;

        public PricingMarketplace Marketplace => PricingMarketplace.CardMarket;

        public CardMarketPricingService(
            IDbContextFactory<OracleDbContext> oracle, 
            IDbContextFactory<CardMarketDbContext> market,
            IDbContextFactory<CollectionDbContext> collection
        )
        {
            _oracle = oracle;
            _market = market;
            _collection = collection;
        }


        public async Task<(decimal?, decimal?)?> GetPriceByPriceIdAsync(
            int priceId,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        )
        {
            using var marketDb = await _market.CreateDbContextAsync();

            var price = await marketDb.PriceCaches
                .SingleOrDefaultAsync(p => p.ProductId == priceId);

            if (price == null)
                return null;

            return (
                nonFoilMetric switch
                {
                    PriceMetric.Low => price.Low,
                    PriceMetric.Avg => price.Avg,
                    PriceMetric.Trend => price.Trend,
                    _ => null
                },
                foilMetric switch
                {
                    PriceMetric.Low => price.FoilLow,
                    PriceMetric.Avg => price.FoilAvg,
                    PriceMetric.Trend => price.FoilTrend,
                    _ => null
                }
            );
        }


        public async Task<decimal?> GetPriceAsync(
            Guid variantId,
            CardFinish finish,
            CardLanguage language,
            CardCondition condition,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        )
        {
            using var oracleDb = await _oracle.CreateDbContextAsync();

            var productId = await oracleDb.Variants
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

            if (finish == CardFinish.NonFoil)
            {
                return nonFoilMetric switch
                {
                    PriceMetric.Low => price.Low,
                    PriceMetric.Avg => price.Avg,
                    PriceMetric.Trend => price.Trend,
                    _ => null
                };
            }
            else
            {
                return foilMetric switch
                {
                    PriceMetric.Low => price.FoilLow,
                    PriceMetric.Avg => price.FoilAvg,
                    PriceMetric.Trend => price.FoilTrend,
                    _ => null
                };
            }
            
        }

        public async Task<Dictionary<Guid, decimal?>> GetPriceBatchAsync(
            IEnumerable<CollectionAllocationDto> allocationDtos,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        )
        {
            using var oracleDb = await _oracle.CreateDbContextAsync();
            using var marketDb = await _market.CreateDbContextAsync();

            var variantIds = allocationDtos
                .Select(a => a.VariantId)
                .Distinct()
                .ToList();

            var productIds = await oracleDb.Variants
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

                if (allocation.Finish == CardFinish.NonFoil)
                {
                    result[allocation.Id] = nonFoilMetric switch
                    {
                        PriceMetric.Low => price.Low,
                        PriceMetric.Avg => price.Avg,
                        PriceMetric.Trend => price.Trend,
                        _ => null
                    };
                }
                else
                {
                    result[allocation.Id] = foilMetric switch
                    {
                        PriceMetric.Low => price.FoilLow,
                        PriceMetric.Avg => price.FoilAvg,
                        PriceMetric.Trend => price.FoilTrend,
                        _ => null
                    };
                }
            }

            return result;
        }


        public async Task<(decimal, int)> GetUnassignedValueAsync(
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        )
        {
            using var collection = await _collection.CreateDbContextAsync();

            var allocations = await collection.Allocations
                .Where(a => a.AllocationIndex == AllocationIndex.Unassigned)
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    VariantId = a.CollectionCard.VariantId,
                    Finish = a.Finish
                })
                .ToListAsync();

            var prices = await GetPriceBatchAsync(allocations, nonFoilMetric, foilMetric);

            var total = prices.Values.Where(p => p.HasValue).Sum();
            var missing = prices.Values.Count(p => !p.HasValue);

            return (total ?? 0m, missing);
        }

        public async Task<(decimal, int)> GetBinderValueAsync(
            Guid binderId,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        )
        {
            using var collection = await _collection.CreateDbContextAsync();

            var allocations = await collection.Allocations
                .Where(a => a.BinderId == binderId)
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    VariantId = a.CollectionCard.VariantId,
                    Finish = a.Finish
                })
                .ToListAsync();

            var prices = await GetPriceBatchAsync(allocations, nonFoilMetric, foilMetric);

            var total = prices.Values.Where(p => p.HasValue).Sum();
            var missing = prices.Values.Count(p => !p.HasValue);

            return (total ?? 0m, missing);
        }

        public async Task<(decimal, int)> GetDeckValueAsync(
            Guid activeSnapshotId,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        )
        {
            using var collection = await _collection.CreateDbContextAsync();

            var allocations = await collection.Allocations
                .Where(a => a.SnapshotId == activeSnapshotId)
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    VariantId = a.CollectionCard.VariantId,
                    Finish = a.Finish
                })
                .ToListAsync();

            var prices = await GetPriceBatchAsync(allocations, nonFoilMetric, foilMetric);

            var total = prices.Values.Where(p => p.HasValue).Sum();
            var missing = prices.Values.Count(p => !p.HasValue);

            return (total ?? 0m, missing);
        }


        public async Task<List<PricingEditDto>> GetPricingEditsAsync(
            PriceMetric noinFoilMetric,
            PriceMetric foilMetric
        )
        {
            using var collection = await _collection.CreateDbContextAsync();

            var variantIds = await collection.CollectionCards
                .Select(c => c.VariantId)
                .Distinct()
                .ToListAsync();

            using var oracle = await _oracle.CreateDbContextAsync();

            return await oracle.Variants
                .Where(v => 
                    variantIds.Contains(v.ScryfallId) &&
                    v.CardMarketProductId == null
                )
                .Select(v => new PricingEditDto
                {
                    OracleId = v.OracleId,
                    VariantId = v.ScryfallId,
                    Name = v.SearchName,
                    SetName = v.SetName,
                    SetCode = v.SetCode,
                    CollNum = v.CollNum,
                    CardMarketProductId = v.CardMarketProductId
                })
                .ToListAsync();
        }

    }
}