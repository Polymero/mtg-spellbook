using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spellbox.Model;
using Spellbox.Services;


namespace Spellbox.Contexts
{

    public class CardMarketDbContext : DbContext
    {
        public DbSet<CardMarketPriceCache> PriceCaches { get; set; }
        public DbSet<CardMarketProductId> AddedProductIds { get; set; }
        public DbSet<PricingSyncState> SyncStates { get; set; }

        public CardMarketDbContext(DbContextOptions<CardMarketDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.ApplyConfiguration(new CardMarketPriceCacheConfiguration());
            model.ApplyConfiguration(new CardMarketProductIdConfiguration());
            model.ApplyConfiguration(new SyncStatesConfiguration());
        }
    }

    public class CardMarketPriceCacheConfiguration : IEntityTypeConfiguration<CardMarketPriceCache>
    {
        public void Configure(EntityTypeBuilder<CardMarketPriceCache> entity)
        {

            entity.HasKey(e => e.ProductId);

            entity.Property(e => e.Low)
                  .HasColumnType("decimal(10,2)");

            entity.Property(e => e.Avg)
                  .HasColumnType("decimal(10,2)");

            entity.Property(e => e.Trend)
                  .HasColumnType("decimal(10,2)");

            entity.Property(e => e.FoilLow)
                  .HasColumnType("decimal(10,2)");

            entity.Property(e => e.FoilAvg)
                  .HasColumnType("decimal(10,2)");

            entity.Property(e => e.FoilTrend)
                  .HasColumnType("decimal(10,2)");

        }
    }

    public class CardMarketProductIdConfiguration : IEntityTypeConfiguration<CardMarketProductId>
    {
        public void Configure(EntityTypeBuilder<CardMarketProductId> entity)
        {
            
            entity.HasKey(e => e.VariantId);

            entity.Property(e => e.ProductId)
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