using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CollectionDeck
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public string? Created { get; set; }
        public string? Updated { get; set; }
        public List<CollectionSnapshot>? Snapshots { get; set; }
    }
}