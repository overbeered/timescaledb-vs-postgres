using Demo.Database.Contexts.Postgres;
using Demo.Database.Contexts.TimescaleDB;
using Demo.Database.Models;
using Demo.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Demo.Playground
{
#nullable disable
    public class Program
    {
        private static TimeEventData _timeEventData;

        private static TimescaleDbTimeEventDataRepository _timescaleDbRepository;
        private static PostgresDbTimeEventDataRepository _postgresDbRepository;

        public static async Task Main(string[] args)
        {
            Init();

            await WriteDataAsync();
        }

        public static async Task WriteDataAsync()
        {
            var tasks = new List<Task>();

            int counter = 0;
            int bound = 6666;

            while (counter != bound)
            {
                var pgTask = TestPostgresDbAsync();
                tasks.Add(pgTask);

                var timescaleTask = TestTimescaleDbAsync();
                tasks.Add(timescaleTask);

                counter++;
                Console.WriteLine($"[{bound}/{counter}] {counter * 15000}");
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("Data is wrote...");
        }

        public static async Task TestPostgresDbAsync()
        {
            var teds = new List<TimeEventData>();
            for (int i = 0; i < 15000; i++)
            {
                teds.Add(_timeEventData);
            }

            await _postgresDbRepository.AddTimeEventsDataAsync(teds);
        }
        public static async Task TestTimescaleDbAsync()
        {
            var teds = new List<TimeEventData>();
            for (int i = 0; i < 15000; i++)
            {
                teds.Add(_timeEventData);
            }

            await _timescaleDbRepository.AddTimeEventsDataAsync(teds);
        }

        public static void Init(/*int itemsToCreate*/)
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
        }
    }
#nullable restore
}
