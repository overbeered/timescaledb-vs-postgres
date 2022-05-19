using Demo.Database.Contexts.Shared;
using Demo.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Demo.Database.Contexts.Postgres
{
#nullable disable
    public class PostgresDbContext : DbContext
    {
        public DbSet<TimeEventData> TimeEventsData { get; set; }

        public DbConnection Connection => Database.GetDbConnection();

        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TimeEventDataConfiguration());
        }
    }
#nullable restore
}
