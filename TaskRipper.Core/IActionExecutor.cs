namespace TaskRipper.Core
{
    public interface IActionExecutor
    {
        //IWorkerResult Execute(IWorkContract actionContract, Action action, CancellationToken cancellationToken);
        Task<IWorkerResult> ExecuteAsync(IWorkContract workContract, Action action, CancellationToken cancellationToken);
    }

    public class ActionExecutor : IActionExecutor
    {
        private readonly IWorkBalancer workBalancer;
        private static IActionExecutor defaultInstance;
        public ActionExecutor(IWorkBalancer workBalancer)
        {
            this.workBalancer = workBalancer;
        }

        public static IActionExecutor Default
        {
            get
            {
                if (defaultInstance is null)
                    defaultInstance = new ActionExecutor(new WorkBalancer());

                return defaultInstance;
            }
        }

        public async Task<IWorkerResult> ExecuteAsync(IWorkContract workContract, Action action, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncInternal(workContract, action, cancellationToken);
        }

        private async Task<IWorkerResult> ExecuteAsyncInternal(IWorkContract workContract, Action action, CancellationToken cancellationToken)
        {
            if (workContract is null)
            {
                throw new ArgumentNullException(nameof(workContract));
            }
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(workContract.Iterations, workContract.ExecutionSettings.ExecutionEnvironment.ThreadCount);
            var tasks = GetTasks(action, iterationsByThread, cancellationToken);

            await ExecuteTasks(tasks);

            var endDate = DateTime.Now;

            var dateRange = new DateRange(startDate, endDate);
            return await Task.FromResult(new WorkerResult(workContract, tasks.Count(), dateRange));
        }

        private async Task ExecuteTasks(IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
                task.Start();

            await Task.WhenAll(tasks);
        }

        private IEnumerable<Task> GetTasks(Action action, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = WrapActionInIterableTask(action, iterationsByThread[i], cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }

        private Task WrapActionInIterableTask(Action actionToWrap, int iterationsForThisTask, CancellationToken cancellationToken)
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    actionToWrap.Invoke();
                }
            });

            var task = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);

            return task;
        }
    }
}
