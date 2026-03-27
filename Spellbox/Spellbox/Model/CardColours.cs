

namespace Spellbox.Model
{

    public sealed class CardColours
    {
        public bool W { get; set; } = false;
        public bool U { get; set; } = false;
        public bool B { get; set; } = false;
        public bool R { get; set; } = false;
        public bool G { get; set; } = false;

        public bool C { get; set; } = false;

        public int ToInt()
        {
            int value = 0;

            foreach (var property in GetType().GetProperties().OrderBy(x => x.Name))
            {
                value <<= 1;
                value |= (bool) property.GetValue(this)! ? 1 : 0;
            }

            return value;
        }

        public static CardColours FromInt(int value)
        {
            var colours = new CardColours();

            foreach (var property in typeof(CardColours).GetProperties().OrderBy(x => x.Name).Reverse())
            {
                property.SetValue(colours, (value & 1) == 1);
                value >>>= 1;
            }

            return colours;
        }

        public static CardColours FromEnumerable(IEnumerable<string> colors)
        {
            return new CardColours
            {
                W = colors.Contains("W"),
                U = colors.Contains("U"),
                B = colors.Contains("B"),
                R = colors.Contains("R"),
                G = colors.Contains("G"),
                C = colors.Contains("C")
            };
        }

        public override string ToString()
        {
         var value = ToInt();

         return value switch
         {
            // WUBRGC -> BCGRUW (32 + 16 + 8 + 4 + 2 + 1)
            9 => "{G}{W}",
            5 => "{R}{W}",
            10 => "{G}{U}",
            13 => "{R}{G}{W}",
            11 => "{G}{W}{U}",
            7 => "{U}{R}{W}",
            42 => "{B}{G}{U}",
            37 => "{R}{W}{B}",
            14 => "{G}{U}{R}",
            45 => "{B}{R}{G}{W}",
            15 => "{R}{G}{W}{U}",
            43 => "{G}{W}{U}{B}",
            _ => (W ? "{W}" : "") + (U ? "{U}" : "") + (B ? "{B}" : "") + (R ? "{R}" : "") + (G ? "{G}" : "")
         };
        }
    }

}