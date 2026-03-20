using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;


namespace Spellbox.Model
{

    public class CollectionAllocation
    {
        [Key]
        public Guid Id { get; set; }

        public Guid OracleId { get; set; }
        public Guid VariantId { get; set; }

        public AllocationIndex AllocationIndex { get; set; }

        public Guid? BinderId { get; set; }
        public CollectionBinder? Binder { get; set; }

        public Guid? ZoneId { get; set; }
        public DeckZone? Zone { get; set; }
        public Guid? DeckId { get; set; }
        public Deck? Deck { get; set; }

        public CardFinish Finish { get; set; } = CardFinish.Unknown;
        public CardLanguage Language { get; set; } = CardLanguage.Unknown;
        public CardCondition Condition { get; set; } = CardCondition.Unknown;

        public bool IsAltered { get; set; } = false;
        public bool IsSigned { get; set; } = false;
        public bool IsStamped { get; set; } = false;
        public bool IsMisprint { get; set; } = false;

        public decimal? BoughtFor { get; set; }

        public DateTime AddedAt { get; set; }
        public DateTime AllocatedAt { get; set; }
    }

    public sealed class CollectionAllocationDto
    {
        public Guid Id { get; init; }
        public AllocationType Type { get; init; } = AllocationType.Collection;

        public Guid? BinderId { get; init; }
        public string? BinderName { get; init; }
        public Guid? ZoneId { get; init; }
        public string? DeckName { get; init; }

        // from CollectionCard
        public Guid OracleId { get; init; }
        public Guid VariantId { get; init; }

        public CardFinish Finish { get; init; }
        public CardLanguage Language { get; init; }
        public CardCondition Condition { get; init; }
        
        public bool IsAltered { get; init; }
        public bool IsSigned { get; init; }
        public bool IsStamped { get; init; }
        public bool IsMisprint { get; init; }

        public decimal? BoughtFor { get; init; }

        public DateTime AddedAt { get; init; }
        public DateTime AllocatedAt { get; init; }

        public decimal? Price { get; set; }


        public static Expression<Func<CollectionAllocation, CollectionAllocationDto>> FromCollectionEntity => a
            => new()
            {
                Id = a.Id,
                Type = AllocationType.Collection,
                BinderId = a.BinderId,
                BinderName = a.BinderId.HasValue ? a.Binder!.Name : null,
                ZoneId = a.ZoneId,
                DeckName = a.ZoneId.HasValue ? a.Zone!.Snapshot.Deck.Name : null,
                OracleId = a.OracleId,
                VariantId = a.VariantId,
                Finish = a.Finish,
                Language = a.Language,
                Condition = a.Condition,
                IsAltered = a.IsAltered,
                IsMisprint = a.IsMisprint,
                IsSigned = a.IsSigned,
                IsStamped = a.IsStamped,
                BoughtFor = a.BoughtFor,
                AddedAt = a.AddedAt,
                AllocatedAt = a.AllocatedAt
            };

        public static Expression<Func<DeckCard, CollectionAllocationDto>> FromGhostEntity => a
            => new()
            {
                Type = AllocationType.Ghost,
                // BinderId = a.BinderId,
                // BinderName = a.BinderId.HasValue ? a.Binder!.Name : null,
                ZoneId = a.ZoneId,
                // DeckName = a.ZoneId.HasValue ? a.Zone!.Snapshot.Deck.Name : null,
                DeckName = a.Zone.Snapshot.Deck.Name,
                OracleId = a.OracleId,
                VariantId = a.VariantId,
                AddedAt = a.AddedAt,
                AllocatedAt = a.AllocatedAt
            };
    }

    public sealed class EditableAllocationDto
    {
        public Guid AllocationId { get; init; }

        public Guid OracleId { get; init; }
        public Guid VariantId { get; init; }

        public CardFinish Finish { get; set; }
        public CardLanguage Language { get; set; }
        public CardCondition Condition { get; set; }

        public bool IsAltered { get; set; }
        public bool IsMisprint { get; set; }
        public bool IsSigned { get; set; }
        public bool IsStamped { get; set; }

        public decimal? BoughtFor { get; set; }

        public Guid? BinderId { get; set; }
        public Guid? DeckId { get; set; }
        public Guid? SnapshotId { get; set; }
        public Guid? ZoneId { get; set; }

        public static Expression<Func<CollectionAllocation, EditableAllocationDto>> FromEntity => a
            => new()
            {
                AllocationId = a.Id,
                OracleId = a.OracleId,
                VariantId = a.VariantId,
                Finish = a.Finish,
                Language = a.Language,
                Condition = a.Condition,
                IsAltered = a.IsAltered,
                IsMisprint = a.IsMisprint,
                IsSigned = a.IsSigned,
                IsStamped = a.IsStamped,
                BoughtFor = a.BoughtFor,
                BinderId = a.BinderId,
                DeckId = a.ZoneId.HasValue ? a.Zone!.Snapshot.Deck.Id : null,
                SnapshotId = a.ZoneId.HasValue ? a.Zone!.Snapshot.Id : null,
                ZoneId = a.ZoneId
            };
    }

    public sealed class NewAllocationDto
    {
        public Guid OracleId { get; set; }
        public Guid VariantId { get; set; }

        public string Name { get; set; } = null!;
        public string SetCode { get; set; } = null!;
        public string CollNum { get; set; } = null!;

        public CardFinish Finish { get; set; } = CardFinish.NonFoil;
        public CardLanguage Language { get; set; } = CardLanguage.English;
        public CardCondition Condition { get; set; } = CardCondition.NearMint;

        public bool IsAltered { get; set; } = false;
        public bool IsMisprint { get; set; } = false;
        public bool IsSigned { get; set; } = false;
        public bool IsStamped { get; set; } = false;
        
        public decimal? BoughtFor { get; set; }

        public decimal? Price { get; set; }
    }

    public enum AllocationIndex
    {
        Unassigned = 0,
        Binder = 1,
        Deck = 2
    }

    public enum CardFinish
    {
        Unknown = 0,
        NonFoil = 1,
        Foil = 2,
        EtchedFoil = 3,
        GalaxyFoil = 4,
        SurgeFoil = 5,
        TexturedFoil = 6,
        NeonInkFoil = 7
    }

    public enum CardLanguage
    {
        Unknown = 0,
        English = 1,
        Japanese = 2,
        German = 3,
        French = 4,
        Italian = 5,
        Spanish = 6,
        Portuguese = 7,
        Russian = 8,
        Korean = 9,
        ChineseSimplified = 10,
        ChineseTraditional = 11,
        Phyrexian = 12
    }

    public enum CardCondition
    {
        Unknown = 0,
        Poor = 1,
        Played = 2,
        LightPlayed = 3,
        Good = 4,
        Excellent = 5,
        NearMint = 6,
        Mint = 7
    }

    public enum AllocationType
    {
        Collection = 0,
        Ghost = 1
    }
}