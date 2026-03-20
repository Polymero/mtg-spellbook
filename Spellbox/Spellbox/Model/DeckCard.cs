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

        public DateTime AddedAt { get; set; }
        public DateTime AllocatedAt { get; set; }
    }

    public class DeckCardDto
    {
        public Guid Id { get; init; }
        public Guid ZoneId { get; init; }
        public Guid OracleId { get; init; }
        public Guid VariantId { get; init; }
        public int Quantity { get; init; }
    }
    
}