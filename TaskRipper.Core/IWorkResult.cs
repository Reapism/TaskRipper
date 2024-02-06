namespace TaskRipper.Core
{

    public interface IWorkResult<TResult>
    {
        IEnumerable<IEnumerable<TResult>> ResultsMatrix { get; }
        /// <summary>
        /// The original contract used to generate this result.
        /// </summary>
        IWorkContract OriginalContract { get; }

        bool HasCompleted { get; }

        bool ContractHonored { get; }

        /// <summary>
        /// The duration for this execution.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The number of threads used for this specific task.
        /// </summary>
        int ThreadsUsed { get; }
    }

    public sealed class WorkResult<TResult> : IWorkResult<TResult>
    {
        /// <inheritdoc/>
        public IWorkContract OriginalContract { get; init; }

        /// <inheritdoc/>
        public TimeSpan Duration { get; init; }

        /// <inheritdoc/>
        public int ThreadsUsed { get; init; }

        /// <summary>
        /// Inspect this task to see information on the overall task
        /// </summary>
        public Task ExecuterTask { get; init; }

        public IEnumerable<IEnumerable<TResult>> ResultsMatrix { get; init; }
        public int PartitionCount => ResultsMatrix.Count();
        public int TotalCount => ResultsMatrix.Sum(p => p.Count());

        public bool HasCompleted => ExecuterTask.IsCompleted;

        public bool ContractHonored => OriginalContract.IterationsRequested == TotalCount && HasCompleted;

        public IEnumerable<TResult> FromPartition(int partitionIndex)
        {
            var range = new Range(0, PartitionCount);

            if (!range.IsInRange(partitionIndex))
            {
                return Enumerable.Empty<TResult>();
            }

            return ResultsMatrix.ElementAt(partitionIndex);
        }
        public TResult? FromIndex(int partitionIndex, int index)
        {
            var partition = FromPartition(partitionIndex);
            
            if (partition == Enumerable.Empty<TResult>())
            {
                return default;
            }

            var count = partition.Count();
            var range = new Range(0, count);

            if (!range.IsInRange(index))
            {
                return default;
            }

            return partition.ElementAt(index);
        }
    }
}
