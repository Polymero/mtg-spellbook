using Microsoft.EntityFrameworkCore;
using Spellbox.Model;

namespace Spellbox.Contexts
{
    public class OracleDbContext : DbContext
    {
        public DbSet<CardOracle> Oracles => Set<CardOracle>();
        public DbSet<CardFace> Faces => Set<CardFace>();
        public DbSet<CardVariant> Variants => Set<CardVariant>();


        public OracleDbContext(DbContextOptions<OracleDbContext> options) : base(options)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<CardOracle>(entity =>
            {
                entity.HasKey(e => e.OracleId);
            });

            model.Entity<CardVariant>(entity =>
            {
                entity.HasKey(e => e.ScryfallId);

                entity.HasIndex(e => e.SearchName);

                entity.HasIndex(e => new { e.SetCode, e.CollNum })
                      .IsUnique();

                entity.HasIndex(e => e.CardMarketProductId);

                entity.HasOne(e => e.Oracle)
                      .WithMany(c => c.Variants)
                      .HasForeignKey(e => e.OracleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            model.Entity<CardFace>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Oracle)
                      .WithMany(c => c.Faces)
                      .HasForeignKey(e => e.OracleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.OracleId)
                      .IsRequired();

                entity.HasIndex(e => new { e.OracleId, e.Order })
                      .IsUnique();
            });
        }   
    }
}