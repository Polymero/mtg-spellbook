using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class Binder
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Created { get; set; }
        public string? Updated { get; set; }
        public List<Card> Cards { get; set; } = [];
    }
}