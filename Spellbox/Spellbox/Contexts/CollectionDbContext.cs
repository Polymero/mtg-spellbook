using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Spellbox.Model;


namespace Spellbox.Contexts
{

    public class CollectionDbContext : DbContext
    {
        public DbSet<CollectionAllocation> Allocations { get; set; }
        public DbSet<CollectionBinder> Binders { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<DeckSnapshot> Snapshots { get; set; }
        public DbSet<DeckZone> Zones { get; set; }
        public DbSet<DeckCard> DeckCards { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserPricingSettings> UserPricingSettings { get; set; }

        public CollectionDbContext(DbContextOptions<CollectionDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.ApplyConfiguration(new CollectionAllocationConfiguration());
            model.ApplyConfiguration(new CollectionBinderConfiguration());
            model.ApplyConfiguration(new DeckConfiguration());
            model.ApplyConfiguration(new DeckSnapshotConfiguration());
            model.ApplyConfiguration(new DeckZoneConfiguration());
            model.ApplyConfiguration(new DeckCardConfiguration());

            model.ApplyConfiguration(new UserProfileConfiguration());
            model.ApplyConfiguration(new UserPricingSettingsConfiguration());
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

            entity.Property(e => e.IsStamped)
                  .IsRequired();

            entity.Property(e => e.IsMisprint)
                  .IsRequired();

            entity.Property(e => e.BoughtFor)
                  .HasPrecision(10, 2);

            entity.HasIndex(e => e.OracleId);
            entity.HasIndex(e => e.VariantId);
            entity.HasIndex(e => e.BinderId);
            entity.HasIndex(e => e.ZoneId);

            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_CollectionAllocation_AllocationIndex",
                    @"
                    (
                        (AllocationIndex = 0 AND BinderId IS NULL AND ZoneId IS NULL) OR
                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND ZoneId IS NULL) OR
                        (AllocationIndex = 2 AND BinderId IS NULL AND ZoneId IS NOT NULL)
                    )
                    "
                );
            });
        }
    }

    public class CollectionBinderConfiguration : IEntityTypeConfiguration<CollectionBinder>
    {
        public void Configure(EntityTypeBuilder<CollectionBinder> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                  .IsRequired();

            entity.Property(e => e.CoverImages)
                  .HasConversion(
                        e => JsonSerializer.Serialize(e, (JsonSerializerOptions?)null),
                        e => JsonSerializer.Deserialize<List<Guid>>(e, (JsonSerializerOptions?)null)!
                  )
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .IsRequired();

            entity.Property(e => e.UpdatedAt)
                  .IsRequired();

            entity.HasMany(e => e.Cards)
                  .WithOne(a => a.Binder)
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

            entity.Property(e => e.ColorIdentity)
                  .IsRequired();

            entity.Property(e => e.LegalityStatus)
                  .IsRequired();

            entity.Property(e => e.CoverImages)
                  .HasConversion(
                        e => JsonSerializer.Serialize(e, (JsonSerializerOptions?)null),
                        e => JsonSerializer.Deserialize<List<Guid>>(e, (JsonSerializerOptions?)null)!
                  )
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

            entity.Property(e => e.Name)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .IsRequired();

            entity.Property(e => e.UpdatedAt)
                  .IsRequired();

            entity.HasMany(e => e.Zones)
                  .WithOne(z => z.Snapshot)
                  .HasForeignKey(z => z.SnapshotId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Deck)
                  .WithMany(d => d.Snapshots)
                  .HasForeignKey(e => e.DeckId)
                  .OnDelete(DeleteBehavior.Cascade);
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

            entity.HasMany(e => e.Allocations)
                  .WithOne(a => a.Zone)
                  .HasForeignKey(a => a.ZoneId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Snapshot)
                  .WithMany(s => s.Zones)
                  .HasForeignKey(e => e.SnapshotId)
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

            entity.HasOne(e => e.Zone)
                  .WithMany(s => s.Cards)
                  .HasForeignKey(e => e.ZoneId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DisplayName)
                  .IsRequired()
                  .HasMaxLength(64);

            entity.HasIndex(e => e.DisplayName)
                  .IsUnique();
        }
    }

    public class UserPricingSettingsConfiguration : IEntityTypeConfiguration<UserPricingSettings>
    {
        public void Configure(EntityTypeBuilder<UserPricingSettings> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Marketplace)
                  .HasConversion<int>();

            entity.Property(e => e.NonFoilMetric)
                  .HasConversion<int>();
            
            entity.Property(e => e.FoilMetric)
                  .HasConversion<int>();
        }
    }
    
}