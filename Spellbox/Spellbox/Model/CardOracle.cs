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
        public decimal CMC { get; set; }
        public List<string> ColorIdentity { get; set; } = [];

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
        public List<string> ColorIdentity { get; init; } = null!;

        public static Expression<Func<CardOracle, CardOracleDto>> FromEntity => o
            => new()
            {
                OracleId = o.OracleId,
                Name = o.Name,
                TypeLine = o.TypeLine,
                Keywords = o.Keywords,
                CMC = o.CMC,
                ColorIdentity = o.ColorIdentity
            };
    }

}