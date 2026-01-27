using System.ComponentModel.DataAnnotations;


namespace Spellbox.Model
{

    public class CardOracle
    {
        [Key]
        public Guid OracleId { get; set; }

        public string Name { get; set; } = null!;
        public string TypeLine { get; set; } = null!;
        public List<string> Keywords { get; set; } = null!;
        public decimal CMC { get; set; }
        public List<string> ColorIdentity { get; set; } = null!;

        public ICollection<CardFace> Faces { get; set; } = new List<CardFace>();
        public ICollection<CardVariant> Variants { get; set; } = new List<CardVariant>();
    }

    public class CardOracleDto
    {
        public Guid OracleId { get; init; }
        public string Name { get; init; } = null!;
        public string TypeLine { get; init; } = null!;
        public List<string> Keywords { get; init; } = null!;
        public decimal CMC { get; init; }
        public List<string> ColorIdentity { get; init; } = null!;
    }

}