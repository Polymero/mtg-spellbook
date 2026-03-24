

namespace Spellbox.Model
{

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
