namespace TaskRipper.Core
{
    public interface IWorkResult<TResult> : IWorkResult
    {
        IDictionary<int, IterationResult<TResult>> Results { get; }
    }

    public interface IWorkResult
    {
        /// <summary>
        /// The original contract used to generate this result.
        /// </summary>
        IWorkContract WorkContract { get; }

        /// <summary>
        /// The duration for this execution.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The number of threads used for this specific task.
        /// </summary>
        int ThreadsUsed { get; }
    }

    public class WorkResult : IWorkResult
    {
        public WorkResult(IWorkContract workContract, int threadsUsed, IDateRange dateRange)
        {
            WorkContract = workContract;
            ThreadsUsed = threadsUsed;
            Duration = dateRange.EndDate - dateRange.StartDate;
        }
        /// <inheritdoc/>
        public IWorkContract WorkContract { get; }

        /// <inheritdoc/>
        public TimeSpan Duration { get; }

        /// <inheritdoc/>
        public int ThreadsUsed { get; }

        /// <summary>
        /// Inspect this task to see information on the overall task
        /// </summary>
        public Task ExecuterTask { get; }
    }

    public class WorkResult<TResult> : WorkResult, IWorkResult<TResult>
    {
        public WorkResult(IWorkContract workContract, int threadsUsed, IDateRange dateRange, IDictionary<int, IterationResult<TResult>> results) 
            : base(workContract, threadsUsed, dateRange)
        {
            Results = results;
        }

        public IDictionary<int, IterationResult<TResult>> Results { get; }
    }
}
