﻿using System.Threading.Tasks.Dataflow;

namespace TaskRipper.Core
{
    public delegate TResult Actionable<TRequest, TResult>(TRequest request);

    /// <summary>
    /// Represents the contract for executing work.
    /// </summary>
    public interface IWorkContract
    {
        /// <summary>
        /// The execution settings for this contract.
        /// </summary>
        IExecutionSettings ExecutionSettings { get; }

        /// <summary>
        /// The number of iterations requested for this contract.
        /// </summary>
        int IterationsRequested { get; }
        /// <summary>
        /// The options to describe to a <see cref="IWorkBalancer"/>
        /// how to balance the work for this contract.
        /// </summary>
        WorkBalancerOptions WorkBalancerOptions { get; }

        CancellationToken CancellationToken { get; }
    }

    public class WorkContract : IWorkContract, IEquatable<WorkContract>
    {
        private WorkContract(int iterationsRequested = 1)
            : this(Core.ExecutionSettings.Default, WorkBalancerOptions.Optimize, CancellationToken.None, iterationsRequested)
        { }
        private WorkContract(IExecutionSettings executionSettings, WorkBalancerOptions workBalancerOptions, CancellationToken cancellationToken, int iterationsRequested = 1)
        {
            ExecutionSettings = Guard.Against.Null(executionSettings);
            IterationsRequested = Guard.Against.NegativeOrZero(iterationsRequested);
            CancellationToken = cancellationToken;
            WorkBalancerOptions = workBalancerOptions;
        }

        /// <summary>
        /// The number of iterations requested to execute.
        /// </summary>
        public int IterationsRequested { get; }

        public IExecutionSettings ExecutionSettings { get; }

        public WorkBalancerOptions WorkBalancerOptions { get; }

        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Creates a work contract with default execution settings.
        /// <para>See <see cref="ExecutionSettings.Default"/></para> to get the default settings.
        /// </summary>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public static IWorkContract Create(int iterations)
        {
            var workContract = new WorkContract(iterations);
            return workContract;
        }

        public static IWorkContract Create([ValidatedNotNull, NotNull] IExecutionSettings executionSettings, WorkBalancerOptions workBalancerOptions, CancellationToken cancellationToken, int iterations)
        {
            var workContract = new WorkContract(executionSettings, workBalancerOptions, cancellationToken, iterations);
            return workContract;
        }

        public bool Equals(WorkContract? other)
        {
            if (other is null)
                return false;

            return IterationsRequested == other.IterationsRequested &&
                ExecutionSettings.Equals(other.ExecutionSettings) && 
                WorkBalancerOptions.Equals(other.WorkBalancerOptions) && 
                CancellationToken.Equals(other.CancellationToken);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (obj is WorkContract other)
                return Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IterationsRequested, ExecutionSettings.GetHashCode());
        }
    }
}
