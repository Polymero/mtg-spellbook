using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class DeckSnapshot
    {
        [Key]
        public Guid Id { get; set; }

        public Guid DeckId { get; set; }
        public Deck Deck { get; set; } = null!;

        public bool IsActive { get; set; }
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<DeckZone> Zones { get; set; } = new List<DeckZone>();

        // Only populated if IsActive is true
        public ICollection<CollectionAllocation> Allocations { get; set; } = new List<CollectionAllocation>();

    }
}