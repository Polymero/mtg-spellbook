using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CardVariant
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? OracleId { get; set; }
        public string? SetName { get; set; }
        public string? SetCode { get; set; }
        public string? CollNum { get; set; }
        public List<string>? Finishes { get; set; }
        public string? Artist { get; set; }
        public string? Released { get; set; }
        public string? Rarity { get; set; }
        public List<string>? Thumbs { get; set; }
        public List<string>? Images { get; set; }
        public List<string>? FlavorTexts { get; set; }
        public List<string>? FlavorNames { get; set; }
    }
}