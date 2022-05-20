using Demo.Database.Contexts.Postgres;
using Demo.Database.Contexts.TimescaleDB;
using Microsoft.EntityFrameworkCore;

namespace Demo.Server.Extensions
{
    public static class MigrationManager
    {
        public static IHost MigrateTimescaleDatabase(this IHost webHost)
        {
            using var serviceScope = webHost.Services.CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<TimescaleDbContext>()!;

            context.Database.Migrate();
            return webHost;
        }

        public static IHost MigratePostgresDatabase(this IHost webHost)
        {
            using var serviceScope = webHost.Services.CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<PostgresDbContext>()!;

            context.Database.Migrate();
            return webHost;
        }
    }
}
