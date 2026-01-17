using Microsoft.EntityFrameworkCore;

namespace Spellbox.Model
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


        public async Task<List<CollectionCardDetailsDto>> GetBinderCardsAsync(Guid binderId)
        {
            var allocations = await _collection.GetBinderAllocationsAsync(binderId);

            if (allocations.Count == 0)
                return new List<CollectionCardDetailsDto>();

            var variantIds = allocations
                .Select(a => a.VariantId)
                .Distinct()
                .ToList();

            var variants = await _oracle.GetVariantsByIdsAsync(variantIds);

            return allocations.Select(a =>
            {
                var v = variants[a.VariantId];

                return new CollectionCardDetailsDto
                {
                    AllocationId = a.Id,
                    Finish = a.Finish,
                    Language = a.Language,
                    Condition = a.Condition,
                    IsAltered = a.IsAltered,
                    IsSigned = a.IsSigned,
                    Name = v.Name,
                    SetCode = v.SetCode,
                    CollNum = v.CollNum,
                    Thumbs = v.Thumbs,
                    Images = v.Images
                };
            })
            .OrderBy(c => c.Name)
            .ToList();
        }


        public async Task<CardViewerSingleDto> GetCardByAllocationIdAsync(Guid allocationId)
        {
            var allocation = await _collection.GetSingleAllocationAsync(allocationId);

            var variant = await _oracle.GetVariantsByIdsAsync(new List<Guid> { allocation.VariantId });
            var v = variant[allocation.VariantId];

            (OracleDto oracle, List<CFaceDto> faces) = await _oracle.GetSingleOracleAsync(allocation.OracleId);

            return new CardViewerSingleDto
            {
                OracleId = oracle.OracleId,
                Name = oracle.Name,
                Faces = faces,
                ScryfallId = v.ScryfallId,
                SetCode = v.SetCode,
                CollNum = v.CollNum,
                Artist = v.Artist,
                Released = v.Released,
                Rarity = v.Rarity,
                FlavorTexts = v.FlavorTexts,
                Thumbs = v.Thumbs,
                Images = v.Images,
                AllocationId = allocation.Id,
                Finish = allocation.Finish,
                Language = allocation.Language,
                Condition = allocation.Condition,
                IsAltered = allocation.IsAltered,
                IsSigned = allocation.IsSigned
            };
        }

    }


    public sealed class CollectionCardDetailsDto
    {
        // from CollectionAllocation
        public Guid AllocationId { get; init; }

        public CardFinish Finish { get; init; }
        public CardLanguage Language { get; init; }
        public CardCondition Condition { get; init; }

        public bool IsAltered { get; init; }
        public bool IsSigned { get; init; }

        // from CVariant
        public string Name { get; init; } = null!;
        public string SetCode { get; init; } = null!;
        public string CollNum { get; init; } = null!;
        public List<string> Thumbs { get; init; } = null!;
        public List<string> Images { get; init; } = null!;
    }


    public sealed class CardViewerSingleDto
    {
        // Oracle
        public Guid OracleId { get; init; }
        public string Name { get; init; } = null!;

        // Face(s)
        public ICollection<CFaceDto> Faces { get; init; } = null!;

        // Variant
        public Guid ScryfallId { get; init; }
        public string SetCode { get; init; } = null!;
        public string CollNum { get; init; } = null!;
        public string? Artist { get; init; }
        public string Released { get; init; } = null!;
        public string Rarity { get; init; } = null!;
        public List<string> FlavorTexts { get; init; } = null!;
        public List<string> Thumbs { get; init; } = null!;
        public List<string> Images { get; init; } = null!;
        

        // Collection
        public Guid AllocationId { get; init; }
        public CardFinish Finish { get; set; }
        public CardLanguage Language { get; set; }
        public CardCondition Condition { get; set; }
        public bool IsAltered { get; set; }
        public bool IsSigned { get; set; }
    }
}