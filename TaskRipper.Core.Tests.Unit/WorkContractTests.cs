using FluentAssertions;
using System;
using System.Diagnostics.Contracts;
using System.Threading;
using Xunit;

namespace TaskRipper.Core.Tests.Unit
{
    public class WorkContractTests
    {
        [Theory]
        [InlineData(1, 1, WorkBalancerOptions.None, WorkBalancerOptions.None, true)]
        [InlineData(1, 1, WorkBalancerOptions.None, WorkBalancerOptions.Optimize, false)]
        [InlineData(1, 2, WorkBalancerOptions.None, WorkBalancerOptions.None, false)]
        [InlineData(2, 2, WorkBalancerOptions.None, WorkBalancerOptions.None, true)]
        [InlineData(2, 2, WorkBalancerOptions.None, WorkBalancerOptions.Optimize, false)]
        [InlineData(2, 1, WorkBalancerOptions.None, WorkBalancerOptions.None, false)]
        public void ShouldBeEqual(int iter1, int iter2, WorkBalancerOptions opt1, WorkBalancerOptions opt2, bool shouldBeEqual)
        {
            var cancellationToken = CancellationToken.None;


            var firstContract = new WorkContractBuilder()
                .WithExecutionSettings(GetExecutionSettings())
                .WithWorkBalancingOptions(opt1)
                .WithCancellationToken(cancellationToken)
                .WithIterations(iter1)
                .Build();
            var secondContract = new WorkContractBuilder()
                .WithExecutionSettings(GetExecutionSettings())
                .WithWorkBalancingOptions(opt2)
                .WithCancellationToken(cancellationToken)
                .WithIterations(iter2)
                .Build();

            firstContract.Should().NotBeNull();
            secondContract.Should().NotBeNull();


            if (shouldBeEqual)
                firstContract.Should().Be(secondContract);
            else
                firstContract.Should().NotBe(secondContract);

            firstContract.IterationsRequested.Should().Be(iter1);
            firstContract.ExecutionSettings.Should().Be(ExecutionSettings.Default);
            firstContract.ExecutionSettings.Environment.Should().Be(ExecutionSettings.Default.Environment);
            firstContract.WorkBalancerOptions.Should().Be(opt1);

            secondContract.IterationsRequested.Should().Be(iter2);
            secondContract.ExecutionSettings.Should().Be(ExecutionSettings.Default);
            secondContract.ExecutionSettings.Environment.Should().Be(ExecutionSettings.Default.Environment);
            secondContract.WorkBalancerOptions.Should().Be(opt2);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ThrowsOutOfRange(int iter)
        {
            Assert.Throws<ArgumentException>(() => WorkContract.Create(iter));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(int.MaxValue)]
        public void DoesNotThrowOutOfRange(int iter)
        {
            var contract = WorkContract.Create(iter);
            contract.Should().NotBeNull();
            contract.IterationsRequested.Should().Be(iter);
            contract.ExecutionSettings.Should().Be(ExecutionSettings.Default);
            contract.ExecutionSettings.Environment.Should().Be(ExecutionSettings.Default.Environment);
        }

        private IExecutionSettings GetExecutionSettings()
        {
            return ExecutionSettings.Default;
        }
    }
}
