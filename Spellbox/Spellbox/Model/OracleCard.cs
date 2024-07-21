using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class OracleCard
    {
        [Key]
        public string? OracleId { get; set; }
        public string? Name { get; set; }
        public string? TypeLine { get; set; }
        public List<string>? Keywords { get; set; }
        public string? OracleText { get; set; }
        public string? ManaCost { get; set; }
        public decimal? CMC { get; set; }
        public List<string>? Colors { get; set; }
        public List<string>? ColorIdentity { get; set; }
        public List<CardVariant>? Variants { get; set; }
        public Legalities? Legalities { get; set; }
    }

    public class CardVariant
    {
        [Key]
        public int Id { get; set; }
        public string? SetName { get; set; }
        public string? SetCode { get; set; }
        public string? CollNum { get; set; }
        public List<string>? Finishes { get; set; }
        public string? Artist { get; set; }
        public string? Thumb { get; set; }
        public string? Image { get; set; }
        public string? Language { get; set; }
        public string? Released { get; set; }
        public string? Rarity { get; set; }
        public string? FlavorName { get; set; }
        public string? FlavorText { get; set; }
    }
}