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
            var executor = ActionExecutor.Default;
            var cancellationToken = new CancellationTokenSource().Token;
            var result = await executor.ExecuteAsync(actionContract, GetAction(), cancellationToken);

            result.ThreadsUsed.Should().Be(actionContract.ExecutionSettings.ExecutionEnvironment.ThreadCount);
        }

        [Fact]
        public async Task ExecuteActionCanBeCancelled()
        {
            // create a contract that should take very long.
            var actionContract = new WorkContract(GetExecutionSettings(), "Test", int.MaxValue);
            var executor = ActionExecutor.Default;
            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            var result = executor.ExecuteAsync(actionContract, GetAction(), cancellationToken);

            source.Cancel();

            result.IsCompleted.Should().BeFalse();
            result.IsCompletedSuccessfully.Should().BeFalse();
        }

        private Action GetAction() => new Action(() => testOutputHelper.WriteLine(Random.Shared.Next(2).ToString()));
        private IExecutionSettings GetExecutionSettings() => new ExecutionSettings(new ExecutionEnvironment());
    }
}