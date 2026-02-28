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
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<DeckZone> Zones { get; set; } = [];
    }
}