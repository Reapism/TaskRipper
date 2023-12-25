using System;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace TaskRipper.Core.Tests.Unit
{
    
    public class DelegateBuilderTests
    {
        [Fact]
        public void DelegateBuilder_ShouldExecuteAllActions_WhenConditionIsMet()
        {
            // Arrange
            var preActionCalled = false;
            var postActionCalled = false;
            var executingFunctionResult = "Not Executed";
            var request = 5; // Positive number to pass the conditional check

            var builder = new DelegateBuilder<int, string>()
                .WithPreAction(() => { request++; preActionCalled = true; })
                .WithExecutingFunction(request => {
                    executingFunctionResult = $"Processed {request}";
                    return executingFunctionResult;
                })
                .WithPostAction(() => { request++; postActionCalled = true; })
                .WithConditionalAction(request => request > 0)
                .WithCancellationToken(CancellationToken.None);

            var delegateToTest = builder.Build();

            // Act
            var result = delegateToTest(request, CancellationToken.None);

            // Assert
            Assert.True(preActionCalled, "Pre-action was called.");
            Assert.Equal("Processed 5", result);
            Assert.True(postActionCalled, "Post-action was called.");
        }

        [Fact]
        public void DelegateBuilder_ShouldThrowException_WhenConditionIsNotMet()
        {
            // Arrange
            var request = -1; // Negative number to fail the conditional check
            var builder = new DelegateBuilder<int, string>()
                .WithExecutingFunction(request => $"Processed {request}")
                .WithConditionalAction(request => request > 0)
                .WithCancellationToken(CancellationToken.None);

            var delegateToTest = builder.Build();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => delegateToTest(request, CancellationToken.None));
            Assert.Equal("Conditional action failed.", exception.Message);
        }
    }
}
