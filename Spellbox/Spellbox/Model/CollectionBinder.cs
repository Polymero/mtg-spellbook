using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CollectionBinder
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<CollectionAllocation> Cards { get; set; } = new List<CollectionAllocation>();
    }

    public sealed class CollectionBinderDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}