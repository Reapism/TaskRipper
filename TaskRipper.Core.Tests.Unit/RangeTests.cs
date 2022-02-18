using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TaskRipper.Core.Tests.Unit
{
    public class RangeTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public RangeTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        //[InlineData(1, 10, 11, true)]
        [InlineData(1, 10, 2, true)]
        [InlineData(1, 10, 10, false)] // exclusive
        [InlineData(1, 10, 9, true)]
        [InlineData(5, 10, 1, false)]
        [InlineData(5, 10, 5, true)]
        [InlineData(5, 10, 4, false)]

        public void IsInRange(int min, int max, int value, bool isInRange)
        {
            var range = new Range(min, max);
            var actualIsInRange = range.IsInRange(value);
            isInRange.Should().Be(actualIsInRange);
        }
    }
}
