using Demo.Database.Contexts.Shared;
using Demo.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Demo.Database.Contexts.TimescaleDB
{
#nullable disable
    public class TimescaleDbContext : DbContext
    {
        public DbSet<TimeEventData> TimeEventsData { get; set; }

        public DbConnection Connection => Database.GetDbConnection();

        public TimescaleDbContext(DbContextOptions<TimescaleDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("timescaledb");

            modelBuilder.ApplyConfiguration(new TimeEventDataConfiguration());
        }
    }
#nullable restore
}
