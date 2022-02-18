using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TaskRipper.Core.Tests.Unit
{
    public class ActionTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ActionTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task ExecuteActionWithAction()
        {
            var actionContract = new WorkContract(GetExecutionSettings(), "Test", 1010);
            var executor = WorkExecutor.Default;
            var cancellationToken = new CancellationTokenSource().Token;
            var result = await executor.ExecuteAsync(actionContract, GetAction(), cancellationToken);

            result.ThreadsUsed.Should().Be(actionContract.ExecutionSettings.ExecutionEnvironment.ThreadCount);
        }

        public async Task ExecuteCustomDelegateWithAction()
        {
            var actionContract = new WorkContract(GetExecutionSettings(), "Test", 1010);
            var executor = WorkExecutor.Default;
            var cancellationToken = new CancellationTokenSource().Token;
            var result = await executor.ExecuteAsync(actionContract, GetAction(), cancellationToken);

            result.ThreadsUsed.Should().Be(actionContract.ExecutionSettings.ExecutionEnvironment.ThreadCount);
        }

        [Fact]
        public async Task ExecuteActionCanBeCancelled()
        {
            // This test relies on timing of methods to recieve cancellation source.
            // Using as little resources as possible to confirm cancellation works across many threads.
            var actionContract = new WorkContract(GetExecutionSettings(), "Test", 100000);
            var executor = WorkExecutor.Default;
            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            
            var result = executor.ExecuteAsync(actionContract, GetAction(), cancellationToken);
            source.CancelAfter(5);
            await Task.Delay(30);

            result.Status.Should().Be(TaskStatus.Canceled);
            result.IsCompletedSuccessfully.Should().BeFalse();
            result.IsCanceled.Should().BeTrue();
        }

        private Action GetAction() => new Action(() => testOutputHelper.WriteLine(Random.Shared.Next(2).ToString()));
        private IExecutionSettings GetExecutionSettings()
        {
            return new ExecutionSettings(new Range(1, 8), new Range(1, int.MaxValue));
        }
    }
}