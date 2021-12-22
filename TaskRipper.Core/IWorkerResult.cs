namespace TaskRipper.Core
{
    public interface IWorkerResult : IDateRange
    {
        IWorkContract WorkContract { get; }
        TimeSpan Duration { get; }
        int ThreadsUsed { get; }
    }

    public class WorkerResult : IWorkerResult
    {
        public WorkerResult(IWorkContract workContract, int threadsUsed, IDateRange dateRange)
        {
            WorkContract = workContract;
            ThreadsUsed = threadsUsed;
            StartDate = dateRange.StartDate;
            EndDate = dateRange.EndDate;
            Duration = dateRange.EndDate - dateRange.StartDate;
        }
        public IWorkContract WorkContract { get; }

        public TimeSpan Duration { get; }

        public int ThreadsUsed { get; }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }
    }
}
