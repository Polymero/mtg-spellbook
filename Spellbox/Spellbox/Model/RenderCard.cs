

namespace Spellbox.Model
{
    public sealed class RenderCard
    {
        public Guid OracleId { get; init; }
        public Guid? VariantId { get; init; } = null;

        public string Name { get; init; } = null!;
        public int Quantity { get; init; }

        public List<string> Images { get; init; } = [];
        public IReadOnlyList<CollectionAllocationDto> Allocations { get; init; } = [];
    }
}