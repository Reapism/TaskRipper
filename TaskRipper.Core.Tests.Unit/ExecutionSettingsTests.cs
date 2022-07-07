﻿using FluentAssertions;
using System;
using Xunit;

namespace TaskRipper.Core.Tests.Unit
{
    public class ExecutionSettingsTests
    {
        [Theory]

        [InlineData(1, 10, 1, 10, 1, 10, 1, 10, WorkBalancerOptions.Optimize, WorkBalancerOptions.Optimize, true)]
        [InlineData(2, 10, 2, 10, 1, 10, 1, 10, WorkBalancerOptions.Optimize, WorkBalancerOptions.Optimize, true)]

        [InlineData(1, 10, 1, 1000, 1, 10, 1, 1000, WorkBalancerOptions.Optimize, WorkBalancerOptions.Optimize, false)]
        [InlineData(2, 10, 1, 1000, 1, 10, 1, 1000, WorkBalancerOptions.Optimize, WorkBalancerOptions.Optimize, false)]
        [InlineData(1, 10, 2, 1000, 1, 10, 1, 1000, WorkBalancerOptions.Optimize, WorkBalancerOptions.Optimize, false)]
        [InlineData(1, 10, 1, 1000, 2, 10, 1, 1000, WorkBalancerOptions.Optimize, WorkBalancerOptions.Optimize, false)]
        [InlineData(1, 10, 1, 1000, 1, 10, 2, 1000, WorkBalancerOptions.Optimize, WorkBalancerOptions.Optimize, false)]

        [InlineData(1, 10, 1, 1000, 1, 10, 1, 1000, WorkBalancerOptions.Optimize, WorkBalancerOptions.Medium, false)]
        [InlineData(2, 10, 1, 1000, 1, 10, 1, 1000, WorkBalancerOptions.Optimize, WorkBalancerOptions.Medium, false)]
        [InlineData(1, 10, 2, 1000, 1, 10, 1, 1000, WorkBalancerOptions.Optimize, WorkBalancerOptions.Medium, false)]
        [InlineData(1, 10, 1, 1000, 2, 10, 1, 1000, WorkBalancerOptions.Optimize, WorkBalancerOptions.Medium, false)]
        [InlineData(1, 10, 1, 1000, 1, 10, 2, 1000, WorkBalancerOptions.Optimize, WorkBalancerOptions.Medium, false)]

        [InlineData(1, 10, 1, 1000, 1, 10, 1, 1000, WorkBalancerOptions.Optimize, WorkBalancerOptions.Min, false)]
        public void ShouldBeEqual(int minThread1, int maxThread1, int minThread2, int maxThread2, int minExecution1, int maxExecution1, int minExecution2, int maxExecution2, WorkBalancerOptions workBalancerOptions1, WorkBalancerOptions workBalancerOptions2, bool shouldBeEqual)
        {
            IExecutionSettings firstSettings = new ExecutionSettings(GetExecutionEnvironment(), new Range(minThread1, maxThread1), new Range(minExecution1, maxExecution1), workBalancerOptions1);
            IExecutionSettings secondSettings = new ExecutionSettings(GetExecutionEnvironment(), new Range(minThread2, maxThread2), new Range(minExecution2, maxExecution2), workBalancerOptions2);

            if (shouldBeEqual)
                firstSettings.Should().Be(secondSettings);
            else
                firstSettings.Should().NotBe(secondSettings);
        }

        private IExecutionEnvironment GetExecutionEnvironment()
        {
            return new ExecutionEnvironment();
        }
    }
}
