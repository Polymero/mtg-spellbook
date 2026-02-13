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


        public async Task<List<CollectionVariantGroupDto>> GetBinderVariantGroupsAsync(
            Guid binderId
        )
        {
            var allocations = await _collection.GetBinderAllocationsAsync(binderId);

            if (allocations.Count == 0)
                return new List<CollectionVariantGroupDto>();

            var variantGroups = allocations
                .GroupBy(a => a.VariantId)
                .ToList();

            var variantIds = variantGroups
                .Select(g => g.Key)
                .ToList();
            
            var variants = await _oracle.GetVariantsByIdsAsync(variantIds);

            return variantGroups.Select(g =>
            {
                var variant = variants[g.Key];

                return new CollectionVariantGroupDto
                {
                    OracleId = variant.OracleId,
                    VariantId = g.Key,
                    Name = variant.Name,
                    SetCode = variant.SetCode,
                    CollNum = variant.CollNum,
                    Thumbs = variant.Thumbs,
                    Images = variant.Images,

                    Quantity = g.Count(),

                    Allocations = g.Select(a => new CollectionAllocationDto
                    {
                        Id = a.Id,
                        BinderId = a.BinderId,
                        BinderName = a.BinderName,
                        DeckId = a.DeckId,
                        DeckName = a.DeckName,
                        Finish = a.Finish,
                        Language = a.Language,
                        Condition = a.Condition,
                        IsAltered = a.IsAltered,
                        IsSigned = a.IsSigned,
                        IsStamped = a.IsStamped,
                        BoughtFor = a.BoughtFor,
                        OracleId = variant.OracleId,
                        VariantId = variant.ScryfallId
                    }).ToList()
                };
            }).OrderBy(g => g.Name).ToList();
        }

        public async Task<List<CollectionVariantGroupDto>> GetUnassignedVariantGroupsAsync()
        {
            var allocs = await _collection.GetUnassignedAllocationsAsync();

            if (allocs.Count == 0)
                return new List<CollectionVariantGroupDto>();

            var variantGroups = allocs
                .GroupBy(a => a.VariantId)
                .ToList();

            var variantIds = variantGroups
                .Select(g => g.Key)
                .ToList();
            
            var variants = await _oracle.GetVariantsByIdsAsync(variantIds);

            return variantGroups.Select(g =>
            {
                var variant = variants[g.Key];

                return new CollectionVariantGroupDto
                {
                    VariantId = g.Key,
                    Name = variant.Name,
                    SetCode = variant.SetCode,
                    CollNum = variant.CollNum,
                    Thumbs = variant.Thumbs,
                    Images = variant.Images,

                    Quantity = g.Count(),

                    Allocations = g.Select(a => new CollectionAllocationDto
                    {
                        Id = a.Id,
                        BinderId = a.BinderId,
                        BinderName = a.BinderName,
                        DeckId = a.DeckId,
                        DeckName = a.DeckName,
                        Finish = a.Finish,
                        Language = a.Language,
                        Condition = a.Condition,
                        IsAltered = a.IsAltered,
                        IsSigned = a.IsSigned,
                        IsStamped = a.IsStamped,
                        BoughtFor = a.BoughtFor,
                        OracleId = variant.OracleId,
                        VariantId = variant.ScryfallId
                    }).ToList()
                };
            }).OrderBy(g => g.Name).ToList();
        }

        public async Task<List<CollectionVariantGroupDto>> GetCollectionVariantGroupsAsync()
        {
            var allocs = await _collection.GetAllAllocationsAsync();

            if (allocs.Count == 0)
                return new List<CollectionVariantGroupDto>();

            var variantGroups = allocs
                .GroupBy(a => a.VariantId)
                .ToList();

            var variantIds = variantGroups
                .Select(g => g.Key)
                .ToList();
            
            var variants = await _oracle.GetVariantsByIdsAsync(variantIds);

            return variantGroups.Select(g =>
            {
                var variant = variants[g.Key];

                return new CollectionVariantGroupDto
                {
                    OracleId = variant.OracleId,
                    VariantId = g.Key,
                    Name = variant.Name,
                    SetCode = variant.SetCode,
                    CollNum = variant.CollNum,
                    Thumbs = variant.Thumbs,
                    Images = variant.Images,

                    Quantity = g.Count(),

                    Allocations = g.Select(a => new CollectionAllocationDto
                    {
                        Id = a.Id,
                        BinderId = a.BinderId,
                        BinderName = a.BinderName,
                        DeckId = a.DeckId,
                        DeckName = a.DeckName,
                        Finish = a.Finish,
                        Language = a.Language,
                        Condition = a.Condition,
                        IsAltered = a.IsAltered,
                        IsSigned = a.IsSigned,
                        IsStamped = a.IsStamped,
                        BoughtFor = a.BoughtFor,
                        OracleId = variant.OracleId,
                        VariantId = variant.ScryfallId
                    }).ToList()
                };
            }).OrderBy(g => g.Name).ToList();
        }


        public async Task<CardDetailsViewerDto> GetCardDetailsViewerDtoAsync(
            Guid? oracleId = null,
            Guid? variantId = null,
            Guid? binderId = null,
            Guid? snapshotId = null
        )
        {
            if (oracleId is null && variantId is null)
                return new CardDetailsViewerDto();

            if (binderId is not null && snapshotId is not null)
                return new CardDetailsViewerDto();

            CardOracleDto oracle = new();
            List<CardFaceDto> faces = [];
            CardVariantDto variant;

            if (oracleId is not null)
                (oracle, faces) = await _oracle.GetSingleOracleAsync(oracleId.Value);

            if (variantId is not null)
            {
                variant = (await _oracle.GetVariantsByIdsAsync([variantId.Value]))[variantId.Value];
            }
            else
            {
                variant = (await _oracle.GetVariantsByOracleIdAsync(oracleId!.Value)).First();
            }

            if (oracleId is null)
                (oracle, faces) = await _oracle.GetSingleOracleAsync(variant.OracleId);

            var allocations = await _collection.GetCardDetailsAllocationDtosAsync(oracleId, variantId, binderId, snapshotId);

            return new CardDetailsViewerDto
            {
                Oracle = oracle,
                Faces = faces,
                Variant = variant,
                Allocations = allocations
            };
        }




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
        public List<string> Thumbs { get; init; } = null!;
        public List<string> Images { get; init; } = null!;
        public int? CardMarketProductId { get; init; } = null;
    }

    public sealed class CardDetailsViewerDto
    {
        public List<CollectionAllocationDto> Allocations { get; init; } = new List<CollectionAllocationDto>();
        public CardOracleDto Oracle { get; init; } = null!;
        public List<CardFaceDto> Faces { get; init; } = null!;
        public CardVariantDto Variant { get; init; } = null!;
    }

}