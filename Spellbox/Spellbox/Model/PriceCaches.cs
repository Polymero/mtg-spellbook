using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CardMarketPriceCache
    {
        [Key]
        public int ProductId { get; set; }

        public decimal? Low { get; set; }
        public decimal? Avg { get; set; }
        public decimal? Trend { get; set; }
        public decimal? Avg1 { get; set; }
        public decimal? Avg7 { get; set; }
        public decimal? Avg30 { get; set; }

        public decimal? FoilLow { get; set; }
        public decimal? FoilAvg { get; set; }
        public decimal? FoilTrend { get; set; }
        public decimal? FoilAvg1 { get; set; }
        public decimal? FoilAvg7 { get; set; }
        public decimal? FoilAvg30 { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
