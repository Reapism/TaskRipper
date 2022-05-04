using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TaskRipper.Core.Tests.Unit
{
    public class FunctionTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public FunctionTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task ExecuteFuncShouldExecuteWithExpectations()
        {
            var contract = WorkContract.Create(GetExecutionSettings(), "TEST", 1000);
            var executor = WorkExecutor.Default;
            var cancellationToken = new CancellationTokenSource().Token;
            var result = await executor.ExecuteAsync(contract, IsNumberPrime(), 1, cancellationToken);
            
            result.Results.Count.Should().Be(1000);
            // expectations
            result.ThreadsUsed.Should().Be(contract.ExecutionSettings.ExecutionEnvironment.ThreadCount);
        }

        private Action PrintOnesAndZeros() => new Action(() => testOutputHelper.WriteLine(Random.Shared.Next(2).ToString()));
        private Func<int, bool> IsNumberPrime()
            => new Func<int, bool>((number) =>
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        });


        private IExecutionSettings GetExecutionSettings()
        {
            return new ExecutionSettings(new Range(1, 8), new Range(1, int.MaxValue), WorkBalancerOptions.Optimize);
        }

    }
}