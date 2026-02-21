using Spellbox.Model;


namespace Spellbox.Services
{
    public sealed class CrossService
    {
        
        private readonly CollectionService _collection;
        private readonly OracleService _oracle;

        public CrossService(CollectionService collection, OracleService oracle)
        {
            _collection = collection;
            _oracle = oracle;
        }


        private async Task<List<CollectionGroupedOracleDto>> GroupAllocations(IEnumerable<CollectionAllocationDto> allocations)
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

            return allocations
                .GroupBy(a => a.OracleId)
                .Select(oracleGroup => 
                {
                    var oracle = oracleCards[oracleGroup.Key];

                    return new CollectionGroupedOracleDto
                    {
                        OracleId = oracleGroup.Key,
                        Name = oracle.Name,

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
                })
                .OrderBy(g => g.Name)
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

        public async Task<List<CollectionGroupedOracleDto>> GetCollectionGroupsAsync()
        {
            var allocations = await _collection.GetAllAllocationsAsync();
            return await GroupAllocations(allocations);
        }


        public async Task<CardDetailsViewerDto> GetCardDetailsViewerDtoAsync(
            Guid variantId
        )
        {
            // CardOracleDto oracle = new();
            // List<CardFaceDto> faces = [];
            // CardVariantDto variant;

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


    public sealed class CollectionGroupedOracleDto
    {
        public Guid OracleId { get; init; }
        public string Name { get; init; } = null!;

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
    }

    public sealed class CollectionVariantGroupDto
    {
        // from CollectionAllocations
        public List<CollectionAllocationDto> Allocations { get; set; } = new List<CollectionAllocationDto>();
        public int Quantity { get; set; }

        // from CVariant
        public Guid OracleId { get; init; }
        public Guid VariantId { get; init; }
        public string Name { get; init; } = null!;
        public string SetCode { get; init; } = null!;
        public string CollNum { get; init; } = null!;
        public string Released { get; init; } = null!;
        public List<string> Thumbs { get; init; } = null!;
        public List<string> Images { get; init; } = null!;
        public int? CardMarketProductId { get; init; } = null;
    }

    public sealed class CardDetailsViewerDto
    {
        public List<CollectionAllocationDto> Allocations { get; set; } = [];
        public CardOracleDto Oracle { get; init; } = null!;
        public List<CardFaceDto> Faces { get; init; } = null!;
        public CardVariantDto Variant { get; init; } = null!;
    }

}