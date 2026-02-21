using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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


        // Adding Cards
        public async Task SubmitBatchAsync(
            IEnumerable<NewAllocationDto> submissionBatch,
            Guid? binderId,
            Guid? activeSnapshotId
        )
        {
            using var db = await _factory.CreateDbContextAsync();
            using var tx = await db.Database.BeginTransactionAsync();

            var allocationIndex = 
                binderId != null ? AllocationIndex.Binder :
                activeSnapshotId != null ? AllocationIndex.Deck :
                AllocationIndex.Unassigned;

            var groups = submissionBatch
                .ToLookup(s => Tuple.Create(s.OracleId, s.VariantId));

            foreach (var group in groups)
            {
                (var oracleId, var variantId) = group.Key;

                // Get existing collection card
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

                foreach (var newAlloc in group)
                {
                    db.Allocations.Add(new CollectionAllocation
                    {
                        Id = Guid.NewGuid(),
                        CollectionCardId = collectionCard.Id,
                        AllocationIndex = allocationIndex,
                        BinderId = binderId,
                        SnapshotId = activeSnapshotId,
                        Finish = newAlloc.Finish,
                        Language = newAlloc.Language,
                        Condition = newAlloc.Condition,
                        IsAltered = newAlloc.IsAltered,
                        IsSigned = newAlloc.IsSigned,
                        IsStamped = newAlloc.IsStamped,
                        BoughtFor = newAlloc.BoughtFor,
                        AddedAt = DateTime.UtcNow,
                        AllocatedAt = DateTime.UtcNow
                    });
                }

            }

            await db.SaveChangesAsync();
            await tx.CommitAsync();
        }


        // Collection Overview
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
                    CoverImage = b.CoverImage,
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


        // Unassigned Contents
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


        // Binder Contents
        public async Task<CollectionBinderDto> GetBinderDetails(Guid binderId)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Binders
                .AsNoTracking()
                .Where(b => b.Id == binderId)
                .Select(b => new CollectionBinderDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    CoverImage = b.CoverImage,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,

                    Quantity = b.Cards.Count
                })
                .SingleAsync();
        }

        public async Task<List<CollectionAllocationDto>> GetBinderAllocationsAsync(Guid binderId)
        {
            using var db = await _factory.CreateDbContextAsync();

            var binder = await GetBinderDetails(binderId);

            return await db.Allocations
                .AsNoTracking()
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


        // Deck Contents
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
                .AsNoTracking()
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


        // Collection Contents
        public async Task<List<CollectionAllocationDto>> GetAllAllocationsAsync()
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Allocations
                .AsNoTracking()
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    BinderId = a.Binder != null ? a.BinderId : null,
                    BinderName = a.Binder != null ? a.Binder.Name : null,
                    DeckId = a.DeckSnapshot != null ? a.DeckSnapshot.DeckId : null,
                    DeckName = a.DeckSnapshot != null ? a.DeckSnapshot.Deck.Name : null,
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


        // Allocation Editing
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

        public async Task<CollectionAllocationDto> UpdateAllocationAsync(EditableAllocationDto editDto)
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
            
            // await db.DisposeAsync();
            // using var db = await _factory.CreateDbContextAsync();

            return await db.Allocations
                .Where(a => a.Id == editDto.AllocationId)
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    BinderId = a.Binder != null ? a.BinderId : null,
                    BinderName = a.Binder != null ? a.Binder.Name : null,
                    DeckId = a.DeckSnapshot != null ? a.DeckSnapshot.DeckId : null,
                    DeckName = a.DeckSnapshot != null ? a.DeckSnapshot.Deck.Name : null,
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
                .SingleAsync();
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


        // Binder Editing
        public async Task<EditableBinderDto> GetEditableBinderAsync(Guid binderId)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Binders
                .Where(b => b.Id == binderId)
                .Select(b => new EditableBinderDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    CoverImage = b.CoverImage
                })
                .SingleAsync();
        }

        public async Task<CollectionBinderDto> UpdateBinderAsync(EditableBinderDto editDto)
        {
            using var db = await _factory.CreateDbContextAsync();

            var binder = await db.Binders
                .FindAsync(editDto.Id);

            if (binder is not null)
            {
                binder.Name = editDto.Name.Trim();
                binder.Description = editDto.Description?.Trim();
                binder.CoverImage = editDto.CoverImage;

                binder.UpdatedAt = DateTime.UtcNow;

                await db.SaveChangesAsync();
            }

            // Remake db?

            return await db.Binders
                .Where(b => b.Id == editDto.Id)
                .Select(b => new CollectionBinderDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    CoverImage = b.CoverImage,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,

                    Quantity = b.Cards.Count()
                })
                .SingleAsync();
        }

        public async Task DeleteBinderAsync(Guid binderId)
        {
            using var db = await _factory.CreateDbContextAsync();

            var binder = await db.Binders
                .FindAsync(binderId);

            if (binder is not null)
            {
                db.Binders.Remove(binder);

                var allocations = await db.Allocations
                .Where(a => a.BinderId == binderId)
                .ToListAsync();

                foreach (var alloc in allocations)
                {
                    alloc.AllocationIndex = AllocationIndex.Unassigned;
                    alloc.BinderId = null;
                    alloc.AllocatedAt = DateTime.UtcNow;
                }

            await db.SaveChangesAsync();
            }
        }



    }
}