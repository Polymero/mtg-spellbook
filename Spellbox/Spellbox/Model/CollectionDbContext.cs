using Microsoft.EntityFrameworkCore;

namespace Spellbox.Model
{
    public class CollectionDbContext : DbContext
    {
        public DbSet<CollectionCard> Cards { get; set; }
        public DbSet<CollectionBinder> Binders { get; set; }
        public DbSet<CollectionDeck> Decks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=Data/Collection.db3");
            SQLitePCL.Batteries.Init();
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<CollectionCard>(entity =>
            {
                
            });

            model.Entity<CollectionBinder>(entity =>
            {
                
            });

            model.Entity<CollectionDeck>(entity =>
            {
                
            });
        }
    }
}