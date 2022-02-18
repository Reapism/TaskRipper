namespace TaskRipper.Core
{
    public enum LoadBalancerOptions
    {
        /// <summary>
        /// Allow TaskRipper to infer the task load based on the number of iterations, and maximum threads available
        /// and chooses how to balance the load of the task.
        /// <para>Only choose this option if don't understand the load of each iterable task.</para>
        /// </summary>
        Optimize,

        /// <summary>
        /// Split the iterations up in a way where the work is divided proportionally,
        /// to each thread while using the maximum number of threads available.
        /// <para>
        /// Choose this option for a task to iterate 
        /// </para>
        /// </summary>
        Fair,

        /// <summary>
        /// For low iterations, use maximum number of threads available.
        /// <para>
        /// Choose this option for a task with as little iterations as there are threads
        /// to maximize concurrency.
        /// </para>
        /// </summary>
        Heavy,

        /// <summary>
        /// Split up the iterations on half as many threads are available.
        /// </summary>
        Moderate,

        /// <summary>
        /// Choose this option to iterate on a single thread.
        /// </summary>
        None,
    }
    public interface IExecutionSettings
    {
        /// <summary>
        /// Gets a <see cref="Range"/> indicating the
        /// minimum and maximum number of threads
        /// to allow and use.
        /// </summary>
        Range ThreadRange{ get; }

        /// Gets a <see cref="Range"/> indicating the
        /// minimum and maximum number of iterations
        /// to allow.
        Range ExecutionRange { get; }
        
        /// <summary>
        /// The <see cref="IExecutionEnvironment"/>.
        /// </summary>
        IExecutionEnvironment ExecutionEnvironment { get; }

        LoadBalancerOptions LoadBalancerOptions { get; }
    }

    public class ExecutionSettings : IExecutionSettings
    {
        private static IExecutionSettings? defaultInstance;
        /// <summary>
        /// Gets a default instance of the <see cref="IExecutionSettings"/>
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
                    var executionEnv = new ExecutionEnvironment();
                    var threadRange = new Range(1, executionEnv.ThreadCount);
                    var maxExecutionRange = threadRange.End.Value * 1000;
                    var executionRange = new Range(1, maxExecutionRange);
                    defaultInstance = new ExecutionSettings(executionEnv, threadRange, executionRange);

                }

                return defaultInstance;
            }
        }

        public ExecutionSettings(IExecutionEnvironment executionEnvironment, Range threadRange, Range executionRange)
        {
            ThreadRange = threadRange;
            ExecutionRange = executionRange;
            ExecutionEnvironment = executionEnvironment;
        }

        public ExecutionSettings(Range threadRange, Range executionRange)
            : this(new ExecutionEnvironment(), threadRange, executionRange)
        {
        }

        /// <inheritdoc/>
        public Range ThreadRange { get; }

        /// <inheritdoc/>
        public Range ExecutionRange { get; }

        /// <inheritdoc/>
        public IExecutionEnvironment ExecutionEnvironment { get; }

        public LoadBalancerOptions LoadBalancerOptions { get; }
    }
}
