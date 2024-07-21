using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class Snapshot
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Created { get; set; }
        public List<Card>? Commanders { get; set; }
        public List<Card>? Companions { get; set; }
        public List<Card>? Mainboard { get; set; }
        public List<Card>? Sideboard { get; set; }
        public List<Card>? Maybeboard { get; set; }
    }
}