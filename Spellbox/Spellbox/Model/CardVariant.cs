using System.ComponentModel.DataAnnotations;


namespace Spellbox.Model
{

    public class CardVariant
    {
        [Key]
        public Guid ScryfallId { get; set; }

        [Required]
        public Guid OracleId { get; set; }
        public CardOracle Oracle { get; set; } = null!;

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

    public sealed class CardVariantDto
    {
        public Guid ScryfallId { get; init; }
        public Guid OracleId { get; init; }

        public string Name { get; init; } = null!;
        public string SetName { get; init; } = null!;
        public string SetCode { get; init; } = null!;
        public string CollNum { get; init; } = null!;
        public List<string> Finishes { get; init; } = null!;
        public string? Artist { get; init; }
        public string Released { get; init; } = null!;
        public string Rarity { get; init; } = null!;
        public List<string> FlavorTexts { get; init; } = null!;

        public List<string> Thumbs { get; init; } = null!;
        public List<string> Images { get; init; } = null!;

        public int? CardMarketProductId { get; set; }
    }

}