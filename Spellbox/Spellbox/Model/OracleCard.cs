using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class OracleCard
    {
        [Key]
        public Guid OracleId { get; set; }

        public string Name { get; set; } = null!;
        public string TypeLine { get; set; } = null!;
        public List<string> Keywords { get; set; } = null!;
        public decimal CMC { get; set; }
        public List<string> ColorIdentity { get; set; } = null!;

        public ICollection<CFace> Faces { get; set; } = new List<CFace>();
        public ICollection<CVariant> Variants { get; set; } = new List<CVariant>();
    }

    public class CFace
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid OracleId { get; set; }
        public OracleCard OracleCard { get; set; } = null!;

        public int Order { get; set; }
        public string Name { get; set; } = null!;
        public string? ManaCost { get; set; }
        public string TypeLine { get; set; } = null!;
        public string? OracleText { get; set; }
        public string? Power { get; set; }
        public string? Toughness { get; set; }
        public string? Defense { get; set; }
    }

    public class CVariant
    {
        [Key]
        public Guid ScryfallId { get; set; }

        [Required]
        public Guid OracleCardId { get; set; }
        public OracleCard OracleCard { get; set; } = null!;

        public string SetName { get; set; } = null!;
        public string SetCode { get; set; } = null!;
        public string CollNum { get; set; } = null!;
        public List<string> Finishes { get; set; } = null!;
        public string? Artist { get; set; }
        public string Released { get; set; } = null!;
        public string Rarity { get; set; } = null!;
        public List<string> Thumbs { get; set; } = null!;
        public List<string> Images { get; set; } = null!;
        public List<string> FlavorTexts { get; set; } = null!;
        public string? FlavorName { get; set; }
    }
}