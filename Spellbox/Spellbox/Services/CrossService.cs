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


        // public async Task<List<CollectionCardDetailsDto>> GetBinderCardsAsync(Guid binderId)
        // {
        //     var allocations = await _collection.GetBinderAllocationsAsync(binderId);

        //     if (allocations.Count == 0)
        //         return new List<CollectionCardDetailsDto>();

        //     var variantIds = allocations
        //         .Select(a => a.VariantId)
        //         .Distinct()
        //         .ToList();

        //     var variants = await _oracle.GetVariantsByIdsAsync(variantIds);

        //     return allocations.Select(a =>
        //     {
        //         var v = variants[a.VariantId];

        //         return new CollectionCardDetailsDto
        //         {
        //             AllocationId = a.Id,
        //             Finish = a.Finish,
        //             Language = a.Language,
        //             Condition = a.Condition,
        //             IsAltered = a.IsAltered,
        //             IsSigned = a.IsSigned,
        //             Name = v.Name,
        //             SetCode = v.SetCode,
        //             CollNum = v.CollNum,
        //             Thumbs = v.Thumbs,
        //             Images = v.Images
        //         };
        //     })
        //     .OrderBy(c => c.Name)
        //     .ToList();
        // }

        public async Task<List<CollectionVariantGroupDto>> GetCollapsedVariantsAsync(
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
                        Finish = a.Finish,
                        Language = a.Language,
                        Condition = a.Condition,
                        IsAltered = a.IsAltered,
                        IsSigned = a.IsSigned,
                        OracleId = variant.OracleCardId,
                        VariantId = variant.ScryfallId
                    }).ToList()
                };
            }).OrderBy(g => g.Name).ToList();
        }




    }


    public sealed class CollectionVariantGroupDto
    {
        // from CollectionAllocations
        public IReadOnlyList<CollectionAllocationDto> Allocations { get; init; } = new List<CollectionAllocationDto>();
        public int Quantity { get; init; }

        // from CVariant
        public Guid VariantId { get; init; }
        public string Name { get; init; } = null!;
        public string SetCode { get; init; } = null!;
        public string CollNum { get; init; } = null!;
        public List<string> Thumbs { get; init; } = null!;
        public List<string> Images { get; init; } = null!;
    }

}