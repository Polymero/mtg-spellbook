using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Spellbox.Model;
using Spellbox.Services;


namespace Spellbox.Contexts
{

    public class OracleDbContext : DbContext
    {
        public DbSet<CardOracle> Oracles { get; set; }
        public DbSet<CardFace> Faces { get; set; }
        public DbSet<CardVariant> Variants { get; set; }
        public DbSet<ScryfallSyncState> SyncStates { get; set; }
        public DbSet<Symbol> Symbols { get; set; }

        public OracleDbContext(DbContextOptions<OracleDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.ApplyConfiguration(new CardOracleConfiguration());
            model.ApplyConfiguration(new CardFaceConfiguration());
            model.ApplyConfiguration(new CardVariantConfiguration());
            model.ApplyConfiguration(new ScryfallSyncStatesConfiguration());
            model.ApplyConfiguration(new SymbolConfiguration());
        }
    }

    public class CardOracleConfiguration : IEntityTypeConfiguration<CardOracle>
    {
        public void Configure(EntityTypeBuilder<CardOracle> entity)
        {
            entity.HasKey(e => e.OracleId);

            entity.Property(e => e.Name)
                  .IsRequired();

            entity.Property(e => e.TypeLine)
                  .IsRequired();

            entity.Property(e => e.Keywords)
                  .HasConversion(
                        e => JsonSerializer.Serialize(e, (JsonSerializerOptions?)null),
                        e => JsonSerializer.Deserialize<List<string>>(e, (JsonSerializerOptions?)null)!
                  )
                  .IsRequired();

            entity.Property(e => e.CMC)
                  .IsRequired();

            entity.Property(e => e.ColorIdentity)
                  .HasConversion(
                        e => JsonSerializer.Serialize(e, (JsonSerializerOptions?)null),
                        e => JsonSerializer.Deserialize<List<string>>(e, (JsonSerializerOptions?)null)!
                  )
                  .IsRequired();

            entity.HasIndex(e => e.Name);

            entity.HasMany(e => e.Faces)
                  .WithOne(f => f.Oracle)
                  .HasForeignKey(f => f.OracleId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Variants)
                  .WithOne(v => v.Oracle)
                  .HasForeignKey(v => v.OracleId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class CardFaceConfiguration : IEntityTypeConfiguration<CardFace>
    {
        public void Configure(EntityTypeBuilder<CardFace> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Order)
                  .IsRequired();

            entity.Property(e => e.Name)
                  .IsRequired();

            entity.Property(e => e.TypeLine)
                  .IsRequired();

            entity.HasIndex(e => new { e.OracleId, e.Order})
                  .IsUnique();

            entity.HasIndex(x => x.OracleId);

            entity.HasOne(e => e.Oracle)
                  .WithMany(o => o.Faces)
                  .HasForeignKey(e => e.OracleId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class CardVariantConfiguration : IEntityTypeConfiguration<CardVariant>
    {
        public void Configure(EntityTypeBuilder<CardVariant> entity)
        {
            entity.HasKey(e => e.ScryfallId);

            entity.Property(e => e.SearchName)
                  .IsRequired();

            entity.Property(e => e.SetName)
                  .IsRequired();

            entity.Property(e => e.SetCode)
                  .IsRequired();

            entity.Property(e => e.CollNum)
                  .IsRequired();

            entity.Property(e => e.Finishes)
                  .HasConversion(
                        e => JsonSerializer.Serialize(e, (JsonSerializerOptions?)null),
                        e => JsonSerializer.Deserialize<List<string>>(e, (JsonSerializerOptions?)null)!
                  )
                  .IsRequired();

            entity.Property(e => e.Released)
                  .IsRequired();

            entity.Property(e => e.Rarity)
                  .IsRequired();

            entity.Property(e => e.Thumbs)
                  .HasConversion(
                        e => JsonSerializer.Serialize(e, (JsonSerializerOptions?)null),
                        e => JsonSerializer.Deserialize<List<string>>(e, (JsonSerializerOptions?)null)!
                  )
                  .IsRequired();

            entity.Property(e => e.Images)
                  .HasConversion(
                        e => JsonSerializer.Serialize(e, (JsonSerializerOptions?)null),
                        e => JsonSerializer.Deserialize<List<string>>(e, (JsonSerializerOptions?)null)!
                  )
                  .IsRequired();

            entity.Property(e => e.FlavorTexts)
                  .HasConversion(
                        e => JsonSerializer.Serialize(e, (JsonSerializerOptions?)null),
                        e => JsonSerializer.Deserialize<List<string>>(e, (JsonSerializerOptions?)null)!
                  );

            entity.HasIndex(e => e.SearchName);

            entity.HasIndex(e => new { e.SetCode, e.CollNum })
                  .IsUnique();

            entity.HasIndex(e => e.OracleId);

            entity.HasIndex(e => e.CardMarketProductId);

            entity.HasOne(e => e.Oracle)
                  .WithMany(o => o.Variants)
                  .HasForeignKey(e => e.OracleId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ScryfallSyncStatesConfiguration : IEntityTypeConfiguration<ScryfallSyncState>
    {
        public void Configure(EntityTypeBuilder<ScryfallSyncState> entity)
        {
            entity.HasKey(e => e.Key);
        }
    }

    public class SymbolConfiguration : IEntityTypeConfiguration<Symbol>
    {
        public void Configure(EntityTypeBuilder<Symbol> entity)
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Code);
        }
    }

}