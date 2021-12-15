using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TaskRipper.Core.Tests.Unit
{
    public class ActionTest
    {
        [Fact]
        public async Task ExecuteActionWithAction()
        {
            var actionContract = new ActionContract(ExecutionSettings, "Test", 1010);
            var executor = new ActionExecutorAsync(new WorkBalancer());
            var cancellationToken = new CancellationTokenSource().Token;
            var result = await executor.ExecuteAsync(actionContract, GetAction(), cancellationToken);

            result.ThreadsUsed.Should().Be(actionContract.ExecutionSettings.ExecutionEnvironment.ThreadCount);
            
        }

        private Action GetAction() => new Action(() => Console.Write(Random.Shared.Next(1).ToString()));
        private IExecutionSettings ExecutionSettings => new ExecutionSettings(new ExecutionEnvironment());}
    }