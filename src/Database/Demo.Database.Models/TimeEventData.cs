namespace Demo.Database.Models
{
    /// <summary>
    /// Данные о событии за определенный момент во времени.
    /// </summary>
    public class TimeEventData
    {
        /// <summary>
        /// Идентификатор студента.
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// Идентификатор события.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Временная метка.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Информация о произошедшем событии в формате JSON.
        /// </summary>
        public string Payload { get; set; }

        public TimeEventData(Guid studentId, Guid eventId, DateTimeOffset timestamp, string payload)
        {
            StudentId = studentId;
            EventId = eventId;
            Timestamp = timestamp;
            Payload = payload;
        }
    }

    //public class TimeEventData
    //{
    //    public Guid StudentId { get; set; }

    //    public Guid EventId { get; set; }

    //    public DateTimeOffset Timestamp { get; set; }

    //    public string Payload { get; set; }

    //    public TimeEventData(Guid studentId, Guid eventId, DateTimeOffset timestamp, string payload)
    //    {
    //        StudentId = studentId;
    //        EventId = eventId;
    //        Timestamp = timestamp;
    //        Payload = payload;
    //    }
    //}
}
