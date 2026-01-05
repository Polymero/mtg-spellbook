using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class DeckCard
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ZoneId { get; set; }
        public DeckZone Zone { get; set; } = null!;

        public Guid OracleId { get; set; }
        public Guid VariantId { get; set; }

        public int Quantity { get; set; }
    }
}