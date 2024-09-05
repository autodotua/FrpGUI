using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FrpGUI.Models
{
    public class FrpDbContext : DbContext
    {
        static FrpDbContext()
        {
          
            connectionString = $"Data Source=logs.db";
        }

        private static readonly string connectionString;

        public FrpDbContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<LogEntity> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString);
        }


    }
}