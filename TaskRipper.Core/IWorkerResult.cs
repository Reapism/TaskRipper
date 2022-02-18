namespace TaskRipper.Core
{
    public interface IWorkerResult<T> : IWorkerResult
    {
        IDictionary<int, IterationResult<T>> Results { get; }
    }

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

        /// <summary>
        /// Inspect this task to see information on the overall task
        /// </summary>
        public Task ExecuterTask { get; }
    }

    public class WorkerResult<T> : WorkerResult, IWorkerResult<T>
    {
        public WorkerResult(IWorkContract workContract, int threadsUsed, IDateRange dateRange, IDictionary<int, IterationResult<T>> results) 
            : base(workContract, threadsUsed, dateRange)
        {
            Results = results;
        }

        public IDictionary<int, IterationResult<T>> Results { get; }
    }
}
