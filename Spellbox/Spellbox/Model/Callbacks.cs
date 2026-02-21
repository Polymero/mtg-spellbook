

namespace Spellbox.Model
{

    public sealed record EditAllocationCallback(
        CollectionAllocationDto Allocation,
        bool IsUpdated,
        bool IsDeleted
    );

    public sealed record EditBinderCallback(
        CollectionBinderDto Binder,
        bool IsUpdated,
        bool IsDeleted
    );

}
