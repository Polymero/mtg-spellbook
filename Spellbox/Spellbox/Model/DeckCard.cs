using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;


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

        public static Expression<Func<DeckCard, DeckCardDto>> FromEntity => e
            => new()
            {
                Id = e.Id,
                ZoneId = e.ZoneId,
                OracleId = e.OracleId,
                VariantId = e.VariantId,
                Quantity = e.Quantity
            };
    }
    
}