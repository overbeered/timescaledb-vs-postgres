using Dapper;
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

        public static Task FindsAndPauseCompressionPolicyJobAsync(this DbContext context,
            string hypertableName)
        {
            string query = $@"SELECT alter_job(job_id => 
                              (
                                  SELECT s.job_id 
                                  FROM timescaledb_information.jobs j 
                                  INNER JOIN timescaledb_information.job_stats s ON j.job_id = s.job_id 
                                  WHERE j.proc_name = 'policy_compression' 
                                  AND s.hypertable_name = '{hypertableName}' 
                              ), 
                              scheduled => false, next_start => 'infinity');";

            return context.Database.ExecuteSqlRawAsync(query);
        }

        public static Task DecompressChunkAsync(this TimescaleDbContext context,
            string chunkName)
        {
            string query = $"SELECT decompress_chunk(CONCAT('_timescaledb_internal.', @chunkName), if_compressed => true)::varchar;";

            return context.Connection.ExecuteAsync(query, new { chunkName });
        }

        public static Task FindsAndScheduleCompressionPolicyJobAsync(this TimescaleDbContext context,
           string hypertableName)
        {
            string query = $@"SELECT alter_job(job_id => 
                              (
                                  SELECT s.job_id 
                                  FROM timescaledb_information.jobs j 
                                  INNER JOIN timescaledb_information.job_stats s ON j.job_id = s.job_id 
                                  WHERE j.proc_name = 'policy_compression' 
                                  AND s.hypertable_name = '{hypertableName}' 
                              ), 
                              scheduled => true, next_start => 'now');";

            return context.Database.ExecuteSqlRawAsync(query);
        }
    }
}
