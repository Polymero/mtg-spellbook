using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Spellbox.Model
{
    public class OracleDbContext : DbContext
    {
        public DbSet<OracleCard> OracleCards => Set<OracleCard>();
        public DbSet<CFace> CardFaces => Set<CFace>();
        public DbSet<CVariant> CardVariants => Set<CVariant>();


        public OracleDbContext(DbContextOptions<OracleDbContext> options) : base(options)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<OracleCard>(entity =>
            {
                entity.HasKey(e => e.OracleId);
            });

            model.Entity<CVariant>(entity =>
            {
                entity.HasKey(e => e.ScryfallId);

                entity.HasIndex(e => e.SearchName);

                entity.HasIndex(e => new { e.SetCode, e.CollNum })
                      .IsUnique();

                entity.HasOne(e => e.OracleCard)
                      .WithMany(c => c.Variants)
                      .HasForeignKey(e => e.OracleCardId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            model.Entity<CFace>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.OracleCard)
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