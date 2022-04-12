namespace TaskRipper.Core
{
    public interface IWorkerResult<TResult> : IWorkerResult
    {
        IDictionary<int, IterationResult<TResult>> Results { get; }
    }

    public interface IWorkerResult
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
            Duration = dateRange.EndDate - dateRange.StartDate;
        }
        public IWorkContract WorkContract { get; }

        public TimeSpan Duration { get; }

        public int ThreadsUsed { get; }

        /// <summary>
        /// Inspect this task to see information on the overall task
        /// </summary>
        public Task ExecuterTask { get; }
    }

    public class WorkerResult<TResult> : WorkerResult, IWorkerResult<TResult>
    {
        public WorkerResult(IWorkContract workContract, int threadsUsed, IDateRange dateRange, IDictionary<int, IterationResult<TResult>> results) 
            : base(workContract, threadsUsed, dateRange)
        {
            Results = results;
        }

        public IDictionary<int, IterationResult<TResult>> Results { get; }
    }
}
