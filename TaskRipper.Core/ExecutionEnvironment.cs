namespace TaskRipper.Core
{
    public class ExecutionEnvironment : IExecutionEnvironment, IEquatable<ExecutionEnvironment>
    {
        public ExecutionEnvironment()
        {
            ThreadCount = Environment.ProcessorCount;
        }

        public ExecutionEnvironment(int threadCount)
        {
            ThreadCount = threadCount;
        }

        public int ThreadCount { get; }

        /// <summary>
        /// Returns an instance using the default constructor.
        /// </summary>
        public static IExecutionEnvironment Default { get; } = new ExecutionEnvironment();
        public static IExecutionEnvironment Create(int threadCount)
        {
            return new ExecutionEnvironment(threadCount);
        }

        public static IExecutionEnvironment FromThreadPool()
        {
            long threadPoolCount = GetThreadPoolCount();

            // This cast will never fail since the min case will at least be int.MaxValue
            int maxPossibleThreadPoolValue = (int)Math.Min(threadPoolCount, int.MaxValue);
            return new ExecutionEnvironment(maxPossibleThreadPoolValue);
        }

        private static long GetThreadPoolCount()
        {
            ThreadPool.GetAvailableThreads(out int availableWorkerThreads, out int completionPortThreads);

            return Math.Max(availableWorkerThreads, 0);
        }

        public bool Equals(ExecutionEnvironment? other)
        {
            if (other is null)
                return false;

            return ThreadCount == other.ThreadCount;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (obj is ExecutionEnvironment other)
                return Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine
            (
                ThreadCount
            );
        }
    }
}
