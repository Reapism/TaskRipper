using FluentAssertions;
using Xunit;

namespace TaskRipper.Core.Tests.Unit
{
    public class WorkContractTests
    {
        [Theory]
        [InlineData("", "", 1, 1, true)]
        [InlineData("a", "a", 1, 1, true)]
        [InlineData("a", "A", 1, 1, false)]
        [InlineData("a", "A", 1, 0, false)]
        [InlineData("a", "a", 1, 0, false)]
        public void ShouldBeEqual(string desc1, string desc2, int iter1, int iter2, bool shouldBeEqual)
        {
            IWorkContract firstContract = WorkContract.Create(GetExecutionSettings(), desc1, iter1);
            IWorkContract secondContract = WorkContract.Create(GetExecutionSettings(), desc2, iter2);

            if (shouldBeEqual)
                firstContract.Should().Be(secondContract);
            else
                firstContract.Should().NotBe(secondContract);
        }

        private IExecutionSettings GetExecutionSettings()
        {
            return ExecutionSettings.Default;
        }
    }
}
