using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class OracleCard
    {
        [Key]
        public int Id { get; set; }
        public string? OracleId { get; set; }
        public string? Name { get; set; }
        public List<CardFace>? Faces { get; set; }
        public string? TypeLine { get; set; }
        public List<string>? Keywords { get; set; }
        public string? OracleText { get; set; }
        public string? ManaCost { get; set; }
        public decimal? CMC { get; set; }
        public List<string>? Colors { get; set; }
        public List<string>? ColorIdentity { get; set; }
        public List<CardVariant>? Variants { get; set; }
        public CardLegalities? Legalities { get; set; }
    }
}