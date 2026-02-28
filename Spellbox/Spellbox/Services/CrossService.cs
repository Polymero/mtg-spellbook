using System.Text.RegularExpressions;
using Spellbox.Model;


namespace Spellbox.Services
{
    public sealed class CrossService
    {
        
        private readonly CollectionService _collection;
        private readonly OracleService _oracle;
        private readonly IPricingRouter _pricing;

        public CrossService(CollectionService collection, OracleService oracle, IPricingRouter pricing)
        {
            _collection = collection;
            _oracle = oracle;
            _pricing = pricing;
        }


        private async Task<List<CollectionGroupedOracleDto>> GroupAllocations(
            IEnumerable<CollectionAllocationDto> allocations
        )
        {
            if (allocations.Count() == 0)
                return [];

            var oracleIds = allocations
                .Select(a => a.OracleId)
                .Distinct()
                .ToList();

            var oracleCards = await _oracle.GetOraclesByIdsAsync(oracleIds);

            var variantIds = allocations
                .Select(a => a.VariantId)
                .Distinct()
                .ToList();

            var variantCards = await _oracle.GetVariantsByIdsAsync(variantIds);

            var real = allocations
                .Where(a => a.Type == AllocationType.Collection)
                .ToList();
            var pricings = await _pricing.GetPriceBatchAsync(real);

            foreach (var allocation in real)
            {
                allocation.Price = pricings[allocation.Id];
            }

            return allocations
                .GroupBy(a => a.Type)
                .SelectMany(typeGroup =>
                {
                    return typeGroup.GroupBy(a => a.OracleId)
                    .Select(oracleGroup => 
                    {
                        var oracle = oracleCards[oracleGroup.Key];

                        return new CollectionGroupedOracleDto
                        {
                            OracleId = oracleGroup.Key,
                            Name = oracle.Name,

                            Type = typeGroup.Key,

                            Variants = oracleGroup
                                .GroupBy(a => a.VariantId)
                                .Select(variantGroup =>
                                {
                                    var variant = variantCards[variantGroup.Key];

                                    return new CollectionGroupedVariantDto
                                    {
                                        VariantId = variantGroup.Key,
                                        SetCode = variant.SetCode,
                                        CollNum = variant.CollNum,
                                        Released = variant.Released,
                                        Images = variant.Images,
                                        CardMarketProductId = variant.CardMarketProductId,

                                        Allocations = variantGroup
                                            .ToList()
                                    };
                                })
                                .OrderByDescending(g => g.Released)
                                .ThenBy(g => g.SetCode)
                                .ThenBy(g => g.CollNum)
                                .ToList()
                        };
                    });
                })
                .OrderBy(g => g.Name)
                .ThenBy(g => g.Type)
                .ToList();
        }

        public async Task<List<CollectionGroupedOracleDto>> GetUnassignedGroupsAsync()
        {
            var allocations = await _collection.GetUnassignedAllocationsAsync();
            return await GroupAllocations(allocations);
        }

        public async Task<List<CollectionGroupedOracleDto>> GetBinderGroupsAsync(Guid binderId)
        {
            var allocations = await _collection.GetBinderAllocationsAsync(binderId);
            return await GroupAllocations(allocations);
        }

        public async Task<Dictionary<DeckZoneType,List<CollectionGroupedOracleDto>>> GetSnapshotZoneGroupsAsync(Guid snapshotId)
        {
            var zoneAllocations = await _collection.GetZoneAllocationsAsync(snapshotId);

            var tasks = zoneAllocations.Select(async pair =>
            {
                var groups = await GroupAllocations(pair.Value);

                return new KeyValuePair<DeckZoneType,List<CollectionGroupedOracleDto>>(pair.Key, groups);
            });

            var results = await Task.WhenAll(tasks);

            return results.ToDictionary(r => r.Key, r => r.Value);
        }

        public async Task<List<CollectionGroupedOracleDto>> GetCollectionGroupsAsync()
        {
            var allocations = await _collection.GetAllAllocationsAsync();
            return await GroupAllocations(allocations);
        }


        public async Task<CardDetailsViewerDto> GetCardDetailsViewerDtoAsync(
            Guid variantId
        )
        {
            var variant = (await _oracle.GetVariantsByIdsAsync([variantId]))[variantId];
            (var oracle, var faces) = await _oracle.GetSingleOracleAsync(variant.OracleId);

            return new CardDetailsViewerDto
            {
                Oracle = oracle,
                Faces = faces,
                Variant = variant
            };
        }




    }


    // public interface ICardAllocation
    // {
    //     Guid OracleId { get; }
    //     Guid VariantId { get; }

    //     AllocationType Type { get; }

    //     decimal? Price { get; set; }
    // }

    // public enum AllocationType
    // {
    //     Collection = 0,
    //     Ghost = 1
    // }


    // public sealed class CardCollectionAllocation : ICardAllocation
    // {
    //     public CollectionAllocationDto Source { get; }

    //     public CardCollectionAllocation(CollectionAllocationDto source)
    //     {
    //         Source = source;
    //     }

    //     public Guid OracleId => Source.OracleId;
    //     public Guid VariantId => Source.VariantId;

    //     public AllocationType Type => AllocationType.Collection;

    //     public decimal? Price
    //     {
    //         get => Source.Price;
    //         set => Source.Price = value;
    //     }
    // }

    // public sealed class CardGhostAllocation : ICardAllocation
    // {
    //     public DeckCardDto Source { get; }

    //     public CardGhostAllocation(DeckCardDto source)
    //     {
    //         Source = source;
    //     }

    //     public Guid OracleId => Source.OracleId;
    //     public Guid VariantId => Source.VariantId;

    //     public AllocationType Type => AllocationType.Ghost;

    //     public decimal? Price { get; set; }
    // }


    public sealed class CollectionGroupedOracleDto
    {
        public Guid OracleId { get; init; }
        public string Name { get; init; } = null!;

        public AllocationType Type { get; init; }

        public List<CollectionGroupedVariantDto> Variants { get; set; } = [];
    }

    public sealed class CollectionGroupedVariantDto
    {
        public Guid VariantId { get; init; }
        public string SetCode { get; init; } = null!;
        public string CollNum { get; init; } = null!;
        public string Released { get; init; } = null!;
        public List<string> Images { get; init; } = null!;
        public int? CardMarketProductId { get; init; } = null;
        
        public List<CollectionAllocationDto> Allocations { get; set; } = [];
        public int Quantity =>
            Allocations.Count;
    }


    public sealed class CardDetailsViewerDto
    {
        public List<CollectionAllocationDto> Allocations { get; set; } = [];
        public CardOracleDto Oracle { get; init; } = null!;
        public List<CardFaceDto> Faces { get; init; } = null!;
        public CardVariantDto Variant { get; init; } = null!;
    }

}