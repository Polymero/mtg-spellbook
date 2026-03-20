

namespace Spellbox.Model
{

    public sealed class CardLegality
    {
        public CardLegalityType Standard { get; set; }
        public CardLegalityType Modern { get; set; }
        public CardLegalityType Pioneer { get; set; }
        public CardLegalityType Legacy { get; set; }
        public CardLegalityType Vintage { get; set; }
        public CardLegalityType Pauper { get; set; }
        public CardLegalityType Penny { get; set; }
        public CardLegalityType Commander { get; set; }
        public CardLegalityType Oathbreaker { get; set; }
        public CardLegalityType PauperCommander { get; set; }
        public CardLegalityType DuelCommander { get; set; }
        public CardLegalityType OldSchool { get; set; }
        public CardLegalityType PreModern { get; set; }
        public CardLegalityType PreDH { get; set; }

        public int ToInt()
        {
            int value = 0;

            foreach (var property in GetType().GetProperties().OrderBy(x => x.Name))
            {
                value <<= 2;
                value |= (int) property.GetValue(this)!;
            }

            return value;
        }

        public static CardLegality FromInt(int value)
        {
            var legality = new CardLegality();

            foreach (var property in typeof(CardLegality).GetProperties().OrderBy(x => x.Name).Reverse())
            {
                property.SetValue(legality, (CardLegalityType) (value & 3));
                value >>>= 2;
            }

            return legality;
        }
    }

    public enum CardLegalityType
    {
        NotLegal = 0,
        Legal = 1,
        Restricted = 2,
        Banned = 3
    }

}