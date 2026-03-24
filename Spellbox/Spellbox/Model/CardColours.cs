

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
                value <<= 2;
                value |= (int) property.GetValue(this)!;
            }

            return value;
        }

        public static CardColours FromInt(int value)
        {
            var colours = new CardColours();

            foreach (var property in typeof(CardColours).GetProperties().OrderBy(x => x.Name).Reverse())
            {
                property.SetValue(colours, value & 1);
                value >>>= 2;
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
            41 => "{G}{W}{U}",
            7 => "{U}{R}{W}",
            42 => "{B}{G}{U}",
            44 => "{R}{W}{B}",
            14 => "{G}{U}{R}",
            45 => "{B}{R}{G}{W}",
            15 => "{R}{G}{W}{U}",
            43 => "{G}{W}{U}{B}",
            _ => (W ? "{W}" : "") + (U ? "{U}" : "") + (B ? "{B}" : "") + (R ? "{R}" : "") + (G ? "{G}" : "")
         };
        }

        // public bool Any() => W | U | B | R | G | C;

        // public static CardColours operator |(CardColours lhs, CardColours rhs)
        // {
        //     return new CardColours
        //     {
        //       W = lhs.W | rhs.W,
        //       U = lhs.U | rhs.U,
        //       B = lhs.B | rhs.B,
        //       R = lhs.R | rhs.R,
        //       G = lhs.G | rhs.G,
        //       C = lhs.C | rhs.C  
        //     };
        // }

        // public static bool operator ==(CardColours lhs, CardColours rhs) =>
        //     (lhs.W == rhs.W) & (lhs.U == rhs.U) & (lhs.B == rhs.B) & (lhs.R == rhs.R) & (lhs.G == rhs.G) & (lhs.C == rhs.C);

        // public static bool operator !=(CardColours lhs, CardColours rhs) => !(lhs == rhs);
    }

}