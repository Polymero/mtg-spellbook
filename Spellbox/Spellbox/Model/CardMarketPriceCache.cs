using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CardMarketPriceCache
    {
        [Key]
        public int ProductId { get; set; }

        public decimal? PriceLow { get; set; }
        public decimal? PriceTrend { get; set; }
        public decimal? PriceAverage { get; set; }

        public decimal? PriceFoilLow { get; set; }
        public decimal? PriceFoilTrend { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
