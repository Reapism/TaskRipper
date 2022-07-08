using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TaskRipper.Core.Tests.Unit
{
    public class ActionTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ActionTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task ExecuteActionWithAction()
        {
            var actionContract = new WorkContract(GetExecutionSettings(), "Test", 1010);
            var executor = WorkExecutor.Default;
            var cancellationToken = new CancellationTokenSource().Token;
            var workAction = new WorkAction(actionContract, () => PrintOnesAndZeros());
            var result = await executor.ExecuteAsync(workAction, cancellationToken);

            result.Should().NotBeNull();
            result.ThreadsUsed.Should().Be(actionContract.ExecutionSettings.ExecutionEnvironment.ThreadCount);
            result.Duration.Should().BeGreaterThan(TimeSpan.FromTicks(1));
            result.WorkContract.Should().Be(actionContract);
        }

        [Fact]
        public async Task ExecuteActionWithActionInt()
        {
            var actionContract = new WorkContract(GetExecutionSettings(), "Test", 1010);
            var executor = WorkExecutor.Default;
            var cancellationToken = new CancellationTokenSource().Token;
            var workAction = new WorkAction<int>(actionContract, (a) => Print1ToA(), 1, null, false);
            var result = await executor.ExecuteAsync(workAction, cancellationToken);

            result.Should().NotBeNull();
            result.ThreadsUsed.Should().Be(actionContract.ExecutionSettings.ExecutionEnvironment.ThreadCount);
            result.Duration.Should().BeGreaterThan(TimeSpan.FromTicks(1));
            result.WorkContract.Should().Be(actionContract);
        }

        [Fact]
        public async Task ExecuteActionWithActionIntInterlocked()
        {
            var actionContract = new WorkContract(GetExecutionSettings(), "Test", 1010);
            var executor = WorkExecutor.Default;
            var cancellationToken = new CancellationTokenSource().Token;
            var workAction = new WorkAction<int>(actionContract, (a) => Print1ToAInterlocked(), 1, null, false);
            var result = await executor.ExecuteAsync(workAction, cancellationToken);

            result.Should().NotBeNull();
            result.ThreadsUsed.Should().Be(actionContract.ExecutionSettings.ExecutionEnvironment.ThreadCount);
            result.Duration.Should().BeGreaterThan(TimeSpan.FromTicks(1));
            result.WorkContract.Should().Be(actionContract);
        }

        public async Task ExecuteCustomDelegateWithAction()
        {
            var actionContract = new WorkContract(GetExecutionSettings(), "Test", 1010);
            var executor = WorkExecutor.Default;
            var cancellationToken = new CancellationTokenSource().Token;
            var workAction = new WorkAction(actionContract, () => Print1ToAInterlocked());
            var result = await executor.ExecuteAsync(workAction, cancellationToken);

            result.ThreadsUsed.Should().Be(actionContract.ExecutionSettings.ExecutionEnvironment.ThreadCount);
        }

        [Theory]
        [InlineData(8)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        //[InlineData(100000, Skip = "Inconclusive, works by itself, not running tests in group")]
        //[InlineData(1000000, Skip = "Inconclusive, works by itself, not running tests in group")]
        public async Task ExecuteActionCanBeCancelled(int iterations)
        {
            // This test relies on timing of methods to receive cancellation source.
            // Using as little resources as possible to confirm cancellation works across many threads.
            var actionContract = new WorkContract(GetExecutionSettings(), "Test", iterations);
            var executor = WorkExecutor.Default;
            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            var workAction = new WorkAction(actionContract, () => PrintOnesAndZeros());
            var result = executor.ExecuteAsync(workAction, cancellationToken);
            source.CancelAfter(10);
            await Task.Delay(10);

            
            if (result.IsCompleted)
            {
                result.IsCompleted.Should().BeTrue();
                result.IsCompletedSuccessfully.Should().BeTrue();
                result.Status.Should().Be(TaskStatus.RanToCompletion);
            }
            else
            {
                // This is testing if its incomplete, the task can be cancelled or waiting for activation or other states.
                // but definitely not ran to completion
                result.IsCompletedSuccessfully.Should().BeFalse();
                result.IsCanceled.Should().BeTrue();
                result.Status.Should().NotBe(TaskStatus.RanToCompletion);
            }
        }

        private Action PrintOnesAndZeros() => new Action(() => testOutputHelper.WriteLine(Random.Shared.Next(2).ToString()));
        private Action<int> Print1ToA() => new Action<int>((a) =>
        {
            testOutputHelper.WriteLine(Random.Shared.Next(a).ToString());
            a++;
        });

        private Action<int> Print1ToAInterlocked() => new Action<int>((a) =>
        {
            testOutputHelper.WriteLine(Random.Shared.Next(a).ToString());
            Interlocked.Increment(ref a);
        });
        private IExecutionSettings GetExecutionSettings()
        {
            return new ExecutionSettings(new Range(1, 8), new Range(1, int.MaxValue), WorkBalancerOptions.Optimize);
        }

    }
}