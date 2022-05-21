namespace Demo.Database.Contexts.TimescaleDB
{
    public class TimeEventDataHypertableSharedResource
    {
        public long IsNormalMode;

        public long WorkingThreadsCounter;

        public readonly string HypertableName;

        public readonly SemaphoreSlim Semaphore;

        public TimeEventDataHypertableSharedResource()
        {
            HypertableName = "timeeventsdata";

            IsNormalMode = 1;
            WorkingThreadsCounter = 0;

            Semaphore = new SemaphoreSlim(1, 1);
        }
    }
}
