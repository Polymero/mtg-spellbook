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

    public class OracleDto
    {
        public Guid OracleId { get; init; }
        public string Name { get; init; } = null!;
        public string TypeLine { get; init; } = null!;
        public List<string> Keywords { get; init; } = null!;
        public decimal CMC { get; init; }
        public List<string> ColorIdentity { get; init; } = null!;
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
        public string? Loyalty { get; set; }
    }

        public sealed class CFaceDto
    {
        public Guid OracleId { get; init; }
        public int Order { get; init; }
        public string Name { get; init; } = null!;
        public string? ManaCost { get; init; }
        public string TypeLine { get; init; } = null!;
        public string? OracleText { get; init; }
        public string? Power { get; init; }
        public string? Toughness { get; init; }
        public string? Defense { get; init; }
        public string? Loyalty { get; init; }
    }


    public class CVariant
    {
        [Key]
        public Guid ScryfallId { get; set; }

        [Required]
        public Guid OracleCardId { get; set; }
        public OracleCard OracleCard { get; set; } = null!;

        public string SearchName { get; set; } = null!;

        public string SetName { get; set; } = null!;
        public string SetCode { get; set; } = null!;
        public string CollNum { get; set; } = null!;
        public List<string> Finishes { get; set; } = null!;
        public string? Artist { get; set; }
        public string Released { get; set; } = null!;
        public string Rarity { get; set; } = null!;
        public List<string> FlavorTexts { get; set; } = null!;

        public List<string> Thumbs { get; set; } = null!;
        public List<string> Images { get; set; } = null!;

        public int? CardMarketProductId { get; set; }
    }

    public sealed class CVariantDto
    {
        public Guid ScryfallId { get; init; }
        public Guid OracleCardId { get; init; }

        public string Name { get; init; } = null!;
        public string SetName { get; init; } = null!;
        public string SetCode { get; init; } = null!;
        public string CollNum { get; init; } = null!;
        public string? Artist { get; init; }
        public string Released { get; init; } = null!;
        public string Rarity { get; init; } = null!;
        public List<string> FlavorTexts { get; init; } = null!;

        public List<string> Thumbs { get; init; } = null!;
        public List<string> Images { get; init; } = null!;

        public int? CardMarketProductId { get; set; }
    }
}