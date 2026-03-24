using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;


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
        public bool IsReversed { get; set; } = false;

        public string SetName { get; set; } = null!;
        public string SetCode { get; set; } = null!;
        public string CollNum { get; set; } = null!;
        public List<string> Finishes { get; set; } = null!;
        public string? Artist { get; set; }
        public string Released { get; set; } = null!;
        public string Rarity { get; set; } = null!;
        public List<string> FlavorTexts { get; set; } = null!;

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
        public List<string> Finishes { get; init; } = [];
        public string? Artist { get; init; }
        public string Released { get; init; } = null!;
        public string Rarity { get; init; } = null!;
        public List<string> FlavorTexts { get; init; } = [];

        public CardImage Images { get; init; } = null!;

        public int? CardMarketProductId { get; set; }

        public static Expression<Func<CardVariant, CardVariantDto>> FromEntity => e
            => new()
            {
                ScryfallId = e.ScryfallId,
                OracleId = e.OracleId,

                Name = e.SearchName,
                SetName = e.SetName,
                SetCode = e.SetCode,
                CollNum = e.CollNum,
                Finishes = e.Finishes,
                Artist = e.Artist,
                Released = e.Released,
                Rarity = e.Rarity,
                FlavorTexts = e.FlavorTexts,

                Images = new CardImage(e.ScryfallId, e.IsReversed)
            };

        public override string ToString() => $"{SetCode.ToUpper()} ({CollNum.ToUpper()}) - {SetName}";
    }

}