namespace TaskRipper.Core
{
    public interface IExecutionSettings
    {
        /// <summary>
        /// Gets a <see cref="Range"/> indicating the
        /// minimum and maximum number of threads
        /// to allow and use.
        /// </summary>
        Range ThreadRange { get; }

        /// Gets a <see cref="Range"/> indicating the
        /// minimum and maximum number of iterations
        /// to allow.
        Range ExecutionRange { get; }
        
        /// <summary>
        /// The <see cref="IExecutionEnvironment"/>.
        /// </summary>
        IExecutionEnvironment ExecutionEnvironment { get; }

        /// <summary>
        /// The options to describe to a <see cref="IWorkBalancer"/>
        /// how to balance the work.
        /// </summary>
        WorkBalancerOptions WorkBalancerOptions { get; }
    }

    public class ExecutionSettings : IExecutionSettings, IEquatable<ExecutionSettings>
    {
        private static IExecutionSettings? defaultInstance;
        /// <summary>
        /// Gets a default instance of the <see cref="IExecutionSettings"/>.
        /// </summary>
        /// <remarks>
        /// * Default <see cref="IExecutionSettings.ThreadRange"/> is (1 - <see cref="IExecutionEnvironment.ThreadCount"/>)
        /// <para>
        /// * Default <see cref="IExecutionSettings.ExecutionRange"/> is (1 - <see cref="IExecutionEnvironment.ThreadCount"/> * 1000)
        /// </para>
        /// </remarks>
        public static IExecutionSettings Default
        {
            get 
            {
                if (defaultInstance is null)
                {
                    var executionEnv = LocalExecutionEnvironment.Default;
                    var threadRange = new Range(1, executionEnv.ThreadCount);
                    var maxExecutionRange = threadRange.End.Value * 1000;
                    var executionRange = new Range(1, maxExecutionRange);
                    defaultInstance = new ExecutionSettings(executionEnv, threadRange, executionRange, WorkBalancerOptions.Optimize);
                }

                return defaultInstance;
            }
        }

        public static IExecutionSettings Create(IExecutionEnvironment executionEnvironment, Range threadRange, Range executionRange, WorkBalancerOptions workBalancerOptions)
        {
            return new ExecutionSettings(executionEnvironment, threadRange, executionRange, workBalancerOptions);
        }

        private ExecutionSettings(IExecutionEnvironment executionEnvironment, Range threadRange, Range executionRange, WorkBalancerOptions workBalancerOptions)
        {
            ThreadRange = threadRange;
            ExecutionRange = executionRange;
            ExecutionEnvironment = executionEnvironment;
            WorkBalancerOptions = workBalancerOptions;
        }

        /// <inheritdoc/>
        public Range ThreadRange { get; }

        /// <inheritdoc/>
        public Range ExecutionRange { get; }

        /// <inheritdoc/>
        public IExecutionEnvironment ExecutionEnvironment { get; }

        /// <inheritdoc/>
        public WorkBalancerOptions WorkBalancerOptions { get; }

        public bool Equals(ExecutionSettings? other)
        {
            if (other is null)
                return false;

            return this.ExecutionRange.Equals(other.ExecutionRange) &&
                ThreadRange.Equals(other.ThreadRange) &&
                WorkBalancerOptions == other.WorkBalancerOptions &&
                ExecutionEnvironment.Equals(other.ExecutionEnvironment);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (obj is ExecutionSettings other)
                return Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine
            (
                ExecutionRange.GetHashCode(),
                ThreadRange.GetHashCode(),
                ExecutionEnvironment.GetHashCode(),
                WorkBalancerOptions.GetHashCode()
            );
        }
    }
}
