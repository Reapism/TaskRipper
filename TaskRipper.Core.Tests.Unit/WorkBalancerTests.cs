using FluentAssertions;
using Xunit;

namespace TaskRipper.Core.Tests.Unit
{
    public class WorkBalancerTests
    {
        [Theory]
        [InlineData(1, 1, 1, 1)]
        [InlineData(1, 1, 2, 2)]
        [InlineData(1, 1, 3, 3)]
        [InlineData(2, 2, 3, 1)]
        [InlineData(10, 10, 3, 3)]
        public void BalanceShouldReturnCorrectValues(int iterations, int expectedIterations, int numberOfThreads, int expectedNumberOfThreads)
        {
            var workContract = WorkContract.Create("TEST", iterations);
            var actualIterationsByThread = GetWorkBalancer().Balance(workContract);

            if (iterations <= numberOfThreads)
                actualIterationsByThread.Count.Should().Be(iterations);
            else
                // Should be the same number of the number of threads.
                actualIterationsByThread.Count.Should().Be(expectedNumberOfThreads);

            int actualTotalCount = 0;
            foreach (var kvp in actualIterationsByThread)
            {
                actualTotalCount += kvp.Value;
            }

            // Should be same number of iterations after balancing.
            actualTotalCount.Should().Be(expectedIterations);

        }
        private IWorkBalancer workBalancer;
        private IWorkBalancer GetWorkBalancer()
        {
            if (workBalancer is null)
                workBalancer = new WorkBalancer();

            return workBalancer;
        }
    }
}
