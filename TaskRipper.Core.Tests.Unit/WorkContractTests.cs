using FluentAssertions;
using System;
using System.Diagnostics.Contracts;
using Xunit;

namespace TaskRipper.Core.Tests.Unit
{
    public class WorkContractTests
    {
        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(1, 2, false)]
        [InlineData(2, 2, true)]
        [InlineData(2, 1, false)]
        public void ShouldBeEqual(int iter1, int iter2, bool shouldBeEqual)
        {
            IWorkContract firstContract = WorkContract.Create(GetExecutionSettings(), iter1);
            IWorkContract secondContract = WorkContract.Create(GetExecutionSettings(), iter2);

            firstContract.Should().NotBeNull();
            secondContract.Should().NotBeNull();


            if (shouldBeEqual)
                firstContract.Should().Be(secondContract);
            else
                firstContract.Should().NotBe(secondContract);

            firstContract.IterationsRequested.Should().Be(iter1);
            firstContract.ExecutionSettings.Should().Be(ExecutionSettings.Default);
            firstContract.ExecutionSettings.ExecutionEnvironment.Should().Be(ExecutionSettings.Default.ExecutionEnvironment);


            secondContract.IterationsRequested.Should().Be(iter2);
            secondContract.ExecutionSettings.Should().Be(ExecutionSettings.Default);
            secondContract.ExecutionSettings.ExecutionEnvironment.Should().Be(ExecutionSettings.Default.ExecutionEnvironment);
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
            contract.ExecutionSettings.ExecutionEnvironment.Should().Be(ExecutionSettings.Default.ExecutionEnvironment);
        }

        private IExecutionSettings GetExecutionSettings()
        {
            return ExecutionSettings.Default;
        }
    }
}
