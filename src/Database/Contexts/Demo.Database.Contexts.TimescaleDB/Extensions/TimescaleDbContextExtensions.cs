using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Demo.Database.Contexts.TimescaleDB.Extensions
{
    public static class TimescaleDbContextExtensions
    {
        public static Task DropChunksNewerThanAsync(this TimescaleDbContext context,
            string hypertableName,
            DateTimeOffset startPoint)
        {
            string query = $"SELECT drop_chunks('{hypertableName}', newer_than => @startPoint);";

            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@startPoint", NpgsqlTypes.NpgsqlDbType.TimestampTz) { Value = startPoint }
            };

            return context.Database.ExecuteSqlRawAsync(query, parameters);
        }

        public static Task DropChunksNewerThanNegativeInfinityAsync(this TimescaleDbContext context,
                    string hypertableName)
        {
            return DropChunksNewerThanAsync(context, hypertableName, DateTimeOffset.MinValue);
        }
    }
}
