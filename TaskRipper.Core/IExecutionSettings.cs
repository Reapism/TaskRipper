namespace TaskRipper.Core
{
    public enum WorkBalancerOptions
    {
        /// <summary>
        /// This option allows the TaskRipper to optimize the number of threads based on the number of iterations given,
        /// the execution range, and the maximum threads available
        /// and chooses how to balance the load of the task.
        /// <para>Choose this option if you do not understand the task load, and the number of threads you should use.</para>
        /// </summary>
        Optimize,

        /// <summary>
        /// Indicates to the <see cref="IWorkBalancer"/> to not balance the task.
        /// <para>
        /// Choose this option to iterate the task on a single thread.
        /// </para>
        /// </summary>
        None,

        /// <summary>
        /// Splits the iterations up in a way where the work is divided proportionally,
        /// onto the minimum number of threads available.
        /// <para>
        /// Choose this option to minimize concurrency of running the tasks by using the minimum number of threads 
        /// available defined in a work contract.
        /// </para>
        /// </summary>
        Min,

        /// <summary>
        /// Splits the iterations up in a way where the work is divided proportionally,
        /// onto the median number of threads available.
        /// <para>
        /// Choose this option to moderately run the tasks by using the median number of threads available defined in a work contract.
        /// </para>
        /// </summary>
        Medium,

        /// <summary>
        /// Split the iterations up in a way where the work is divided proportionally,
        /// onto the maxiumum number of threads available.
        /// <para>
        /// Choose this option to maximize concurrency of running the tasks by using all threads available defined in a work contract. 
        /// </para>
        /// </summary>
        High,

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

        /// <summary>
        /// 
        /// </summary>
        WorkBalancerOptions WorkBalancerOptions { get; }
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
                    defaultInstance = new ExecutionSettings(executionEnv, threadRange, executionRange, WorkBalancerOptions.Optimize);

                }

                return defaultInstance;
            }
        }

        public ExecutionSettings(IExecutionEnvironment executionEnvironment, Range threadRange, Range executionRange, WorkBalancerOptions workBalancerOptions)
        {
            ThreadRange = threadRange;
            ExecutionRange = executionRange;
            ExecutionEnvironment = executionEnvironment;
            WorkBalancerOptions = workBalancerOptions;
        }

        public ExecutionSettings(Range threadRange, Range executionRange, WorkBalancerOptions workBalancerOptions)
            : this(new ExecutionEnvironment(), threadRange, executionRange, workBalancerOptions)
        {
        }

        /// <inheritdoc/>
        public Range ThreadRange { get; }

        /// <inheritdoc/>
        public Range ExecutionRange { get; }

        /// <inheritdoc/>
        public IExecutionEnvironment ExecutionEnvironment { get; }

        public WorkBalancerOptions WorkBalancerOptions { get; }
    }
}
