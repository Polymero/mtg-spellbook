using Microsoft.EntityFrameworkCore;

using Spellbox.Contexts;
using Spellbox.Model;


namespace Spellbox.Services
{
    public sealed class CollectionService
    {

        private readonly IDbContextFactory<CollectionDbContext> _factory;

        public CollectionService(IDbContextFactory<CollectionDbContext> factory)
        {
            _factory = factory;
        }


        // Card adding
        public async Task AddCopiesAsync(
            Guid oracleId,
            Guid variantId,
            int quantity,
            CardFinish finish,
            CardLanguage language,
            CardCondition condition,
            bool isAltered,
            bool isSigned,
            Guid? binderId = null,
            Guid? snapshotId = null
        )
        {
            using var db = await _factory.CreateDbContextAsync();
            using var tx = await db.Database.BeginTransactionAsync();

            // Get existing collection cards
            var collectionCard = await db.CollectionCards.FirstOrDefaultAsync(c => 
                c.OracleId == oracleId && c.VariantId == variantId);

            // Create entry if unavailable
            if (collectionCard == null)
            {
                collectionCard = new CollectionCard
                {
                    Id = Guid.NewGuid(),
                    OracleId = oracleId,
                    VariantId = variantId
                };
                db.CollectionCards.Add(collectionCard);
            }

            var allocationIndex = 
                binderId != null ? AllocationIndex.Binder :
                snapshotId != null ? AllocationIndex.Deck :
                AllocationIndex.Unassigned;

            for (int i = 0; i < quantity; i++)
            {
                db.Allocations.Add(new CollectionAllocation
                {
                    Id = Guid.NewGuid(),
                    CollectionCardId = collectionCard.Id,
                    AllocationIndex = allocationIndex,
                    BinderId = binderId,
                    SnapshotId = snapshotId,
                    Finish = finish,
                    Language = language,
                    Condition = condition,
                    IsAltered = isAltered,
                    IsSigned = isSigned,
                    AddedAt = DateTime.UtcNow,
                    AllocatedAt = DateTime.UtcNow
                });
            }

            await db.SaveChangesAsync();
            await tx.CommitAsync();
        }


        // Collection overview
        public async Task<int> GetQuantityUnassigned()
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Allocations
                .Where(a => a.AllocationIndex == AllocationIndex.Unassigned)
                .CountAsync();
        }

        public async Task<List<CollectionBinderDto>> GetAllBinders()
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Binders
                .OrderBy(b => b.Name)
                .Select(b => new CollectionBinderDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,

                    Quantity = b.Cards.Count()
                })
                .ToListAsync();
        }

        public async Task<List<DeckDto>> GetAllDecks()
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Decks
                .OrderByDescending(d => d.UpdatedAt)
                .Select(d => new DeckDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Type = d.Type,
                    Description = d.Description,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,

                    ActiveSnapshotId = d.Snapshots
                        .Where(s => s.IsActive)
                        .Select(s => s.Id)
                        .FirstOrDefault(),

                    Quantity = d.Snapshots
                        .Where(s => s.IsActive)
                        .Sum(s => s.Allocations.Count)
                })
                .ToListAsync();
        }


        // Unassigend viewing
        public async Task<List<CollectionAllocationDto>> GetUnassignedAllocationsAsync()
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Allocations
                .Where(a => a.AllocationIndex == AllocationIndex.Unassigned)
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    OracleId = a.CollectionCard.OracleId,
                    VariantId = a.CollectionCard.VariantId,
                    Finish = a.Finish,
                    Language = a.Language,
                    Condition = a.Condition,
                    IsAltered = a.IsAltered,
                    IsSigned = a.IsSigned,
                    IsStamped = a.IsStamped,
                    BoughtFor = a.BoughtFor
                })
                .ToListAsync();
        }


        // Binder viewing
        public async Task<CollectionBinderDto> GetBinderDetails(Guid binderId)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Binders
                .Where(b => b.Id == binderId)
                .Select(b => new CollectionBinderDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    Quantity = b.Cards.Count,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .SingleAsync();
        }

        public async Task<List<CollectionAllocationDto>> GetBinderAllocationsAsync(Guid binderId)
        {
            using var db = await _factory.CreateDbContextAsync();

            var binder = await GetBinderDetails(binderId);

            return await db.Allocations
                .Where(a => a.BinderId == binderId)
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    BinderId = a.BinderId,
                    BinderName = binder.Name,
                    OracleId = a.CollectionCard.OracleId,
                    VariantId = a.CollectionCard.VariantId,
                    Finish = a.Finish,
                    Language = a.Language,
                    Condition = a.Condition,
                    IsAltered = a.IsAltered,
                    IsSigned = a.IsSigned,
                    IsStamped = a.IsStamped,
                    BoughtFor = a.BoughtFor
                })
                .ToListAsync();
        }


        // Deck viewing
        public async Task<DeckDto> GetDeckDetails(Guid deckId)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Decks
                .Where(d => d.Id == deckId)
                .Select(d => new DeckDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Type = d.Type,
                    Description = d.Description,
                })
                .SingleAsync();
        }

        public async Task<List<CollectionAllocationDto>> GetDeckAllocationsAsync(Guid deckId)
        {
            using var db = await _factory.CreateDbContextAsync();

            var activeSnapshotId = await db.Snapshots
                .Where(s => (s.DeckId == deckId) && s.IsActive)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            return await db.Allocations
                .Where(a => a.SnapshotId == activeSnapshotId)
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    OracleId = a.CollectionCard.OracleId,
                    VariantId = a.CollectionCard.VariantId,
                    Finish = a.Finish,
                    Language = a.Language,
                    Condition = a.Condition,
                    IsAltered = a.IsAltered,
                    IsSigned = a.IsSigned
                })
                .ToListAsync();
        }


        // Card viewer
        public async Task<CollectionAllocationDto> GetSingleAllocationAsync(Guid allocationId)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Allocations
                .Where(a => a.Id == allocationId)
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    OracleId = a.CollectionCard.OracleId,
                    VariantId = a.CollectionCard.VariantId,
                    Finish = a.Finish,
                    Language = a.Language,
                    Condition = a.Condition,
                    IsAltered = a.IsAltered,
                    IsSigned = a.IsSigned,
                    IsStamped = a.IsStamped,
                    BoughtFor = a.BoughtFor,
                    AddedAt = a.AddedAt,
                    AllocatedAt = a.AllocatedAt
                })
                .SingleAsync();
        }

        public async Task UpdateVariantGroupAsync(CollectionVariantGroupDto group)
        {
            using var db = await _factory.CreateDbContextAsync();

            var allocIds = group.Allocations
                .Select(a => a.Id)
                .ToList();

            var allocs = await db.Allocations
                .AsNoTracking()
                .Where(a => allocIds.Contains(a.Id))
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    OracleId = a.CollectionCard.OracleId,
                    VariantId = a.CollectionCard.VariantId,
                    BinderId = a.BinderId,
                    BinderName = "nyi",
                    Finish = a.Finish,
                    Language = a.Language,
                    Condition = a.Condition,
                    IsAltered = a.IsAltered,
                    IsSigned = a.IsSigned,
                    IsStamped = a.IsStamped,
                    BoughtFor = a.BoughtFor
                })
                .ToListAsync();

            group.Allocations.Clear();
            group.Allocations.AddRange(allocs);

            group.Quantity = allocs.Count;
        }

        public async Task<EditableAllocationDto> GetEditableAllocationAsync(Guid allocationId)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Allocations
                .Where(a => a.Id == allocationId)
                .Select(a => new EditableAllocationDto
                {
                    AllocationId = a.Id,
                    VariantId = a.CollectionCard.VariantId,
                    Finish = a.Finish,
                    Language = a.Language,
                    Condition = a.Condition,
                    IsAltered = a.IsAltered,
                    IsSigned = a.IsSigned,
                    IsStamped = a.IsStamped,
                    BoughtFor = a.BoughtFor,
                    BinderId = a.BinderId,
                    SnapshotId = a.SnapshotId
                })
                .SingleAsync();
        }

        public async Task UpdateAllocationAsync(EditableAllocationDto editDto)
        {
            using var db = await _factory.CreateDbContextAsync();

            var alloc = await db.Allocations.FindAsync(editDto.AllocationId);

            if (alloc != null)
            {
                alloc.Finish = editDto.Finish;
                alloc.Language = editDto.Language;
                alloc.Condition = editDto.Condition;
                alloc.IsAltered = editDto.IsAltered;
                alloc.IsSigned = editDto.IsSigned;
                alloc.IsStamped = editDto.IsStamped;
                alloc.BoughtFor = editDto.BoughtFor;

                alloc.BinderId = null;
                alloc.SnapshotId = null;

                if (editDto.BinderId.HasValue)
                {
                    alloc.BinderId = editDto.BinderId;
                    alloc.AllocationIndex = AllocationIndex.Binder;
                }
                else if (editDto.SnapshotId.HasValue)
                {
                    alloc.SnapshotId = editDto.SnapshotId;
                    alloc.AllocationIndex = AllocationIndex.Deck;
                }
                else
                {
                    alloc.AllocationIndex = AllocationIndex.Unassigned;
                }

                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAllocationAsync(Guid allocationId)
        {
            using var db = await _factory.CreateDbContextAsync();

            var alloc = await db.Allocations
                .Include(a => a.CollectionCard)
                .SingleAsync(a => a.Id == allocationId);

            if (alloc != null)
            {
                db.Allocations.Remove(alloc);

                var inUse = await db.Allocations
                    .AnyAsync(a => 
                        a.CollectionCardId == alloc.CollectionCardId
                        && a.Id != allocationId);

                if (!inUse)
                    db.CollectionCards.Remove(alloc.CollectionCard);

                await db.SaveChangesAsync();
            }
        }


        public async Task<List<Guid>> GetAllDistinctVariantsAsync()
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.CollectionCards
                .Select(c => c.VariantId)
                .Distinct()
                .ToListAsync();
        }

    }
}