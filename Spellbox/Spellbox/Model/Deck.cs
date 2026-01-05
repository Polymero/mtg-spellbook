using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class Deck
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<DeckSnapshot> Snapshots { get; set; } = new List<DeckSnapshot>();
    }
}