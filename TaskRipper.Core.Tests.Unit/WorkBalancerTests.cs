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
        [InlineData(1, 1, 1, 1)]
        [InlineData(1, 2, 2, 1)]
        [InlineData(1, 3, 3, 1)]
        [InlineData(2, 3, 3, 2)]
        [InlineData(10, 3, 3, 3)]
        [InlineData(100, 3, 3, 3)]
        [InlineData(1000, 3, 3, 3)]
        [InlineData(10000, 3, 3, 3)]
        [InlineData(10, 1, 8, 8)]
        [InlineData(100, 1, 8, 8)]
        [InlineData(1000, 1, 8, 8)]
        [InlineData(10000, 1, 8, 8)]
        [InlineData(10, 1, 16, 10)]
        [InlineData(100, 1, 16, 16)]
        [InlineData(1000, 1, 16, 16)]
        [InlineData(10000, 1, 16, 16)]
        public void OptimizeShouldReturnCorrectValues(int iterations, int minThreadCount, int maxThreadCount, int expectedNumberOfThreads)
        {
            var executionSettings = GetExecutionSettings(iterations, minThreadCount, maxThreadCount, WorkBalancerOptions.Optimize);
            var workContract = GetWorkContract(executionSettings, iterations);
            var actualIterationsByThread = new WorkBalancer().Balance(workContract);

            var actualNumberOfThreads = actualIterationsByThread.Count;
            actualNumberOfThreads.Should().Be(expectedNumberOfThreads);

            // Should be same number of iterations after balancing.
            int actualNumberOfIterationsAfterBalance = actualIterationsByThread.Values.As<IEnumerable<int>>().Sum();
            actualNumberOfIterationsAfterBalance.Should().Be(iterations);

        }

        [Theory]
        [InlineData(1, 1, 1, 1)]
        [InlineData(1, 1, 2, 1)]
        [InlineData(1, 1, 3, 1)]
        [InlineData(2, 1, 3, 1)]
        [InlineData(10, 1, 3, 1)]
        [InlineData(10, 1, 1, 1)]
        [InlineData(100, 1, 1, 1)]
        [InlineData(1000, 1, 1, 1)]
        [InlineData(10000, 1, 1, 1)]
        [InlineData(10, 1, 2, 1)]
        [InlineData(100, 1, 2, 1)]
        [InlineData(1000, 1, 2, 1)]
        [InlineData(10000, 1, 2, 1)]
        [InlineData(10, 1, 4, 1)]
        [InlineData(100, 1, 4, 1)]
        [InlineData(1000, 1, 4, 1)]
        [InlineData(10000, 1, 4, 1)]
        [InlineData(10, 2, 3, 1)]
        [InlineData(100, 2, 3, 1)]
        [InlineData(1000, 2, 3, 1)]
        [InlineData(10000, 2, 3, 1)]
        public void NoneShouldReturnCorrectValues(int iterations, int minThreadCount, int maxThreadCount, int expectedNumberOfThreads)
        {
            var executionSettings = GetExecutionSettings(iterations, minThreadCount, maxThreadCount, WorkBalancerOptions.None);
            var workContract = GetWorkContract(executionSettings, iterations);
            var actualIterationsByThread = new WorkBalancer().Balance(workContract);

            // number of threads after balancing should match expectation
            var actualNumberOfThreads = actualIterationsByThread.Count;
            actualNumberOfThreads.Should().Be(expectedNumberOfThreads);
            
            int actualNumberOfIterationsAfterBalance = actualIterationsByThread.Values.As<IEnumerable<int>>().Sum();
            actualNumberOfIterationsAfterBalance.Should().Be(iterations);
        }

        [Theory]
        [InlineData(1, 1, 1, 1)]
        [InlineData(1, 2, 2, 1)]
        [InlineData(1, 3, 3, 1)]
        [InlineData(2, 3, 3, 2)]
        [InlineData(10, 3, 3, 3)]
        [InlineData(100, 3, 3, 3)]
        [InlineData(1000, 3, 3, 3)]
        [InlineData(10000, 3, 3, 3)]
        [InlineData(10, 1, 8, 1)]
        [InlineData(100, 1, 8, 1)]
        [InlineData(1000, 1, 8, 1)]
        [InlineData(10000, 1, 8, 1)]
        [InlineData(10, 1, 16, 1)]
        [InlineData(100, 1, 1, 1)]
        [InlineData(1000, 1, 16, 1)]
        [InlineData(10000, 1, 16, 1)]
        [InlineData(20, 2, 2, 2)]
        [InlineData(200, 2, 2, 2)]
        [InlineData(2000, 2, 3, 2)]
        [InlineData(20000, 2, 3, 2)]
        [InlineData(20, 2, 8, 2)]
        [InlineData(200, 2, 8, 2)]
        [InlineData(2000, 2, 8, 2)]
        [InlineData(20000, 2, 8, 2)]
        [InlineData(20, 160, 320, 20)]
        [InlineData(200, 160, 320, 160)]
        [InlineData(2000, 160, 320, 160)]
        [InlineData(20000, 160, 320, 160)]
        public void MinShouldReturnCorrectValues(int iterations, int minThreadCount, int maxThreadCount, int expectedNumberOfThreads)
        {
            var executionSettings = GetExecutionSettings(iterations, minThreadCount, maxThreadCount, WorkBalancerOptions.MinimizeThreads);
            var workContract = GetWorkContract(executionSettings, iterations);
            var actualIterationsByThread = new WorkBalancer().Balance(workContract);

            var actualNumberOfThreads = actualIterationsByThread.Count;
            actualNumberOfThreads.Should().Be(expectedNumberOfThreads);

            // Should be same number of iterations after balancing.
            int actualNumberOfIterationsAfterBalance = actualIterationsByThread.Values.As<IEnumerable<int>>().Sum();
            actualNumberOfIterationsAfterBalance.Should().Be(iterations);
        }


        [Theory]
        [InlineData(1, 1, 1, 1)]
        [InlineData(1, 2, 2, 1)]
        [InlineData(1, 3, 3, 1)]
        [InlineData(2, 3, 3, 2)]
        [InlineData(10, 3, 3, 3)]
        [InlineData(100, 3, 3, 3)]
        [InlineData(1000, 3, 3, 3)]
        [InlineData(10000, 3, 3, 3)]
        [InlineData(10, 1, 8, 8)]
        [InlineData(100, 1, 8, 8)]
        [InlineData(1000, 1, 8, 8)]
        [InlineData(10000, 1, 8, 8)]
        [InlineData(10, 1, 16, 10)]
        [InlineData(100, 1, 1, 1)]
        [InlineData(1000, 1, 16, 16)]
        [InlineData(10000, 1, 16, 16)]
        [InlineData(20, 2, 2, 2)]
        [InlineData(200, 2, 2, 2)]
        [InlineData(2000, 2, 3, 3)]
        [InlineData(20000, 2, 3, 3)]
        [InlineData(20, 2, 8, 8)]
        [InlineData(200, 2, 8, 8)]
        [InlineData(2000, 2, 8, 8)]
        [InlineData(20000, 2, 8, 8)]
        [InlineData(20, 160, 320, 20)]
        [InlineData(200, 160, 320, 200)]
        [InlineData(2000, 160, 320, 320)]
        [InlineData(20000, 160, 320, 320)]
        public void HighShouldReturnCorrectValues(int iterations, int minThreadCount, int maxThreadCount, int expectedNumberOfThreads)
        {
            var executionSettings = GetExecutionSettings(iterations, minThreadCount, maxThreadCount, WorkBalancerOptions.MaximizeThreads);
            var workContract = GetWorkContract(executionSettings, iterations);
            var actualIterationsByThread = new WorkBalancer().Balance(workContract);

            var actualNumberOfThreads = actualIterationsByThread.Count;
            actualNumberOfThreads.Should().Be(expectedNumberOfThreads);

            // Should be same number of iterations after balancing.
            int actualNumberOfIterationsAfterBalance = actualIterationsByThread.Values.As<IEnumerable<int>>().Sum();
            actualNumberOfIterationsAfterBalance.Should().Be(iterations);
        }

        // Private Setup Methods

        private static IExecutionSettings GetExecutionSettings(int iterations, int minThreadCount, int maxThreadCount, WorkBalancerOptions workBalancerOptions)
        {
            return ExecutionSettings.Create(LocalExecutionEnvironment.Default, new Range(minThreadCount, maxThreadCount), new Range(1, ++iterations), workBalancerOptions);
        }

        private IWorkContract GetWorkContract(IExecutionSettings executionSettings, int iterations)
        {
            return WorkContract.Create(executionSettings, iterations);
        }
    }
}
