using Demo.Database.Contexts.Postgres;
using Demo.Database.Contexts.TimescaleDB;
using Demo.Database.Models;
using Demo.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

namespace Demo.Playground
{
#nullable disable
    public class Program
    {
        private static TimeEventData _timeEventData;
        private static List<TimeEventData> _listTimeEventData;

        private static TimescaleDbTimeEventDataRepository _timescaleDbRepository;
        private static PostgresDbTimeEventDataRepository _postgresDbRepository;

        //public static void Main(string[] args)
        //{

        //}

        public static async Task Main(string[] args)
        {
            // 65535
            // 4

            Init(16383);

            int total = await TestPostgresDbAsync();
            Console.WriteLine(total);
        }

        public static async Task<int> TestPostgresDbAsync()
        {
            var timer = new Stopwatch();
            timer.Start();

            await _postgresDbRepository.AddTimeEventsDataAsync(_listTimeEventData);

            timer.Stop();

            TimeSpan timeTaken = timer.Elapsed;

            Console.WriteLine("Time taken: " + timeTaken.ToString(@"m\:ss\.fff"));

            return timeTaken.Seconds;
        }

        public static void Init(int itemsToCreate)
        {
            var timescaleDbContext = new DbContextOptionsBuilder<TimescaleDbContext>()
                .UseNpgsql("Host=localhost;Port=5002;Database=timescaledb-ted-database;Username=postgres;Password=postgres")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;
            var contextTimescale = new TimescaleDbContext(timescaleDbContext);
            _timescaleDbRepository = new TimescaleDbTimeEventDataRepository(contextTimescale);

            var postgresDbContext = new DbContextOptionsBuilder<PostgresDbContext>()
                .UseNpgsql("Host=localhost;Port=5001;Database=postgres-ted-database;Username=postgres;Password=postgres")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;
            var contextPostgres = new PostgresDbContext(postgresDbContext);
            _postgresDbRepository = new PostgresDbTimeEventDataRepository(contextPostgres);

            _timeEventData = new TimeEventData(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow, JsonSerializer.Serialize(new { DataId = "AYE" }));

            _listTimeEventData = new List<TimeEventData>();

            for (int i = 0; i < itemsToCreate; i++)
            {
                _listTimeEventData.Add(_timeEventData);
            }
        }
    }
#nullable restore
}
