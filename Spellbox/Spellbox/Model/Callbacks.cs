

namespace Spellbox.Model
{

    public sealed record EditCallback(
        Guid VariantId,
        Guid AllocationId,
        bool IsUpdated,
        bool IsDeleted
    );

}
