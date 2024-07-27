using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CollectionSnapshot
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Created { get; set; }
        public List<CollectionCard>? Commanders { get; set; }
        public List<CollectionCard>? Companions { get; set; }
        public List<CollectionCard>? Mainboard { get; set; }
        public List<CollectionCard>? Sideboard { get; set; }
        public List<CollectionCard>? Maybeboard { get; set; }
    }
}