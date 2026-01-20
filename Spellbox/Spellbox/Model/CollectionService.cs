using Microsoft.EntityFrameworkCore;

namespace Spellbox.Model
{
    public sealed class CollectionService
    {

        private readonly IDbContextFactory<CollectionDbContext> _factory;

        public CollectionService(IDbContextFactory<CollectionDbContext> factory)
        {
            _factory = factory;
        }


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
                    IsSigned = a.IsSigned
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
                    IsSigned = a.IsSigned
                })
                .SingleAsync();
        }

        // public async Task UpdateAllocationAsync(CardViewerSingleDto updatedDto)
        // {
        //     using var db = await _factory.CreateDbContextAsync();

        //     var allocation = await db.Allocations.FindAsync(updatedDto.AllocationId);

        //     if (allocation != null)
        //     {
        //         allocation.Finish = updatedDto.Finish;
        //         allocation.Language = updatedDto.Language;
        //         allocation.Condition = updatedDto.Condition;
        //         allocation.IsAltered = updatedDto.IsAltered;
        //         allocation.IsSigned = updatedDto.IsSigned;

        //         await db.SaveChangesAsync();
        //     }            
        // }

        public async Task DeleteAllocationAsync(Guid allocationId)
        {
            using var db = await _factory.CreateDbContextAsync();

            var allocation = await db.Allocations.FindAsync(allocationId);

            if (allocation != null)
            {
                db.Allocations.Remove(allocation);
                await db.SaveChangesAsync();
            }
        }

    }
}