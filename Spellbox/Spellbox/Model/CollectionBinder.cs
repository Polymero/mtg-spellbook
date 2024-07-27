using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CollectionBinder
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Created { get; set; }
        public string? Updated { get; set; }
        public List<CollectionCard> Cards { get; set; } = [];
    }
}