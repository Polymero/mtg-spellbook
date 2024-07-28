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
            SQLitePCL.Batteries.Init();
        }

        public DbSet<OracleCard> OracleCards { get; set; }
        public DbSet<CardFace> CardFaces { get; set; }
        public DbSet<CardLegality> CardLegalities {get; set; }
        public DbSet<CardVariant> CardVariants { get; set; }
    }
}