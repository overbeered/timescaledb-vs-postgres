using Dapper;
using Demo.Database.Contexts.Shared;
using Demo.Database.Contexts.TimescaleDB;
using Demo.Database.Contexts.TimescaleDB.Extensions;
using Demo.Database.Models;
using Demo.Database.Repositories.Exceptions;
using Demo.Database.Repositories.Utils;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Demo.Database.Repositories
{
    public class TimescaleDbTimeEventDataRepository : RepositoryBase<TimescaleDbContext, TimeEventData>, ITimeEventDataRepository
    {
        private readonly TimeEventDataHypertableSharedResource _sharedResource;

        public TimescaleDbTimeEventDataRepository(TimescaleDbContext context, TimeEventDataHypertableSharedResource sharedResource)
            : base(context)
        {
            _sharedResource = sharedResource;
        }

        #region AddTimeEventDataAsync
        public Task AddTimeEventDataAsync(Guid studentId, Guid eventId, DateTimeOffset timestamp, string payload)
        {
            return TaskTransactionalInsertTimeEventDataAsync(studentId, eventId, timestamp, payload);
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
        #endregion

        #region AddTimeEventsDataAsync
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
        #endregion

        #region GetTimeEventsDataAsync
        public Task<List<TimeEventData>> GetTimeEventsDataAsync(int? offset = null, int? limit = null)
        {
            offset ??= 0;
            limit ??= int.MaxValue;

            return _context.TimeEventsData
                .Take(offset.Value..limit.Value)
                .AsNoTracking()
                .ToListAsync();
        }
        #endregion

        #region TimeEventDataExistsAsync
        public Task<bool> TimeEventDataExistsAsync(Guid studentId,
            DateTimeOffset timestamp)
        {
            return _context.TimeEventsData
                .AnyAsync(ted => ted.StudentId == studentId && ted.Timestamp == timestamp);
        }
        #endregion

        #region UpdateTimeEventDataAsync
        public async Task UpdateTimeEventDataAsync(Guid studentId,
            Guid eventId,
            DateTimeOffset timestamp,
            string payload)
        {
            if (Interlocked.Read(ref _sharedResource.IsNormalMode) == 1)
            {
                Interlocked.Increment(ref _sharedResource.WorkingThreadsCounter);

                await _sharedResource.Semaphore.WaitAsync();

                try
                {
                    if (!await TimeEventDataExistsAsync(studentId, timestamp))
                    {
                        throw new TimeEventsStorageNotFoundException($"Time event data with [StudentId: {studentId} and Timestamp: {timestamp}] is not exist!");
                    }

                    try
                    {
                        await TransactionalUpdateTimeEventDataInUncompressedChunkBySourceIdAsync(studentId, eventId, timestamp, payload);
                    }
                    catch (Exception ex)
                    when (ex.InnerException is not null && TimescaleExceptionsMessagesChecker.CheckModification(ex.InnerException.Message, out string chunkName))
                    {
                        await TransactionalUpdateTimeEventDataInCompressedChunkBySourceIdAsync(chunkName, studentId, eventId, timestamp, payload);
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref _sharedResource.WorkingThreadsCounter);

                    _sharedResource.Semaphore.Release();
                }
            }
            else
            {
                throw new TimeEventsStorageHypertableModeException(_sharedResource.HypertableName,
                    $"Can't insert/update/remove data from hypertable '{_sharedResource.HypertableName}', because it's in configuration mode!");
            }
        }

        private async Task UpdateTimeEventDataInUncompressedChunkBySourceIdAsync(Guid studentId,
            Guid eventId,
            DateTimeOffset timestamp,
            string payload)
        {
            string query = @"UPDATE timeeventsdata
                             SET eventid = @eventid, payload = @payload::json
                             WHERE studentId = @studentId AND timestamp = @timestamp;";

            var parameters = new DynamicParameters(new Dictionary<string, object?>
            {
                { "@sourceid",  studentId  },
                { "@timestamp", timestamp },
                { "@eventid",   eventId   },
                { "@payload",  payload  }
            });

            await _context.Connection.ExecuteAsync(query, parameters);
        }

        private async Task TransactionalUpdateTimeEventDataInUncompressedChunkBySourceIdAsync(Guid studentId,
            Guid eventId,
            DateTimeOffset timestamp,
            string payload)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            bool rollbackIsNeeded = true;

            try
            {
                await UpdateTimeEventDataInUncompressedChunkBySourceIdAsync(studentId, eventId, timestamp, payload);

                await transaction.CommitAsync();
                rollbackIsNeeded = false;
            }
            finally
            {
                if (rollbackIsNeeded)
                {
                    await transaction.RollbackAsync();
                }
            }
        }

        private async Task TransactionalUpdateTimeEventDataInCompressedChunkBySourceIdAsync(string chunkName,
            Guid studentId,
            Guid eventId,
            DateTimeOffset timestamp,
            string payload)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            bool rollbackIsNeeded = true;

            try
            {
                await _context.FindsAndPauseCompressionPolicyJobAsync(_sharedResource.HypertableName);

                await _context.DecompressChunkAsync(chunkName);

                await UpdateTimeEventDataInUncompressedChunkBySourceIdAsync(studentId, eventId, timestamp, payload);

                await _context.FindsAndScheduleCompressionPolicyJobAsync(_sharedResource.HypertableName);

                await transaction.CommitAsync();
                rollbackIsNeeded = false;
            }
            finally
            {
                if (rollbackIsNeeded)
                {
                    await transaction.RollbackAsync();
                }
            }
        }
        #endregion

        #region RemoveTimeEventsDataAsync
        public async Task RemoveTimeEventsDataAsync(Guid studentId,
            DateTimeOffset timestamp)
        {
            if (Interlocked.Read(ref _sharedResource.IsNormalMode) == 1)
            {
                Interlocked.Increment(ref _sharedResource.WorkingThreadsCounter);

                await _sharedResource.Semaphore.WaitAsync();

                try
                {
                    if (!await TimeEventDataExistsAsync(studentId, timestamp))
                    {
                        throw new TimeEventsStorageNotFoundException($"Time event data with [StudentId: {studentId} and Timestamp: {timestamp}] is not exist!");
                    }

                    try
                    {
                        await TransactionalRemoveTimeEventsDataFromUncompressedChunkBySourceIdAsync(studentId, timestamp);
                    }
                    catch (Exception ex)
                    when (ex.InnerException is not null && TimescaleExceptionsMessagesChecker.CheckModification(ex.InnerException.Message, out string chunkName))
                    {
                        await TransactionalRemoveTimeEventsDataFromCompressedChunkBySourceIdAsync(chunkName, studentId, timestamp);
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref _sharedResource.WorkingThreadsCounter);

                    _sharedResource.Semaphore.Release();
                }
            }
            else
            {
                throw new TimeEventsStorageHypertableModeException(_sharedResource.HypertableName,
                    $"Can't insert/update/remove data from hypertable '{_sharedResource.HypertableName}', because it's in configuration mode!");
            }
        }

        private async Task RemoveTimeEventsDataFromUncompressedChunkBySourceIdAsync(Guid studentId, DateTimeOffset timestamp)
        {
            string query = @"DELETE FROM timeeventsdata
                             WHERE studentId = @studentId AND timestamp = @timestamp;";

            var parameters = new DynamicParameters(new Dictionary<string, object?>
            {
                { "@studentId",  studentId  },
                { "@timestamp", timestamp }
            });

            await _context.Connection.QueryAsync<TimeEventData>(query, parameters);
        }

        private async Task TransactionalRemoveTimeEventsDataFromUncompressedChunkBySourceIdAsync(Guid studentId,
            DateTimeOffset timestamp)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            bool rollbackIsNeeded = true;

            try
            {
                await RemoveTimeEventsDataFromUncompressedChunkBySourceIdAsync(studentId, timestamp);

                await transaction.CommitAsync();
                rollbackIsNeeded = false;
            }
            finally
            {
                if (rollbackIsNeeded)
                {
                    await transaction.RollbackAsync();
                }
            }
        }

        private async Task TransactionalRemoveTimeEventsDataFromCompressedChunkBySourceIdAsync(string chunkName,
            Guid sourceId,
            DateTimeOffset timestamp)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            bool rollbackIsNeeded = true;

            try
            {
                await _context.FindsAndPauseCompressionPolicyJobAsync(_sharedResource.HypertableName);

                await _context.DecompressChunkAsync(chunkName);

                await RemoveTimeEventsDataFromUncompressedChunkBySourceIdAsync(sourceId, timestamp);

                await _context.FindsAndScheduleCompressionPolicyJobAsync(_sharedResource.HypertableName);

                await transaction.CommitAsync();
                rollbackIsNeeded = false;
            }
            finally
            {
                if (rollbackIsNeeded)
                {
                    await transaction.RollbackAsync();
                }
            }
        }
        #endregion
    }
}
