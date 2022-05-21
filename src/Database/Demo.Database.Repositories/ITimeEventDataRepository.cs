using Demo.Database.Models;

namespace Demo.Database.Repositories
{
    public interface ITimeEventDataRepository
    {
        Task<bool> TimeEventDataExistsAsync(Guid studentId, DateTimeOffset timestamp);

        Task<List<TimeEventData>> GetTimeEventsDataAsync(int? offset = null, int? limit = null);

        Task AddTimeEventDataAsync(Guid studentId, Guid eventId, DateTimeOffset timestamp, string payload);

        Task AddTimeEventsDataAsync(List<TimeEventData> teds);

        Task UpdateTimeEventDataAsync(Guid studentId, Guid eventId, DateTimeOffset timestamp, string payload);

        Task RemoveTimeEventsDataAsync(Guid studentId, DateTimeOffset timestamp);
    }
}
