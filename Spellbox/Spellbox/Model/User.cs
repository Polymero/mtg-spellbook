using System.ComponentModel.DataAnnotations;


namespace Spellbox.Model
{

    public sealed class UserProfile
    {
        [Key]
        public Guid Id { get; set; }

        public string DisplayName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


    public sealed class UserPricingSettings
    {
        [Key]
        public Guid Id { get; set; }

        public PricingMarketplace Marketplace { get; set; } = PricingMarketplace.CardMarket;
        public PriceMetric NonFoilMetric { get; set; } = PriceMetric.Trend;
        public PriceMetric FoilMetric { get; set; } = PriceMetric.Trend;

        public DateTime UpdatedAt { get; set; }
    }

    public enum PricingMarketplace
    {
        CardMarket = 1,
        // TcgPlayer = 2,
        // CardKingdom = 3
    }

    public enum PriceMetric
    {
        Low = 1,
        Avg = 2,
        Trend = 3,
        Avg1 = 4,
        Avg7 = 5,
        Avg30 = 6
    }

}