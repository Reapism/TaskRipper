using FluentAssertions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TaskRipper.Core.Tests.Unit
{
    
    public class WorkContractBuilderTests(ITestOutputHelper testOutputHelper)
    {
        [Theory]
        [InlineData(1, 5, 1)]
        [InlineData(10, 5, 10)]
        [InlineData(32, 5, 32)]
        [InlineData(100, 5, 32)]
        [InlineData(50000, 5, 32)]
        [InlineData(100000, 5, 32)]
        [InlineData(1000000, 5, 32)]
        [InlineData(2147483646, 5, 32)]
        public async Task WorkContractBuilderBuildsContractCorrectlyAndExecutes(int iterations, int request, int expectedNumOfThreadsUsed)
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var workOptions = WorkBalancerOptions.Optimize;
            var exSettings = ExecutionSettings.Create(LocalExecutionEnvironment.Default, new Range(1, 32), new Range(1, iterations + 1));
            var contract = new WorkContractBuilder()
                .WithIterations(iterations)
                .WithCancellationToken(cancellationToken)
                .WithWorkBalancingOptions(workOptions)
                .WithExecutionSettings(exSettings)
                .Build();

            // Act

            AssertContract(contract, iterations, cancellationToken, exSettings, workOptions);

            var result = await WorkExecutor.Default.ExecuteAsync(contract, Add, request);

            // Assert

            result.ContractHonored.Should().BeTrue();
            result.Duration.Should().BeGreaterThan(TimeSpan.FromMicroseconds(1));
            result.ResultsMatrix.Should().NotBeEmpty();
            result.ThreadsUsed.Should().Be(expectedNumOfThreadsUsed);
            result.HasCompleted.Should().BeTrue();
            result.OriginalContract.Should().Be(contract);
            
            var workResult = result as WorkResult<int>;

            workResult.Should().NotBeNull();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            workResult.TotalCount.Should().Be(iterations);
            workResult.PartitionCount.Should().Be(result.ThreadsUsed);
            workResult.PartitionCount.Should().Be(result.ResultsMatrix.Count());
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            testOutputHelper.WriteLine($"Duration: {workResult.Duration}");
        }

        private void AssertContract(IWorkContract workContract, int iterations, CancellationToken cancellationToken, IExecutionSettings executionSettings, WorkBalancerOptions workBalancerOptions)
        {
            workContract.Should().NotBeNull();
            workContract.IterationsRequested.Should().Be(iterations);
            workContract.WorkBalancerOptions.Should().Be(workBalancerOptions);
            workContract.ExecutionSettings.Should().Be(executionSettings);
            workContract.CancellationToken.Should().Be(cancellationToken);
        }

        [Fact]
        public async Task DelegateBuilder_ShouldThrowException_WhenConditionIsNotMet()
        {
            // Arrange
            var iterations = 10;
            var request = 5; // Positive number to pass the conditional check
            var contract = new WorkContractBuilder()
                .WithIterations(iterations)
                .WithCancellationToken(CancellationToken.None)
                .UseDefaultExecutionSettings()
                .Build();

            // Act & Assert
            var result = await WorkExecutor.Default.ExecuteAsync(contract, ActionableString, request);
        }

        private int Add(int request)
        {
            return request + 1;
        }

        private string ActionableString(int request)
        {
            return (request + 1).ToString();
        }

        private int ActionableInt(string request)
        {
            return Convert.ToInt32(request);
        }

        private async Task<int> ActionableIntAsync(string request)
        {
            await Task.Delay(100);
            return Convert.ToInt32(request);
        }

        private async Task<string> ActionableStringAsync(int request)
        {
            await Task.Delay(100);
            return request.ToString();
        }
    }
}
