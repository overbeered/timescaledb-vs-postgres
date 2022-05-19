namespace Demo.Database.Models
{
    public class TimeEventData
    {
        public Guid StudentId { get; set; }

        public Guid EventId { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
