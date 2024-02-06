using System.Threading;

namespace TaskRipper.Core
{
    public interface IWorkExecutor
    {
        IWorkResult<TResult> Execute<TRequest, TResult>(IWorkContract contract, Actionable<TRequest, TResult> actionable, TRequest request);
        Task<IWorkResult<TResult>> ExecuteAsync<TRequest, TResult> (IWorkContract contract, Actionable<TRequest, TResult> actionable, TRequest request);
    }

    public sealed class WorkExecutor : IWorkExecutor
    {
        private readonly IWorkBalancer workBalancer;
        private static IWorkExecutor? _defaultInstance;

        private bool ResultIsTask;

        private WorkExecutor(IWorkBalancer workBalancer)
        {
            this.workBalancer = workBalancer;
        }

        public static IWorkExecutor Default
        {
            get
            {
                return _defaultInstance ??= new WorkExecutor(new WorkBalancer());
            }
        }

        public IWorkResult<TResult> Execute<TRequest, TResult>(IWorkContract contract, Actionable<TRequest, TResult> actionable, TRequest request)
        {
            throw new NotImplementedException("Use Async version for now");
        }

        public Task<IWorkResult<TResult>> ExecuteAsync<TRequest, TResult>(IWorkContract contract, Actionable<TRequest, TResult> actionable, TRequest request)
        {
            return ExecuteAsyncInternal<TRequest, TResult>(contract, actionable, request);
        }

        private bool IsResultATaskType<TResult>()
        {
            var resultType = typeof(TResult);
            var isTask = resultType == typeof(Task);
            var isGenericTask = resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Task<>);

            return isTask || isGenericTask;
        }

        private async Task<IWorkResult<TResult>> ExecuteInternal<TRequest, TResult>(IWorkContract contract, Actionable<TRequest, TResult> actionable, TRequest request)
        {
            ResultIsTask = IsResultATaskType<TResult>();

            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(contract);
            var tasks = await WrapTasks(contract, actionable, request, iterationsByThread);

            StartTasks(tasks);
            var resultMatrix = await WaitForTasks(tasks);
            HandleIncompleteTasks(tasks);

            var endDate = DateTime.Now;
            var workResult = new WorkResult<TResult>()
            {
                Duration = endDate - startDate,
                OriginalContract = contract,
                ThreadsUsed = iterationsByThread.Count,
                ResultsMatrix = resultMatrix
            };

            return await Task.FromResult(workResult);
        }

        private async Task<IWorkResult<TResult>> ExecuteAsyncInternal<TRequest, TResult>(IWorkContract contract, Actionable<TRequest, TResult> actionable, TRequest request)
        {
            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(contract);
            var tasks = await WrapTasks(contract, actionable, request, iterationsByThread);

            StartTasks(tasks);

            var executerTask = WaitForTasks(tasks);
            var resultMatrix = await executerTask;

            HandleIncompleteTasks(tasks);
            var endDate = DateTime.Now;

            var workResult = new WorkResult<TResult>()
            {
                Duration = endDate - startDate,
                OriginalContract = contract,
                ThreadsUsed = iterationsByThread.Count,
                ResultsMatrix = resultMatrix,
                ExecuterTask = executerTask
            };

            return await Task.FromResult(workResult);
        }  

        private static void HandleIncompleteTasks(IEnumerable<Task> tasks)
        {
            if (tasks.Any(e => e.IsCanceled || !e.IsCompletedSuccessfully || e.Exception is not null))
            {
                // At this point, the task was cancelled either due to an exception
                // TODO handle each case differently, for now, it will just fail

                var exceptionalTasks = tasks.Where(e => e.Exception is not null).ToArray();
                var cancelledTasks = tasks.Where(e => e.IsCanceled).ToArray();
                var incompleteTasks = tasks.Where(e => !e.IsCompletedSuccessfully).ToArray();
            }
        }

        private async Task<Queue<Task<IEnumerable<TResult>>>> WrapTasks<TRequest, TResult>(IWorkContract contract, Actionable<TRequest, TResult> actionable, TRequest request, IDictionary<int, int> iterationsByThread)
        {
            return await WrapTasksInternal(contract, actionable, request, iterationsByThread);
        }


        private async Task<Queue<Task<IEnumerable<TResult>>>> WrapTasksInternal<TRequest, TResult>(IWorkContract contract, Actionable<TRequest, TResult> actionable, TRequest request, IDictionary<int, int> iterationsByThread)
        {
            var queue = new Queue<Task<IEnumerable<TResult>>>();

            for(var i = 0; i < iterationsByThread.Count; i++)
            {
                var iterationsForThisThread = iterationsByThread[i];
                if (contract.CancellationToken == CancellationToken.None)
                {
                    queue.Enqueue(GetTaskWithoutCancellationToken(actionable, request, iterationsForThisThread));
                    continue;
                }

                queue.Enqueue(GetTaskWithCancellationToken(actionable, request, iterationsForThisThread, contract.CancellationToken));
            }

            return queue;
        }  

        private Task<IEnumerable<TResult>> GetTaskWithCancellationToken<TRequest, TResult>(Actionable<TRequest, TResult> actionable, TRequest request, int iterationsForThisThread, CancellationToken cancellationToken)
        {
            return new Task<IEnumerable<TResult>>((r) =>
            {
                var resultQueue = new Queue<TResult>(iterationsForThisThread);
                for(int i = 0; i < iterationsForThisThread; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var result = actionable(request);
                    resultQueue.Enqueue(result);
                }
                return resultQueue;
            }, request, TaskCreationOptions.LongRunning);
        }

        private Task<IEnumerable<TResult>> GetTaskWithoutCancellationToken<TRequest,TResult>(Actionable<TRequest, TResult> actionable, TRequest request, int iterationsForThisThread)
        {
            return new Task<IEnumerable<TResult>>((r) =>
            {
                var resultQueue = new Queue<TResult>(iterationsForThisThread);
                for (int i = 0; i < iterationsForThisThread; i++)
                {
                    var result = actionable(request);
                    resultQueue.Enqueue(result);
                }
                return resultQueue;
            }, request, TaskCreationOptions.LongRunning);
        }

        private void StartTasks<TResult>(Queue<Task<IEnumerable<TResult>>> tasks)
        {
            foreach (var task in tasks)
                task.Start();
        }

        private async Task<IEnumerable<TResult>[]> WaitForTasks<TResult>(Queue<Task<IEnumerable<TResult>>> tasks)
        {
            var results = await Task.WhenAll(tasks);
            return results;
        }
    }
}
