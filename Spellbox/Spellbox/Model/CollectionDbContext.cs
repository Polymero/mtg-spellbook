using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Spellbox.Model
{
    public class CollectionDbContext : DbContext
    {
        public DbSet<CollectionCard> CollectionCards { get; set; }
        public DbSet<CollectionAllocation> Allocations { get; set; }
        public DbSet<CollectionBinder> Binders { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<DeckSnapshot> Snapshots { get; set; }
        public DbSet<DeckZone> Zones { get; set; }
        public DbSet<DeckCard> DeckCards { get; set; }

        public CollectionDbContext(DbContextOptions<CollectionDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.ApplyConfiguration(new CollectionCardConfiguration());
            model.ApplyConfiguration(new CollectionAllocationConfiguration());
            model.ApplyConfiguration(new CollectionBinderConfiguration());
            model.ApplyConfiguration(new DeckConfiguration());
            model.ApplyConfiguration(new DeckSnapshotConfiguration());
            model.ApplyConfiguration(new DeckZoneConfiguration());
            model.ApplyConfiguration(new DeckCardConfiguration());
        }
    }

    public class CollectionCardConfiguration : IEntityTypeConfiguration<CollectionCard>
    {
        public void Configure(EntityTypeBuilder<CollectionCard> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OracleId)
                  .IsRequired();

            entity.Property(e => e.VariantId)
                  .IsRequired();

            entity.Property(e => e.Quantity)
                  .IsRequired();

            entity.HasIndex(e => new { e.OracleId, e.VariantId })
                  .IsUnique();

            entity.HasMany(e => e.Allocations)
                  .WithOne(a => a.CollectionCard)
                  .HasForeignKey(a => a.CollectionCardId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class CollectionAllocationConfiguration : IEntityTypeConfiguration<CollectionAllocation>
    {
        public void Configure(EntityTypeBuilder<CollectionAllocation> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AllocationIndex)
                  .HasConversion<int>()
                  .IsRequired();

            entity.Property(e => e.AllocatedAt)
                  .IsRequired();

            entity.Property(e => e.Finish)
                  .HasConversion<int>()
                  .IsRequired();

            entity.Property(e => e.Language)
                  .HasConversion<int>()
                  .IsRequired();

            entity.Property(e => e.Condition)
                  .HasConversion<int>()
                  .IsRequired();

            entity.Property(e => e.IsAltered)
                  .IsRequired();

            entity.Property(e => e.IsSigned)
                  .IsRequired();

            entity.HasIndex(e => e.CollectionCardId);
            entity.HasIndex(e => e.BinderId);
            entity.HasIndex(e => e.SnapshotId);

            entity.HasCheckConstraint(
                "CK_CollectionAllocation_AllocationIndex",
                @"
                (
                    (AllocationIndex = 0 AND BinderId IS NULL AND SnapshotId IS NULL) OR
                    (AllocationIndex = 1 AND BinderId IS NOT NULL AND SnapshotId IS NULL) OR
                    (AllocationIndex = 2 AND BinderId IS NULL AND SnapshotId IS NOT NULL)
                )
                "
            );
        }
    }

    public class CollectionBinderConfiguration : IEntityTypeConfiguration<CollectionBinder>
    {
        public void Configure(EntityTypeBuilder<CollectionBinder> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .IsRequired();

            entity.Property(e => e.UpdatedAt)
                  .IsRequired();

            entity.HasMany<CollectionAllocation>()
                  .WithOne()
                  .HasForeignKey(a => a.BinderId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class DeckConfiguration : IEntityTypeConfiguration<Deck>
    {
        public void Configure(EntityTypeBuilder<Deck> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                  .IsRequired();

            entity.Property(e => e.Type)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .IsRequired();

            entity.Property(e => e.UpdatedAt)
                  .IsRequired();

            entity.HasMany(e => e.Snapshots)
                  .WithOne(s => s.Deck)
                  .HasForeignKey(s => s.DeckId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class DeckSnapshotConfiguration : IEntityTypeConfiguration<DeckSnapshot>
    {
        public void Configure(EntityTypeBuilder<DeckSnapshot> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.IsActive)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .IsRequired();

            entity.Property(e => e.UpdatedAt)
                  .IsRequired();

            entity.HasIndex(e => e.DeckId)
                  .HasFilter("IsActive = 1")
                  .IsUnique();

            entity.HasMany(e => e.Zones)
                  .WithOne(z => z.Snapshot)
                  .HasForeignKey(z => z.SnapshotId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany<CollectionAllocation>()
                  .WithOne()
                  .HasForeignKey(a => a.SnapshotId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class DeckZoneConfiguration : IEntityTypeConfiguration<DeckZone>
    {
        public void Configure(EntityTypeBuilder<DeckZone> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ZoneType)
                  .HasConversion<int>()
                  .IsRequired();

            entity.HasIndex(e => new { e.SnapshotId, e.ZoneType })
                  .IsUnique();

            entity.HasMany(e => e.Cards)
                  .WithOne(c => c.Zone)
                  .HasForeignKey(c => c.ZoneId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class DeckCardConfiguration : IEntityTypeConfiguration<DeckCard>
    {
        public void Configure(EntityTypeBuilder<DeckCard> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OracleId)
                  .IsRequired();

            entity.Property(e => e.VariantId)
                  .IsRequired();

            entity.Property(e => e.Quantity)
                  .IsRequired();

            entity.HasIndex(e => new
            {
                e.ZoneId,
                e.OracleId,
                e.VariantId
            })
            .IsUnique();
        }
    }
}