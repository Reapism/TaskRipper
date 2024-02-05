using FluentAssertions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TaskRipper.Core.Tests.Unit
{
    
    public class WorkContractBuilderTests
    {
        [Fact]
        public void WorkContractBuilderBuildsContractCorrectly()
        {
            // Arrange
            var iterations = 10;
            var request = 5; // Positive number to pass the conditional check
            var cancellationToken = CancellationToken.None;
            var workOptions = WorkBalancerOptions.Optimize;
            var contract = new WorkContractBuilder()
                .WithIterations(iterations)
                .WithCancellationToken(cancellationToken)
                .WithWorkBalancingOptions(workOptions)
                .UseDefaultExecutionSettings()
                .Build();

            // Act

            AssertContract(contract, iterations, cancellationToken, ExecutionSettings.Default, workOptions);

            var workBalancer = new WorkBalancer();
            var workThreads = workBalancer.Balance(contract);

            var result = WorkExecutor.Default.Execute(contract, ActionableString, request);

            // Assert

            result.ContractHonored.Should().BeTrue();
            result.Duration.Should().BeGreaterThan(TimeSpan.FromMicroseconds(1));
            result.ResultsMatrix.Should().NotBeEmpty();
            result.ThreadsUsed.Should().Be(workThreads.Count);
            result.HasCompleted.Should().BeTrue();
            result.OriginalContract.Should().Be(contract);
            
            var workResult = result as WorkResult<string>;

            workResult.Should().NotBeNull();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            workResult.TotalCount.Should().Be(iterations);
            workResult.PartitionCount.Should().Be(result.ThreadsUsed);
            workResult.PartitionCount.Should().Be(result.ResultsMatrix.Count());
#pragma warning restore CS8602 // Dereference of a possibly null reference.


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
        public void DelegateBuilder_ShouldThrowException_WhenConditionIsNotMet()
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
            var result = WorkExecutor.Default.Execute(contract, ActionableString, request);
        }

        private string ActionableString(int request)
        {
            return request.ToString();
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
