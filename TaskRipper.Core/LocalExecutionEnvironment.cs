namespace TaskRipper.Core
{
    public interface IExecutionEnvironment
    {
        string MachineName { get; }
        int ThreadCount { get; }
    }
    public sealed record LocalExecutionEnvironment : IExecutionEnvironment
    {
        private LocalExecutionEnvironment()
            : this(Environment.MachineName, Environment.ProcessorCount)
        { }

        private LocalExecutionEnvironment(string machineName, int threadCount)
        {
            MachineName = Guard.Against.Null(machineName);
            ThreadCount = threadCount;
        }

        public int ThreadCount { get; }
        public string MachineName { get; }

        /// <summary>
        /// Returns an instance using the default constructor.
        /// </summary>
        public static IExecutionEnvironment Default { get; } = new LocalExecutionEnvironment();

        public static IExecutionEnvironment Create(int threadCount, string machineName)
        {
            return new LocalExecutionEnvironment(machineName, threadCount);
        }

        public static IExecutionEnvironment FromThreadPool()
        {
            long threadPoolCount = GetThreadPoolCount();

            // This cast will never fail since the min case will at least be int.MaxValue
            int maxPossibleThreadPoolValue = (int)Math.Min(threadPoolCount, int.MaxValue);
            return new LocalExecutionEnvironment(Environment.MachineName, maxPossibleThreadPoolValue);
        }

        private static long GetThreadPoolCount()
        {
            ThreadPool.GetAvailableThreads(out int availableWorkerThreads, out int completionPortThreads);

            return Math.Max(availableWorkerThreads, 0);
        }

        public bool Equals(LocalExecutionEnvironment? other)
        {
            if (other is null)
                return false;

            return ThreadCount == other.ThreadCount && MachineName == other.MachineName;
        }
    }
}
