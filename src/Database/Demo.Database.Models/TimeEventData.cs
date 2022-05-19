namespace Demo.Database.Models
{
    public class TimeEventData
    {
        public Guid StudentId { get; set; }

        public Guid EventId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string Payload { get; set; }

        public TimeEventData(Guid studentId, Guid eventId, DateTimeOffset timestamp, string payload)
        {
            StudentId = studentId;
            EventId = eventId;
            Timestamp = timestamp;
            Payload = payload;
        }
    }
}
