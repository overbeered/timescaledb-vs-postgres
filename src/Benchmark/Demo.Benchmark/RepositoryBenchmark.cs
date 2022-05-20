using BenchmarkDotNet.Attributes;
using Demo.Database.Contexts.Postgres;
using Demo.Database.Contexts.TimescaleDB;
using Demo.Database.Models;
using Demo.Database.Repositories;

namespace Demo.Benchmark
{
    internal class RepositoryBenchmark
    {
        private readonly PostgresDbContext _contextPostgres;
        private readonly TimescaleDbContext _contextTimescale;
        private readonly TimeEventData _timeEventData;
        private readonly List<TimeEventData> _listTimeEventData1000;

        public RepositoryBenchmark(PostgresDbContext contextPostgres, TimescaleDbContext contextTimescale)
        {
            _contextPostgres = contextPostgres;
            _contextTimescale = contextTimescale;
            _timeEventData = new TimeEventData(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow, "test");
            _listTimeEventData1000 = new List<TimeEventData>();
            for (int i = 0; i < 999; i++)
            {
                _listTimeEventData1000.Add(_timeEventData);
            }
        }

        [Benchmark]
        public void TimescaleDbTimeEventData()
        {
            var repository = new TimescaleDbTimeEventDataRepository(_contextTimescale);
            repository.AddTimeEventDataAsync(_timeEventData);
        }

        [Benchmark]
        public void PostgresDbTimeEventData()
        {
            var repository = new PostgresDbTimeEventDataRepository(_contextPostgres);
            repository.AddTimeEventDataAsync(_timeEventData);
        }

        [Benchmark]
        public void TimescaleDbTimeEventData1000()
        {
            var repository = new TimescaleDbTimeEventDataRepository(_contextTimescale);
            repository.AddTimeEventsDataAsync(_listTimeEventData1000);
        }

        [Benchmark]
        public void PostgresDbTimeEventData1000()
        {
            var repository = new PostgresDbTimeEventDataRepository(_contextPostgres);
            repository.AddTimeEventsDataAsync(_listTimeEventData1000);
        }
    }
}
