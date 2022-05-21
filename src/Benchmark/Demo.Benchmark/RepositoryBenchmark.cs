using BenchmarkDotNet.Attributes;
using Demo.Database.Contexts.Postgres;
using Demo.Database.Contexts.Postgres.Extensions;
using Demo.Database.Contexts.TimescaleDB;
using Demo.Database.Contexts.TimescaleDB.Extensions;
using Demo.Database.Models;
using Demo.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Demo.Benchmark
{
    public class RepositoryBenchmark
    {
        // private readonly string _tableName = "timeeventsdata";

        private readonly TimescaleDbContext _contextTimescale;
        private readonly PostgresDbContext _contextPostgres;

        private readonly List<TimeEventData> _listTimeEventData1000;
        private readonly List<TimeEventData> _listTimeEventData5000;
        private readonly List<TimeEventData> _listTimeEventData10000;
        private readonly List<TimeEventData> _listTimeEventData15000;

        public RepositoryBenchmark()
        {
            var timescaleDbContext = new DbContextOptionsBuilder<TimescaleDbContext>()
                .UseNpgsql("Host=localhost;Port=5002;Database=timescaledb-ted-database;Username=postgres;Password=postgres")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;

            _contextTimescale = new TimescaleDbContext(timescaleDbContext);

            var postgresDbContext = new DbContextOptionsBuilder<PostgresDbContext>()
                .UseNpgsql("Host=localhost;Port=5001;Database=postgres-ted-database;Username=postgres;Password=postgres")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;

            _contextPostgres = new PostgresDbContext(postgresDbContext);

            var timeEventData = new TimeEventData(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow, JsonSerializer.Serialize(new { DataId = "AYE" }));

            _listTimeEventData1000 = new List<TimeEventData>();
            for (int i = 1; i <= 1000; i++)
            {
                _listTimeEventData1000.Add(timeEventData);
            }

            _listTimeEventData5000 = new List<TimeEventData>();
            for (int i = 1; i <= 5000; i++)
            {
                _listTimeEventData5000.Add(timeEventData);
            }

            _listTimeEventData10000 = new List<TimeEventData>();
            for (int i = 1; i <= 10000; i++)
            {
                _listTimeEventData10000.Add(timeEventData);
            }

            _listTimeEventData15000 = new List<TimeEventData>();
            for (int i = 1; i <= 15000; i++)
            {
                _listTimeEventData15000.Add(timeEventData);
            }
        }

        [Benchmark]
        public async Task TimescaleDbTimeEventData1000()
        {
            // await _contextTimescale.DropChunksNewerThanNegativeInfinityAsync(_tableName);

            var repository = new TimescaleDbTimeEventDataRepository(_contextTimescale);

            await repository.AddTimeEventsDataAsync(_listTimeEventData1000);
        }

        [Benchmark]
        public async Task PostgresDbTimeEventData1000()
        {
            // await _contextPostgres.TruncateTableAsync(_tableName);

            var repository = new PostgresDbTimeEventDataRepository(_contextPostgres);

            await repository.AddTimeEventsDataAsync(_listTimeEventData1000);
        }

        [Benchmark]
        public async Task TimescaleDbTimeEventData5000()
        {
            // await _contextTimescale.DropChunksNewerThanNegativeInfinityAsync(_tableName);

            var repository = new TimescaleDbTimeEventDataRepository(_contextTimescale);

            await repository.AddTimeEventsDataAsync(_listTimeEventData5000);
        }

        [Benchmark]
        public async Task PostgresDbTimeEventData5000()
        {
            // await _contextPostgres.TruncateTableAsync(_tableName);

            var repository = new PostgresDbTimeEventDataRepository(_contextPostgres);

            await repository.AddTimeEventsDataAsync(_listTimeEventData5000);
        }

        [Benchmark]
        public async Task TimescaleDbTimeEventData10000()
        {
            // await _contextTimescale.DropChunksNewerThanNegativeInfinityAsync(_tableName);

            var repository = new TimescaleDbTimeEventDataRepository(_contextTimescale);

            await repository.AddTimeEventsDataAsync(_listTimeEventData10000);
        }

        [Benchmark]
        public async Task PostgresDbTimeEventData10000()
        {
            // await _contextPostgres.TruncateTableAsync(_tableName);

            var repository = new PostgresDbTimeEventDataRepository(_contextPostgres);

            await repository.AddTimeEventsDataAsync(_listTimeEventData10000);
        }

        [Benchmark]
        public async Task TimescaleDbTimeEventData15000()
        {
            // await _contextTimescale.DropChunksNewerThanNegativeInfinityAsync(_tableName);

            var repository = new TimescaleDbTimeEventDataRepository(_contextTimescale);

            await repository.AddTimeEventsDataAsync(_listTimeEventData15000);
        }

        [Benchmark]
        public async Task PostgresDbTimeEventData15000()
        {
            // await _contextPostgres.TruncateTableAsync(_tableName);

            var repository = new PostgresDbTimeEventDataRepository(_contextPostgres);

            await repository.AddTimeEventsDataAsync(_listTimeEventData15000);
        }
    }
}
