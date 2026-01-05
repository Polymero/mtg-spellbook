using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class DeckZone
    {
        [Key]
        public Guid Id { get; set; }

        public Guid SnapshotId { get; set; }
        public DeckSnapshot Snapshot { get; set; } = null!;

        public DeckZoneType ZoneType { get; set; }

        public ICollection<DeckCard> Cards { get; set; } = new List<DeckCard>();
    }

    public enum DeckZoneType
    {
        Commanders = 0,
        Companions = 1,
        Mainboard = 2,
        Sideboard = 3,
        Maybeboard = 4
    }
}