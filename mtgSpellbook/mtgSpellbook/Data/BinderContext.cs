using Microsoft.EntityFrameworkCore;

namespace mtgSpellbook.Client.Data
{
    public class BinderDataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public BinderDataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Configuration.GetConnectionString("BinderDB"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Binder>().ToTable("Binder");

            modelBuilder.Entity<Binder>().HasData(
                new Binder { Id = 1, Name = "Default Binder", Cards = [] }
            );
        }

        public DbSet<Binder> Binders { get; set; }
    }
}