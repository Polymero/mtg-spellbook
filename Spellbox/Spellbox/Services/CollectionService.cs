using System.Reflection;
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
            Guid? zoneId
        )
        {
            using var db = await _factory.CreateDbContextAsync();
            using var tx = await db.Database.BeginTransactionAsync();

            var allocationIndex = 
                binderId != null ? AllocationIndex.Binder :
                zoneId != null ? AllocationIndex.Deck :
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
                        ZoneId = zoneId,
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
                    CoverImage = d.CoverImage,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,

                    ActiveSnapshotId = d.Snapshots
                        .First(s => s.IsActive)
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
                    CoverImage = d.CoverImage,
                    
                    ActiveSnapshotId = d.Snapshots
                        .Single(s => s.IsActive)
                        .Id,
                    ActiveMainboardId = d.Snapshots
                        .Single(s => s.IsActive)
                        .Zones
                        .Single(z => z.ZoneType == DeckZoneType.Mainboard)
                        .Id,

                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,

                    Quantity = d.Snapshots
                        .First(s => s.IsActive)
                        .Zones
                        .Sum(z => z.Allocations.Count) +
                        d.Snapshots
                            .First(s => s.IsActive)
                            .Zones
                            .Sum(z => z.Cards.Count)
                })
                .SingleAsync();
        }

        public async Task<Dictionary<DeckZoneType, List<CollectionAllocationDto>>> GetZoneAllocationsAsync(
            Guid snapshotId
        )
        {
            using var db = await _factory.CreateDbContextAsync();

            var zones = await db.Zones
                .AsNoTracking()
                .Where(z => z.SnapshotId == snapshotId)
                .Select(z => new
                {
                    z.ZoneType,
                    Allocations = z.Allocations
                        .Select(a => new CollectionAllocationDto
                        {
                            Id = a.Id,
                            ZoneId = a.ZoneId,
                            DeckName = a.Zone!.Snapshot.Deck.Name,
                            OracleId = a.CollectionCard.OracleId,
                            VariantId = a.CollectionCard.VariantId,
                            Finish = a.Finish,
                            Language = a.Language,
                            Condition = a.Condition,
                            IsAltered = a.IsAltered,
                            IsSigned = a.IsSigned,
                            IsStamped = a.IsStamped,
                            BoughtFor = a.BoughtFor
                        }).ToList()
                })
                .ToListAsync();

            return zones.ToDictionary(z => z.ZoneType, z => z.Allocations);
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
                    ZoneId = a.Zone != null ? a.ZoneId : null,
                    DeckName = a.Zone != null ? a.Zone.Snapshot.Deck.Name : null,
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
                    DeckId = a.DeckId,
                    ZoneId = a.ZoneId
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
                alloc.ZoneId = null;

                if (editDto.BinderId.HasValue)
                {
                    alloc.BinderId = editDto.BinderId;
                    alloc.AllocationIndex = AllocationIndex.Binder;
                }
                else if (editDto.ZoneId.HasValue)
                {
                    alloc.ZoneId = editDto.ZoneId;
                    alloc.AllocationIndex = AllocationIndex.Deck;
                }
                else
                {
                    alloc.AllocationIndex = AllocationIndex.Unassigned;
                }

                await db.SaveChangesAsync();
            }
            
            // Remake db?

            return await db.Allocations
                .Where(a => a.Id == editDto.AllocationId)
                .Select(a => new CollectionAllocationDto
                {
                    Id = a.Id,
                    BinderId = a.Binder != null ? a.BinderId : null,
                    BinderName = a.Binder != null ? a.Binder.Name : null,
                    ZoneId = a.Zone != null ? a.ZoneId : null,
                    DeckName = a.Zone != null ? a.Zone.Snapshot.Deck.Name : null,
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

            if (alloc is null)
                return;

            db.Allocations.Remove(alloc);

            var inUse = await db.Allocations
                .AnyAsync(a => 
                    a.CollectionCardId == alloc.CollectionCardId
                    && a.Id != allocationId);

            if (!inUse)
                db.CollectionCards.Remove(alloc.CollectionCard);

            await db.SaveChangesAsync();
        }


        // Binder Editing
        public async Task<CollectionBinderDto> AddBinderAsync(EditableBinderDto binder)
        {
            using var db = await _factory.CreateDbContextAsync();

            var binderId = Guid.NewGuid();

            db.Binders.Add(new CollectionBinder
            {
               Id = binderId,
               Name = binder.Name.Trim(),
               Description = String.IsNullOrWhiteSpace(binder.Description) ? null : binder.Description.Trim(),
               CoverImage = binder.CoverImage,

               CreatedAt = DateTime.UtcNow,
               UpdatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();

            return await GetBinderDetails(binderId);
        }

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

            if (binder is null)
                return new CollectionBinderDto();

            binder.Name = editDto.Name.Trim();
            binder.Description = editDto.Description?.Trim();
            binder.CoverImage = editDto.CoverImage;

            binder.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return await GetBinderDetails(binder.Id);
        }

        public async Task DeleteBinderAsync(Guid binderId)
        {
            using var db = await _factory.CreateDbContextAsync();

            var binder = await db.Binders
                .FindAsync(binderId);

            if (binder is null)
                return;

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

        public async Task<DeckDto> TransformBinderIntoDeckAsync(EditableBinderDto binderDto)
        {
            using var db = await _factory.CreateDbContextAsync();

            var deck = await AddDeckAsync(new EditableDeckDto
            {
                Id = binderDto.Id,
                Name = binderDto.Name,
                Type = DeckType.Unassigned,
                Description = binderDto.Description,
                CoverImage = binderDto.CoverImage
            });

            var allocations = await db.Allocations
                .Where(a => a.BinderId == binderDto.Id)
                .ToListAsync();

            foreach (var alloc in allocations)
            {
                alloc.AllocationIndex = AllocationIndex.Deck;
                alloc.BinderId = null;
                alloc.ZoneId = deck.ActiveMainboardId;
                alloc.AllocatedAt = DateTime.UtcNow;
            }

            await db.SaveChangesAsync();

            await DeleteBinderAsync(binderDto.Id);

            return await GetDeckDetails(deck.Id);
        }


        // Deck Editing
        public async Task<DeckDto> AddDeckAsync(EditableDeckDto deck)
        {
            using var db = await _factory.CreateDbContextAsync();

            var newDeck = new Deck
            {
                Id = Guid.NewGuid(),
                Name = deck.Name,
                Type = deck.Type,
                Description = String.IsNullOrWhiteSpace(deck.Description) ? null : deck.Description.Trim(),
                CoverImage = deck.CoverImage,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var newSnapshot = new DeckSnapshot
            {
                Id = Guid.NewGuid(),
                DeckId = newDeck.Id,
                IsActive = true,
                Name = "First",
                Description = null,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var newZones = from zoneType in Enum.GetValues<DeckZoneType>() 
                select new DeckZone
                {
                    Id = Guid.NewGuid(),
                    SnapshotId = newSnapshot.Id,
                    ZoneType = zoneType
                };

            db.Decks.Add(newDeck);
            db.Snapshots.Add(newSnapshot);
            db.Zones.AddRange(newZones);

            await db.SaveChangesAsync();

            return await GetDeckDetails(newDeck.Id);
        }

        public async Task<EditableDeckDto> GetEditableDeckAsync(Guid deckId)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Decks
                .Where(d => d.Id == deckId)
                .Select(d => new EditableDeckDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Type = d.Type,
                    Description = d.Description,
                    CoverImage = d.CoverImage
                })
                .SingleAsync();
        }

        public async Task<DeckDto> UpdateDeckAsync(EditableDeckDto editDto)
        {
            using var db = await _factory.CreateDbContextAsync();

            var deck = await db.Decks
                .FindAsync(editDto.Id);

            if (deck is null)
                return new DeckDto();

            deck.Name = editDto.Name.Trim();
            deck.Type = editDto.Type;
            deck.Description = editDto.Description?.Trim();
            deck.CoverImage = editDto.CoverImage;

            deck.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return await GetDeckDetails(deck.Id);
        }

        public async Task DeleteDeckAsync(Guid deckId)
        {
            using var db = await _factory.CreateDbContextAsync();

            var deck = await db.Decks
                .FindAsync(deckId);

            if (deck is null)
                return;

            var snapshots = await db.Snapshots
                .Where(s => s.DeckId == deckId)
                .ToListAsync();

            var snapshotIds = snapshots
                .Select(s => s.Id);

            var zones = await db.Zones
                .Where(z => snapshotIds.Contains(z.SnapshotId))
                .ToListAsync();

            var zoneIds = zones
                .Select(z => z.Id);

            // Remove related entities
            db.Decks.Remove(deck);
            db.Snapshots.RemoveRange(snapshots);
            db.Zones.RemoveRange(zones);

            // De-allocate current cards
            var allocations = await db.Allocations
                .Where(a => a.ZoneId.HasValue && zoneIds.Contains(a.ZoneId.Value))
                .ToListAsync();

            foreach (var alloc in allocations)
            {
                alloc.AllocationIndex = AllocationIndex.Unassigned;
                alloc.DeckId = null;
                alloc.ZoneId = null;
                alloc.AllocatedAt = DateTime.UtcNow;
            }

            await db.SaveChangesAsync();
        }

        public async Task<CollectionBinderDto> TransformDeckIntoBinderAsync(EditableDeckDto deckDto)
        {
            using var db = await _factory.CreateDbContextAsync();

            var binder = await AddBinderAsync(new EditableBinderDto
            {
                Id = deckDto.Id,
                Name = deckDto.Name,
                Description = deckDto.Description,
                CoverImage = deckDto.CoverImage
            });

            var snapshots = await db.Snapshots
                .Where(s => s.DeckId == deckDto.Id)
                .ToListAsync();

            var snapshotIds = snapshots
                .Select(s => s.Id);

            var zones = await db.Zones
                .Where(z => snapshotIds.Contains(z.SnapshotId))
                .ToListAsync();

            var zoneIds = zones
                .Select(z => z.Id);

            var allocations = await db.Allocations
                .Where(a => a.ZoneId.HasValue && zoneIds.Contains(a.ZoneId.Value))
                .ToListAsync();

            foreach (var alloc in allocations)
            {
                alloc.AllocationIndex = AllocationIndex.Binder;
                alloc.ZoneId = null;
                alloc.BinderId = binder.Id;
                alloc.AllocatedAt = DateTime.UtcNow;
            }

            await db.SaveChangesAsync();

            await DeleteDeckAsync(deckDto.Id);

            return await GetBinderDetails(binder.Id);
        }

    }
}