using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CardLegality
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? OracleId { get; set; }
        public string? Standard { get; set; }
        public string? Future { get; set; }
        public string? Historic { get; set; }
        public string? Timeless { get; set; }
        public string? Gladiator { get; set; }
        public string? Pioneer { get; set; }
        public string? Explorer { get; set; }
        public string? Modern { get; set; }
        public string? Legacy { get; set; }
        public string? Pauper { get; set; }
        public string? Vintage { get; set; }
        public string? Penny { get; set; }
        public string? Commander { get; set; }
        public string? Oathbreaker { get; set; }
        public string? PauperCommander { get; set; }
        public string? Duel { get; set; }
        public string? Oldschool { get; set; }
        public string? Premodern { get; set; }
        public string? Predh { get; set; } 
    }
}