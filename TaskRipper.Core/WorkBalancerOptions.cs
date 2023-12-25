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
        MinimizeThreads,

        /// <summary>
        /// Split the iterations up in a way where the work is divided proportionally,
        /// onto the maxiumum number of threads available.
        /// <para>
        /// Choose this option to maximize concurrency of running the tasks by using all threads available defined in a work contract. 
        /// </para>
        /// </summary>
        MaximizeThreads,

    }
}
