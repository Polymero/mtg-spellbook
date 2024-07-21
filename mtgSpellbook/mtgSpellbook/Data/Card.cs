using System.ComponentModel.DataAnnotations;

namespace mtgSpellbook.Client.Data
{
    public class Card
    {
        [Key]
        public string? OracleId { get; set; }
        public string? SetName { get; set; }
        public string? SetId { get; set; }
        public string? CollectorNumber { get; set; }
        public string? Finish { get; set; }
        public string? Language { get; set; }
        public string? Thumb { get; set; }
        public string? Image { get; set; }
        public int Quantity { get; set; }
    }
}