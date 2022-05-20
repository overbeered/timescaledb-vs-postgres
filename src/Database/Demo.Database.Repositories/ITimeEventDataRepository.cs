using Demo.Database.Models;

namespace Demo.Database.Repositories
{
    public interface ITimeEventDataRepository
    {
        Task<List<TimeEventData>> GetTimeEventsDataAsync(int? offset = null, int? limit = null);

        Task AddTimeEventDataAsync(TimeEventData ted);

        Task AddTimeEventsDataAsync(List<TimeEventData> teds);
    }
}
