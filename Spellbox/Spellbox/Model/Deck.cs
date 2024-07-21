using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class Deck
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public string? Created { get; set; }
        public string? Updated { get; set; }
        public List<Snapshot>? Snapshots { get; set; }
    }
}