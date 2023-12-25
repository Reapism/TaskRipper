//namespace TaskRipper.Core
//{
//    public interface IWorkResult<TResult> : IWorkResult
//    {
//        /// <summary>
//        /// A collection of key/value pairs that contain the result of every iteration
//        /// by every thread for a particular work.
//        /// </summary>
//        IEnumerable<IEnumerable<IterationResult<TResult>>> Results { get; }
//    }

//    public interface IWorkResult
//    {
//        /// <summary>
//        /// The original contract used to generate this result.
//        /// </summary>
//        IWorkContract WorkContract { get; }

//        /// <summary>
//        /// The duration for this execution.
//        /// </summary>
//        TimeSpan Duration { get; }

//        /// <summary>
//        /// The number of threads used for this specific task.
//        /// </summary>
//        int ThreadsUsed { get; }
//    }

//    public class WorkResult : IWorkResult
//    {
//        public WorkResult(IWorkContract workContract, int threadsUsed)
//        {
//            WorkContract = workContract;
//            ThreadsUsed = threadsUsed;
//        }
//        /// <inheritdoc/>
//        public IWorkContract WorkContract { get; }

//        /// <inheritdoc/>
//        public TimeSpan Duration { get; }

//        /// <inheritdoc/>
//        public int ThreadsUsed { get; }

//        /// <summary>
//        /// Inspect this task to see information on the overall task
//        /// </summary>
//        public Task ExecuterTask { get; }
//    }

//    public class WorkResult<TResult> : WorkResult, IWorkResult<TResult>
//    {
//        public WorkResult(IWorkContract workContract, int threadsUsed, IEnumerable<IEnumerable<IterationResult<TResult>>> results) 
//            : base(workContract, threadsUsed)
//        {
//            Results = results;
//        }
//        /// <inheritdoc/>
//        public IEnumerable<IEnumerable<IterationResult<TResult>>> Results { get; }
//    }
//}
