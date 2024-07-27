using Microsoft.EntityFrameworkCore;

namespace Spellbox.Model
{
    public class OracleDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public OracleDbContext(IConfiguration configuration) 
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(Configuration.GetConnectionString("OracleDatabaseConnectionString"));
        }

        public DbSet<OracleCard> OracleCards { get; set; }
        public DbSet<CardVariant> CardVariants { get; set; }
    }
}