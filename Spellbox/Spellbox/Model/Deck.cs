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
        public string? CoverImage { get; set; }

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
        public string? CoverImage { get; init; }

        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        
        public Guid ActiveSnapshotId { get; init; }
        // public List<Guid> ActiveZoneIds { get; init; } = [];
        public Guid ActiveMainboardId { get; init; }
        public int Quantity { get; init; }
        public decimal PriceValue { get; set; }
        public int PriceMissing { get; set; }
    }

    public sealed class EditableDeckDto
    {
        public Guid Id { get; init; }

        public string Name { get; set; } = null!;
        public DeckType Type { get; set; }
        public string? Description { get; set; }
        public string? CoverImage { get; set; }
    }

    public enum DeckType
    {
        Unassigned = 0,
        Commander = 1
    }
}