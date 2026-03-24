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

        public decimal? FoilLow { get; set; }
        public decimal? FoilAvg { get; set; }
        public decimal? FoilTrend { get; set; }
    }

    public class CardMarketProductId
    {
        [Key]
        public Guid VariantId { get; set; }
        public int ProductId { get; set; }
    }

    public class PricingEditDto
    {
        public int? CardMarketProductId { get; set; }

        public Guid OracleId { get; init; }
        public Guid VariantId { get; init; }
        public string Name { get; init; } = null!;
        public string SetName { get; init; } = null!;
        public string SetCode { get; init; } = null!;
        public string CollNum { get; init; } = null!;

        public decimal? PriceNonFoil { get; set; }
        public decimal? PriceFoil { get; set; }
    }

}
