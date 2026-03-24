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

        public int ColorIdentity { get; set; }
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

        public CardColours ColorIdentity { get; init; } = null!;
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

        public static Expression<Func<Deck, DeckDto>> FromEntity => e
            => new()
            {
                Id = e.Id,

                Name = e.Name,
                Type = e.Type,
                Description = e.Description,
                CoverImage = e.CoverImage,
                Sleeves = e.Sleeves,

                ColorIdentity = CardColours.FromInt(e.ColorIdentity),
                LegalityStatus = e.LegalityStatus,

                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,

                ActiveSnapshotId = e.Snapshots
                    .Single(s => s.IsActive)
                    .Id,
                ActiveMainboardId = e.Snapshots
                    .First(s => s.IsActive)
                    .Zones
                    .First(z => z.ZoneType == DeckZoneType.Mainboard)
                    .Id,

                Quantity = (
                    e.Snapshots
                        .First(s => s.IsActive)
                        .Zones
                        .Sum(z => z.Allocations.Count) +
                    e.Snapshots
                        .First(s => s.IsActive)
                        .Zones
                        .Sum(z => z.Cards.Count)
                )
            };

        public override string ToString() => Name;
    }

    public sealed class EditableDeckDto
    {
        public Guid Id { get; init; }

        public string Name { get; set; } = null!;
        public DeckType Type { get; set; }
        public string? Description { get; set; }
        public string? CoverImage { get; set; }
        public string? Sleeves { get; set; }

        public static Expression<Func<Deck, EditableDeckDto>> FromEntity => e
            => new()
            {
                Id = e.Id,

                Name = e.Name,
                Type = e.Type,
                Description = e.Description,
                CoverImage = e.CoverImage,
                Sleeves = e.Sleeves
            };
    }

    public enum DeckType
    {
        Unassigned = 0,
        Standard = 1,
        Modern = 2,
        Pioneer = 3,
        Legacy = 4,
        Vintage = 5,
        Pauper = 6,
        Penny = 7,
        Commander = 8,
        Oathbreaker = 9,
        PauperCommander = 10,
        DuelCommander = 11,
        OldSchool = 12,
        PreModern = 13,
        PreDH = 14
    }

}