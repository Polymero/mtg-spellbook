using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spellbox.Model;
using Spellbox.Services;

namespace Spellbox.Contexts
{
    public class CardMarketDbContext : DbContext
    {
        // public DbSet<CardMarketProductMapping> ProductMappings { get; set; }
        public DbSet<CardMarketPriceCache> PriceCaches { get; set; }
        public DbSet<PricingSyncState> SyncStates { get; set; }


        public CardMarketDbContext(DbContextOptions<CardMarketDbContext> options) : base(options)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder model)
        {
            // model.ApplyConfiguration(new CardMarketProductMappingConfiguration());
            model.ApplyConfiguration(new CardMarketPriceCacheConfiguration());
            model.ApplyConfiguration(new SyncStatesConfiguration());
        }
    }


    // public class CardMarketProductMappingConfiguration : IEntityTypeConfiguration<CardMarketProductMapping>
    // {
    //     public void Configure(EntityTypeBuilder<CardMarketProductMapping> entity)
    //     {
    //         entity.HasKey(e => e.Id);

    //         entity.Property(e => e.CardVariantId)
    //               .IsRequired();

    //         entity.Property(e => e.ProductId)
    //               .IsRequired();
            
    //         entity.Property(e => e.SetCode)
    //               .IsRequired();

    //         entity.Property(e => e.CollNum)
    //               .IsRequired();

    //         entity.Property(e => e.Name)
    //               .IsRequired();

    //         entity.Property(e => e.Finish)
    //               .IsRequired();

    //         entity.Property(e => e.Language)
    //               .IsRequired();

    //         entity.Property(e => e.CreatedAt)
    //               .IsRequired();

    //         entity.HasIndex(e => new
    //         {
    //             e.CardVariantId,
    //             e.Finish,
    //             e.Language
    //         }).IsUnique();

    //         entity.HasIndex(e => e.ProductId)
    //               .IsUnique();
    //     }
    // }


    public class CardMarketPriceCacheConfiguration : IEntityTypeConfiguration<CardMarketPriceCache>
    {
        public void Configure(EntityTypeBuilder<CardMarketPriceCache> entity)
        {

            entity.HasKey(e => e.ProductId);

            entity.Property(e => e.PriceLow)
                  .HasColumnType("decimal(10,2)");

            entity.Property(e => e.PriceTrend)
                  .HasColumnType("decimal(10,2)");

            entity.Property(e => e.PriceAverage)
                  .HasColumnType("decimal(10,2)");

            entity.Property(e => e.PriceFoilLow)
                  .HasColumnType("decimal(10,2)");

            entity.Property(e => e.PriceFoilTrend)
                  .HasColumnType("decimal(10,2)");

            entity.Property(e => e.UpdatedAt)
                  .IsRequired();
        }
    }


    public class SyncStatesConfiguration : IEntityTypeConfiguration<PricingSyncState>
    {
        public void Configure(EntityTypeBuilder<PricingSyncState> entity)
        {
            entity.HasKey(e => e.Key);
        }
    }

}