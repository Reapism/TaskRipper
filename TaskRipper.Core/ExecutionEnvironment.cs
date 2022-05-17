namespace TaskRipper.Core
{
    public class ExecutionEnvironment : IExecutionEnvironment, IEquatable<ExecutionEnvironment>
    {
        public ExecutionEnvironment()
        {
            ThreadCount = Environment.ProcessorCount;
        }

        public int ThreadCount { get; }

        /// <summary>
        /// Returns an instance using the default constructor.
        /// </summary>
        public static IExecutionEnvironment Default { get; } = new ExecutionEnvironment();

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
