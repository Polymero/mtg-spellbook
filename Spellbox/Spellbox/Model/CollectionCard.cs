using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CollectionCard
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid OracleId { get; set; }
        public Guid VariantId { get; set; }

        public int Quantity { get; set; }

        public ICollection<CollectionAllocation> Allocations { get; set; } = new List<CollectionAllocation>();
    }
}