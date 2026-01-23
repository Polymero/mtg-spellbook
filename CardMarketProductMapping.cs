using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CardMarketProductMapping
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CardVariantId { get; set; }

        public int ProductId { get; set; }

        public string SetCode { get; set; } = null!;
        public string CollNum { get; set; } = null!;
        public string Name { get; set; } = null!;
        public CardFinish Finish { get; set; } = CardFinish.Unknown;
        public CardLanguage Language { get; set; } = CardLanguage.Unknown;

        public DateTime CreatedAt { get; set; }
    }
}