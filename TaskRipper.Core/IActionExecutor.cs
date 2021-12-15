using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskRipper.Core
{

    public interface IActionExecutor
    {
        IWorkerResult Execute(IWorkContract actionContract, Action action, CancellationToken cancellationToken);
    }
    public interface IActionExecutorAsync
    {
        Task<IWorkerResult> ExecuteAsync(IWorkContract actionContract, Action action, CancellationToken cancellationToken);
    }

    public class ActionExecutorAsync : IActionExecutorAsync
    {
        private readonly IWorkBalancer workBalancer;

        public ActionExecutorAsync(IWorkBalancer workBalancer)
        {
            this.workBalancer = workBalancer;
        }
        public async Task<IWorkerResult> ExecuteAsync(IWorkContract actionContract, Action action, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncInternal(actionContract, action, cancellationToken);
        }

        private async Task<IWorkerResult> ExecuteAsyncInternal(IWorkContract actionContract, Action action, CancellationToken cancellationToken)
        {
            if (actionContract is null)
            {
                await Task.FromException(new ArgumentNullException(nameof(actionContract)));
            }
            // 10,000 ITERATIONS
            // 10 THREADS
            // HOW MANY ITERATIONS PER THREAD?

            var iterationsByThread = workBalancer.Balance(actionContract.Iterations, actionContract.ExecutionSettings.ExecutionEnvironment.ThreadCount);
            var tasks = GetTasks(action, iterationsByThread.Count, cancellationToken);
            
            await ExecuteTasks(tasks);
            return await Task.FromResult(new WorkerResult(actionContract, tasks.Count()));
        }

        private async Task ExecuteTasks(IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
                task.Start();

            await Task.WhenAll(tasks);
        }

        private IEnumerable<Task> GetTasks(Action action, int count, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < count; i++)
            {
                tasks.Add(new Task(action, cancellationToken, TaskCreationOptions.LongRunning));
            }

            return tasks;
        }
    }
}
