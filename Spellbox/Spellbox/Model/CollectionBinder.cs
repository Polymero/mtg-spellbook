using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;


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

        public int Quantity { get; init; }

        public decimal PriceValue { get; set; }
        public int PriceMissing { get; set; }

        public static Expression<Func<CollectionBinder, CollectionBinderDto>> FromEntity => e
            => new()
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                CoverImage = e.CoverImage,

                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,

                Quantity = e.Cards.Count
            };

        public override string ToString() => Name;
    }

    public sealed class EditableBinderDto
    {
        public Guid Id { get; init; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? CoverImage { get; set; }

        public static Expression<Func<CollectionBinder, EditableBinderDto>> FromEntity => e
            => new()
            {
                Id = e.Id,

                Name = e.Name,
                Description = e.Description,
                CoverImage = e.CoverImage
            };
    }
    
}