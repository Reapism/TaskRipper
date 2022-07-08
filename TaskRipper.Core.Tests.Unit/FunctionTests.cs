using FluentAssertions;
using System;
using System.Linq;
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
            Func<int, bool> func = GetIsNumberPrimeFunction();
            Action<int> mutatingStateMche = GetIncrementalMutatingStateMachine();
            var workFunction = new WorkFunction<int, bool>(contract, func, 1, GetIncrementalMutatingStateMachine(), false);
            var result = await executor.ExecuteAsync(workFunction, cancellationToken);

            // expectations
            result.Should().NotBeNull();
            result.ThreadsUsed.Should().Be(contract.ExecutionSettings.ExecutionEnvironment.ThreadCount);
            result.Duration.Should().BeGreaterThan(TimeSpan.FromTicks(1));
            result.Duration.Should().NotBe(default);
            result.WorkContract.Should().Be(contract);

            result.Results.Count().Should().Be(result.ThreadsUsed);
            result.Results.Sum(e => e.Count()).Should().Be(result.WorkContract.Iterations);
        }

        private Func<int, bool> GetIsNumberPrimeFunction()
            => number =>
            {
                if (number <= 1) return false;
                if (number == 2) return true;
                if (number % 2 == 0) return false;

                var boundary = (int)Math.Floor(Math.Sqrt(number));

                for (int i = 3; i <= boundary; i += 2)
                    if (number % i == 0)
                        return false;

                return true;
            };

        private Action<int> GetIncrementalMutatingStateMachine()
        {
            return (a) =>
            {
                a++;
            };
        }

        private IExecutionSettings GetExecutionSettings()
        {
            return new ExecutionSettings(ExecutionEnvironment.Create(1), new Range(1, 1), new Range(1, int.MaxValue), WorkBalancerOptions.Optimize);
        }

    }
}