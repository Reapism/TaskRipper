using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TaskRipper.Core.Tests.Unit
{
    public class WorkBalancerTests
    {
        [Theory]
        [InlineData(1, 1, 1, 1, 1)]
        [InlineData(1, 1, 2, 2, 2)]
        [InlineData(1, 1, 3, 3, 3)]
        [InlineData(2, 2, 3, 3, 1)]
        [InlineData(10, 10, 3, 3, 3)]
        public void OptimizeShouldReturnCorrectValues(int iterations, int expectedIterations, int minThreadCount, int maxThreadCount, int expectedNumberOfThreads)
        {
            var executionSettings = GetExecutionSettings(iterations, minThreadCount, maxThreadCount, WorkBalancerOptions.Optimize);
            var workContract = GetWorkContract(executionSettings, iterations);
            var actualIterationsByThread = GetWorkBalancer().Balance(workContract);

            if (iterations <= maxThreadCount)
                actualIterationsByThread.Count.Should().Be(iterations);
            else
                // Should be the same number of the number of threads.
                actualIterationsByThread.Count.Should().Be(expectedNumberOfThreads);

            int actualNumberOfIterationsAfterBalance = actualIterationsByThread.Values.As<IEnumerable<int>>().Sum();

            // Should be same number of iterations after balancing.
            actualNumberOfIterationsAfterBalance.Should().Be(expectedIterations);
        }

        [Theory]
        [InlineData(1, 1, 1, 1, 1)]
        [InlineData(1, 1, 1, 2, 1)]
        [InlineData(1, 1, 1, 3, 1)]
        [InlineData(2, 2, 1, 3, 1)]
        [InlineData(10, 10, 1, 3, 1)]
        public void NoneShouldReturnCorrectValues(int iterations, int expectedIterations, int minThreadCount, int maxThreadCount, int expectedNumberOfThreads)
        {
            var executionSettings = GetExecutionSettings(iterations, minThreadCount, maxThreadCount, WorkBalancerOptions.None);
            var workContract = GetWorkContract(executionSettings, iterations);
            var actualIterationsByThread = GetWorkBalancer().Balance(workContract);

            actualIterationsByThread.Count.Should().Be(iterations);
            int actualNumberOfIterationsAfterBalance = actualIterationsByThread.Values.As<IEnumerable<int>>().Sum();

            // Should be same number of iterations after balancing.
            actualNumberOfIterationsAfterBalance.Should().Be(expectedIterations);
        }

        // Private Setup Methods

        private static IExecutionSettings GetExecutionSettings(int iterations, int minThreadCount, int maxThreadCount, WorkBalancerOptions workBalancerOptions)
        {
            return new ExecutionSettings(new Range(minThreadCount, maxThreadCount), new Range(1, ++iterations), workBalancerOptions);
        }

        private IWorkBalancer workBalancer;
        private IWorkBalancer GetWorkBalancer()
        {
            if (workBalancer is null)
                workBalancer = new WorkBalancer();

            return workBalancer;
        }

        private IWorkContract GetWorkContract(IExecutionSettings executionSettings, int iterations)
        {
            return WorkContract.Create(executionSettings, "TEST", iterations);
        }
    }
}
