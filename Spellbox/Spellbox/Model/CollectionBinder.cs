using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CollectionBinder
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? CoverImage { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<CollectionAllocation> Cards { get; set; } = [];
    }

    public sealed class CollectionBinderDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public string? CoverImage { get; init; }

        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }

        // CollectionService
        public int Quantity { get; init; }

        // PricingService
        public decimal PriceValue { get; set; }
        public int PriceMissing { get; set; }
    }

    public sealed class EditableBinderDto
    {
        public Guid Id { get; init; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? CoverImage { get; set; }
    }
}