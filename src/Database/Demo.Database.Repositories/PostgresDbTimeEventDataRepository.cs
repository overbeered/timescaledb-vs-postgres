using Dapper;
using Demo.Database.Contexts.Postgres;
using Demo.Database.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Demo.Database.Repositories
{
    public class PostgresDbTimeEventDataRepository : RepositoryBase<PostgresDbContext, TimeEventData>, ITimeEventDataRepository
    {
        public PostgresDbTimeEventDataRepository(PostgresDbContext context)
            : base(context)
        {
        }

        public Task AddTimeEventDataAsync(TimeEventData ted)
        {
            return TaskTransactionalInsertTimeEventDataAsync(ted.StudentId, ted.EventId, ted.Timestamp, ted.Payload);
        }

        private async Task TaskTransactionalInsertTimeEventDataAsync(Guid studentId,
            Guid eventId,
            DateTimeOffset timestamp,
            string payload)
        {
            string query = @"INSERT INTO timeeventsdata (studentid, eventid, timestamp, payload) 
                             VALUES (@studentid, @eventid, @timestamp, @payload::json);";

            var parameters = new DynamicParameters(new Dictionary<string, object?>
            {
                { "@studentid",     studentId   },
                { "@eventid",       eventId     },
                { "@timestamp",     timestamp   },
                { "@payload",       payload     },
            });

            await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            await using var transaction = await connection.BeginTransactionAsync();
            bool rollbackIsNeeded = true;

            try
            {
                await connection.ExecuteAsync(query, parameters);

                await transaction.CommitAsync();
                rollbackIsNeeded = false;
            }
            finally
            {
                if (rollbackIsNeeded)
                {
                    await transaction.RollbackAsync();
                }

                await connection.CloseAsync();
            }
        }

        public Task AddTimeEventsDataAsync(List<TimeEventData> teds)
        {
            return TransactionalBulkInsertTimeEventsDataAsync(teds);
        }

        private async Task TransactionalBulkInsertTimeEventsDataAsync(List<TimeEventData> teds)
        {
            IEnumerable<string> @params = teds.Select((x, i) => $"(@studentid{i}, @eventid{i}, @timestamp{i}, @payload{i}::json)");

            string query = "INSERT INTO timeeventsdata (studentid, eventid, timestamp, payload) VALUES " +
                            string.Join(", ", @params);

            var parameters = new DynamicParameters(new Dictionary<string, object?>());

            for (int i = 0; i < teds.Count; i++)
            {
                parameters.Add($"@studentid{i}", teds[i].StudentId);
                parameters.Add($"@eventid{i}", teds[i].EventId);
                parameters.Add($"@timestamp{i}", teds[i].Timestamp);
                parameters.Add($"@payload{i}", teds[i].Payload);
            }

            await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            await using var transaction = await connection.BeginTransactionAsync();
            bool rollbackIsNeeded = true;

            try
            {
                await connection.ExecuteAsync(query, parameters);

                await transaction.CommitAsync();
                rollbackIsNeeded = false;
            }
            finally
            {
                if (rollbackIsNeeded)
                {
                    await transaction.RollbackAsync();
                }

                await connection.CloseAsync();
            }
        }

        public Task<List<TimeEventData>> GetTimeEventsDataAsync(int? offset = null, int? limit = null)
        {
            offset ??= 0;
            limit ??= int.MaxValue;

            return _context.TimeEventsData
                .Take(offset.Value..limit.Value)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
