using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;


namespace Spellbox.Model
{

    public class Deck
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public DeckType Type { get; set; } = DeckType.Unassigned;
        public string? Description { get; set; }
        public string? CoverImage { get; set; }
        public string? Sleeves { get; set; }

        public List<string> ColorIdentity { get; set; } = [];
        public bool LegalityStatus { get; set; } = false;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<DeckSnapshot> Snapshots { get; set; } = [];
    }


    public sealed class DeckDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public DeckType Type { get; init; }
        public string? Description { get; init; }
        public string? CoverImage { get; init; }
        public string? Sleeves { get; init; }

        public List<string> ColorIdentity { get; init; } = [];
        public bool LegalityStatus { get; init; } = false;

        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        
        public Guid ActiveSnapshotId { get; init; }
        public Guid ActiveMainboardId { get; init; }
        public int Quantity { get; init; }

        public IEnumerable<Guid> SnapshotIds { get; set; } = [];
        public IDictionary<DeckZoneType, Guid> ActiveZoneIds { get; set; } = new Dictionary<DeckZoneType, Guid>();

        public decimal PriceValue { get; set; }
        public int PriceMissing { get; set; }

        public static Expression<Func<Deck, DeckDto>> FromEntity => d
            => new()
            {
                Id = d.Id,

                Name = d.Name,
                Type = d.Type,
                Description = d.Description,
                CoverImage = d.CoverImage,
                Sleeves = d.Sleeves,

                ColorIdentity = d.ColorIdentity,
                LegalityStatus = d.LegalityStatus,

                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,

                ActiveSnapshotId = d.Snapshots
                    .Single(s => s.IsActive)
                    .Id,
                ActiveMainboardId = d.Snapshots
                    .First(s => s.IsActive)
                    .Zones
                    .First(z => z.ZoneType == DeckZoneType.Mainboard)
                    .Id,

                Quantity = d.Snapshots
                    .First(s => s.IsActive)
                    .Zones
                    .Sum(z => z.Allocations.Count) +
                    d.Snapshots
                        .First(s => s.IsActive)
                        .Zones
                        .Sum(z => z.Cards.Count)
            };
    }


    public sealed class EditableDeckDto
    {
        public Guid Id { get; init; }

        public string Name { get; set; } = null!;
        public DeckType Type { get; set; }
        public string? Description { get; set; }
        public string? CoverImage { get; set; }
        public string? Sleeves { get; set; }

        public static Expression<Func<Deck, EditableDeckDto>> FromEntity => d
            => new()
            {
                Id = d.Id,

                Name = d.Name,
                Type = d.Type,
                Description = d.Description,
                CoverImage = d.CoverImage,
                Sleeves = d.Sleeves
            };
    }


    public enum DeckType
    {
        Unassigned = 0,
        Commander = 1,
        Oathbreaker = 2
    }

}