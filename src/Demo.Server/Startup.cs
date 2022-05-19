using Demo.Database.Contexts.Postgres.Extensions;
using Demo.Database.Contexts.TimescaleDB.Extensions;

namespace Demo.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPostgresDbContext(Configuration);
            services.AddTimescaleDbContext(Configuration);
        }

        public void Configure(IApplicationBuilder _)
        {

        }
    }
}
