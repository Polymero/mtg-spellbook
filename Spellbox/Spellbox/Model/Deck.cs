using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class Deck
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public DeckType Type { get; set; } = DeckType.Unassigned;
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<DeckSnapshot> Snapshots { get; set; } = new List<DeckSnapshot>();
    }

    public sealed class DeckDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public DeckType Type { get; init; }
        public string? Description { get; init; }

        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        
        public Guid ActiveSnapshotId { get; init; }
        public int Quantity { get; init; }
    }

    public enum DeckType
    {
        Unassigned = 0,
        Commander = 1
    }
}