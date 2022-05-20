using BenchmarkDotNet.Attributes;
using Demo.Database.Contexts.Postgres;
using Demo.Database.Contexts.TimescaleDB;
using Demo.Database.Models;
using Demo.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Demo.Benchmark
{
    public class RepositoryBenchmark
    {
        private readonly TimescaleDbContext _contextTimescale;
        private readonly PostgresDbContext _contextPostgres;

        private readonly TimeEventData _timeEventData;

        private readonly List<TimeEventData> _listTimeEventData1000;

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

            _timeEventData = new TimeEventData(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow, JsonSerializer.Serialize(new { DataId = "AYE" }));
            _listTimeEventData1000 = new List<TimeEventData>();
            for (int i = 1; i <= 1000; i++)
            {
                _listTimeEventData1000.Add(_timeEventData);
            }
        }

        //[Benchmark]
        //public void TimescaleDbTimeEventData()
        //{
        //    var repository = new TimescaleDbTimeEventDataRepository(_contextTimescale);
        //    repository.AddTimeEventDataAsync(_timeEventData);
        //}

        //[Benchmark]
        //public void PostgresDbTimeEventData()
        //{
        //    var repository = new PostgresDbTimeEventDataRepository(_contextPostgres);
        //    repository.AddTimeEventDataAsync(_timeEventData);
        //}

        //[Benchmark]
        //public void TimescaleDbTimeEventData1000()
        //{
        //    var repository = new TimescaleDbTimeEventDataRepository(_contextTimescale);
        //    repository.AddTimeEventsDataAsync(_listTimeEventData1000);
        //}

        //[Benchmark]
        //public void PostgresDbTimeEventData1000()
        //{
        //    var repository = new PostgresDbTimeEventDataRepository(_contextPostgres);
        //    repository.AddTimeEventsDataAsync(_listTimeEventData1000);
        //}
    }
}
