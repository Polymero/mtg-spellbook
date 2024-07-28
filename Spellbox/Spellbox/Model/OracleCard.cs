using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class OracleCard
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string OracleId { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? TypeLine { get; set; }
        public List<string>? Keywords { get; set; }
        public decimal? CMC { get; set; }
        public List<string>? ColorIdentity { get; set; }
    }
}