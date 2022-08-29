using FluentAssertions;
using System.Threading;
using Xunit;

namespace TaskRipper.Core.Tests.Unit
{
    public class ExecutionEnvironmentTests
    {
        [Fact]
        public void FromThreadPool()
        {
            IExecutionEnvironment actualThreadPoolCount = ExecutionEnvironment.FromThreadPool();
            ThreadPool.GetAvailableThreads(out int expectedWorkerThreads, out int b);
            actualThreadPoolCount.Should().Be(expectedWorkerThreads);
        }
    }
}
