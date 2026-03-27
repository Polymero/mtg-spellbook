using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;


namespace Spellbox.Model
{

    public class CardOracle
    {
        [Key]
        public Guid OracleId { get; set; }

        public string Name { get; set; } = null!;
        public string TypeLine { get; set; } = null!;
        public List<string> Keywords { get; set; } = [];
        public int CMC { get; set; }
        
        public int Colors { get; set; }
        public int ColorIdentity { get; set; }
        public int Legalities { get; set; } = 0;

        public ICollection<CardFace> Faces { get; set; } = [];
        public ICollection<CardVariant> Variants { get; set; } = [];
    }

    public class CardOracleDto
    {
        public Guid OracleId { get; init; }

        public string Name { get; init; } = null!;
        public string TypeLine { get; init; } = null!;
        public List<string> Keywords { get; init; } = null!;
        public decimal CMC { get; init; }

        public CardColours Colors { get; init; } = null!;
        public CardColours ColorIdentity { get; init; } = null!;
        public CardLegality Legalities { get; init; } = null!;

        public List<string?> ManaCosts { get; init; } = null!;

        public static Expression<Func<CardOracle, CardOracleDto>> FromEntity => e
            => new()
            {
                OracleId = e.OracleId,

                Name = e.Name,
                TypeLine = e.TypeLine,
                Keywords = e.Keywords,
                CMC = (decimal) (e.CMC / 10),

                Colors = CardColours.FromInt(e.Colors),
                ColorIdentity = CardColours.FromInt(e.ColorIdentity),
                Legalities = CardLegality.FromInt(e.Legalities),

                ManaCosts = e.Faces
                    .Select(f => f.ManaCost)
                    .ToList()
            };

        public override string ToString() => Name;
    }

}