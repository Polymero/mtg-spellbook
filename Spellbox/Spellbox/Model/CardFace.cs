using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CardFace 
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? OracleId { get; set; }
        public int Order { get; set; }
        public string? Name { get; set; }
        public string? ManaCost { get; set; }
        public string? TypeLine { get; set; }
        public string? OracleText { get; set; }
        public string? Power { get; set; }
        public string? Toughness { get; set; }
        public string? Defense { get; set; }
    }
}