using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;


namespace Spellbox.Model
{

    public class CardFace
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid OracleId { get; set; }
        public CardOracle Oracle { get; set; } = null!;

        public int Order { get; set; }
        public string Name { get; set; } = null!;
        public string TypeLine { get; set; } = null!;
        public string? ManaCost { get; set; }
        public string? OracleText { get; set; }
        public string? Power { get; set; }
        public string? Toughness { get; set; }
        public string? Defense { get; set; }
        public string? Loyalty { get; set; }
    }

    public sealed class CardFaceDto
    {
        public Guid OracleId { get; init; }
        public int Order { get; init; }
        public string Name { get; init; } = null!;
        public string TypeLine { get; init; } = null!;
        public string? ManaCost { get; init; }
        public string? OracleText { get; init; }
        public string? Power { get; init; }
        public string? Toughness { get; init; }
        public string? Defense { get; init; }
        public string? Loyalty { get; init; }

        public static Expression<Func<CardFace, CardFaceDto>> FromEntity => f
            => new()
            {
                OracleId = f.OracleId,
                Name = f.Name,
                TypeLine = f.TypeLine,
                ManaCost = f.ManaCost,
                OracleText = f.OracleText,
                Power = f.Power,
                Toughness = f.Toughness,
                Defense = f.Defense,
                Loyalty = f.Loyalty
            };
    }

}
