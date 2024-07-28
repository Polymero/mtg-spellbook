using Microsoft.EntityFrameworkCore;

namespace Spellbox.Model
{
    public class CollectionDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public CollectionDbContext(IConfiguration configuration) 
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(Configuration.GetConnectionString("CollectionDatabaseConnectionString"));
            SQLitePCL.Batteries.Init();

        }

        public DbSet<CollectionBinder> Binders { get; set; }
        public DbSet<CollectionDeck> Decks { get; set; }
    }
}