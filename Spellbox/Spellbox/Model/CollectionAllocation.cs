using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public class CollectionAllocation
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CollectionCardId { get; set; }
        public CollectionCard CollectionCard { get; set; } = null!;

        public AllocationIndex AllocationIndex { get; set; }

        public Guid? BinderId { get; set; }
        public Guid? SnapshotId { get; set; }

        public CardFinish Finish { get; set; } = CardFinish.Unknown;
        public CardLanguage Language { get; set; } = CardLanguage.Unknown;
        public CardCondition Condition { get; set; } = CardCondition.Unknown;

        public bool IsAltered { get; set; } = false;
        public bool IsSigned { get; set; } = false;

        public DateTime AllocatedAt { get; set; }
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
        ChineseTraditional = 11
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
}