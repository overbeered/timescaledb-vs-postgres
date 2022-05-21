using Microsoft.EntityFrameworkCore;

namespace Demo.Database.Contexts.Postgres.Extensions
{
    public static class PostgresDbContextExtensions
    {
        public static Task TruncateTableAsync(this PostgresDbContext context,
            string hypertableName)
        {
            string query = $"TRUNCATE TABLE {hypertableName}";

            return context.Database.ExecuteSqlRawAsync(query);
        }
    }
}
