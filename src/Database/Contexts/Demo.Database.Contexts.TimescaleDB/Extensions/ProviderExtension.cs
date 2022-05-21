using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Database.Contexts.TimescaleDB.Extensions
{
    public static class ProviderExtension
    {
        public static void AddTimescaleDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var connectionString = configuration.GetConnectionString("TimescaleDbConnection");

            services.AddDbContext<TimescaleDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddSingleton<TimeEventDataHypertableSharedResource>();
        }
    }
}
