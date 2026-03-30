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
            Guid deckId,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        );

        Task<List<PricingEditDto>> GetPricingEditsAsync(
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        );

        Task ApplyPricingEditsAsync(
            List<PricingEditDto> edits,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        );
    }


    public sealed class CardMarketPricingService : IPricingService
    {        
        private readonly IDbContextFactory<OracleDbContext> _oracle;
        private readonly IDbContextFactory<CardMarketDbContext> _market;
        private readonly IDbContextFactory<CollectionDbContext> _collection;
        private readonly ILogger<CardMarketPricingService> _logger;

        public PricingMarketplace Marketplace => PricingMarketplace.CardMarket;

        public CardMarketPricingService(
            IDbContextFactory<OracleDbContext> oracle, 
            IDbContextFactory<CardMarketDbContext> market,
            IDbContextFactory<CollectionDbContext> collection,
            ILogger<CardMarketPricingService> logger
        )
        {
            _oracle = oracle;
            _market = market;
            _collection = collection;
            _logger = logger;
        }


        public async Task<(decimal?, decimal?)?> GetPriceByPriceIdAsync(
            int priceId,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        )
        {
            using var marketDb = await _market.CreateDbContextAsync();

            var price = await marketDb.PriceCaches
                .AsNoTracking()
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
            using var marketDb = await _market.CreateDbContextAsync();

            var productId = await oracleDb.Variants
                .Where(v => v.ScryfallId == variantId)
                .Select(v => v.CardMarketProductId)
                .SingleOrDefaultAsync();

            if (!productId.HasValue)
            {
                productId = await marketDb.AddedProductIds
                    .Where(p => p.VariantId == variantId)
                    .Select(p => p.ProductId)
                    .SingleOrDefaultAsync();
            }

            if (!productId.HasValue || productId.Value <= 0)
                return null;
            
            

            var price = await marketDb.PriceCaches
                .AsNoTracking()
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

            var addedMap = await marketDb.AddedProductIds
                .AsNoTracking()
                .Where(p => variantIds.Contains(p.VariantId))
                .ToDictionaryAsync(p => p.VariantId, p => p.ProductId);

            foreach (var kvp in addedMap)
            {
                if (priceMap.TryGetValue(kvp.Key, out var id))
                    priceMap[kvp.Key] = addedMap[kvp.Key];
                else
                    priceMap.Add(kvp.Key, kvp.Value);
            }

            var prices = await marketDb.PriceCaches
                .AsNoTracking()
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
                .AsNoTracking()
                .Where(a => a.AllocationIndex == AllocationIndex.Unassigned)
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    VariantId = a.VariantId,
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
                .AsNoTracking()
                .Where(a => a.BinderId == binderId)
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    VariantId = a.VariantId,
                    Finish = a.Finish
                })
                .ToListAsync();

            var prices = await GetPriceBatchAsync(allocations, nonFoilMetric, foilMetric);

            var total = prices.Values.Where(p => p.HasValue).Sum();
            var missing = prices.Values.Count(p => !p.HasValue);

            return (total ?? 0m, missing);
        }

        public async Task<(decimal, int)> GetDeckValueAsync(
            Guid deckId,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        )
        {
            using var collection = await _collection.CreateDbContextAsync();

            var allocations = await collection.Decks
                .AsNoTracking()
                .Where(d => d.Id == deckId)
                .SelectMany(d => d.Snapshots)
                .SelectMany(s => s.Zones)
                .SelectMany(z => z.Allocations)
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    VariantId = a.VariantId,
                    Finish = a.Finish
                })
                .ToListAsync();

            var prices = await GetPriceBatchAsync(allocations, nonFoilMetric, foilMetric);

            var total = prices.Values.Where(p => p.HasValue).Sum();
            var missing = prices.Values.Count(p => !p.HasValue);

            return (total ?? 0m, missing);
        }


        public async Task<List<PricingEditDto>> GetPricingEditsAsync(
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        )
        {
            using var collection = await _collection.CreateDbContextAsync();

            var variantIds = await collection.Allocations
                .Select(c => c.VariantId)
                .Distinct()
                .ToListAsync();

            using var market = await _market.CreateDbContextAsync();

            var addedIds = await market.AddedProductIds
                .AsNoTracking()
                .Where(p => variantIds.Contains(p.VariantId))
                .Select(p => p.VariantId)
                .ToListAsync();

            using var oracle = await _oracle.CreateDbContextAsync();

            return await oracle.Variants
                .Where(v => 
                    variantIds.Contains(v.ScryfallId) &&
                    v.CardMarketProductId == null &&
                    !addedIds.Contains(v.ScryfallId)
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

        public async Task ApplyPricingEditsAsync(
            List<PricingEditDto> edits,
            PriceMetric nonFoilMetric,
            PriceMetric foilMetric
        )
        {
            using var market = await _market.CreateDbContextAsync();

            foreach (var edit in edits)
            {
                if (edit.CardMarketProductId is null)
                    continue;

                try
                {
                    market.AddedProductIds.Add(new CardMarketProductId
                    {
                       VariantId = edit.VariantId,
                       ProductId = edit.CardMarketProductId.Value 
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add pricing ID for {VariantId}", edit.VariantId);
                    continue;
                }
            }

            await market.SaveChangesAsync();
        }

    }
}