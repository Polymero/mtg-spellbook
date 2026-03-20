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

        public static Expression<Func<CardVariant, CardVariantDto>> FromEntity => v
            => new()
            {
                ScryfallId = v.ScryfallId,
                OracleId = v.OracleId,
                Name = v.SearchName,
                SetName = v.SetName,
                SetCode = v.SetCode,
                CollNum = v.CollNum,
                Finishes = v.Finishes,
                Artist = v.Artist,
                Released = v.Released,
                Rarity = v.Rarity,
                FlavorTexts = v.FlavorTexts,
                Images = new CardImage(v.ScryfallId, v.IsReversed)
            };
    }

    public class CardImage
    {
        private static string ScryfallId = null!;
        public Side Front;
        public Side Back;

        public CardImage(Guid scryfallId, bool isReversed)
        {
            ScryfallId = scryfallId.ToString();

            if (isReversed)
                ScryfallId = ScryfallId[..^2] + ScryfallId[^1] + ScryfallId[^2];

            Front = new(ScryfallId, isReversed ? "back" : "front");
            Back = new(ScryfallId, isReversed ? "front" : "back");
        }

        public class Side(
            string ScryfallId,
            string side
        )
        {
            readonly string uri = String.Join("/", [
                "https://cards.scryfall.io",
                "{0}",
                side,
                ScryfallId[0],
                ScryfallId[1],
                ScryfallId
            ]) + ".jpg";

            public string Small => String.Format(uri, "small");
            public string Normal => String.Format(uri, "normal");
            public string Large => String.Format(uri, "large");
            public string ArtCrop => String.Format(uri, "art_crop");
        }
    }

}