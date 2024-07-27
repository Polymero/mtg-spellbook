using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CollectionCard
    {
        [Key]
        public int Id { get; set; }
        public string? OracleId { get; set; }
        public int VariantId { get; set; }
        public string? Language { get; set; }
        public int Quantity { get; set; }
    }
}