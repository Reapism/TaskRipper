namespace TaskRipper.Core
{
    public interface IWorkExecutor
    {
        //IWorkerResult Execute(IWorkContract actionContract, Action action, CancellationToken cancellationToken);
        /// <summary>
        /// Executes a cancellable, <see cref="Action"/> asynchronously using information from a <see cref="IWorkContract"/>.
        /// </summary>
        /// <param name="workContract">A <see cref="IWorkContract"/> to describe the workload.</param>
        /// <param name="action">The <see cref="Action"/> to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the action.</param>
        /// <returns>A <see cref="IWorkerResult"/> containing information about the result.</returns>
        Task<IWorkerResult> ExecuteAsync(IWorkContract workContract, Action action, CancellationToken cancellationToken);

        /// <summary>
        /// Executes a cancellable, <see cref="Action{T}"/> asynchronously using information from a <see cref="IWorkContract"/>.
        /// </summary>
        /// <typeparam name="T">A parameter used in the <see cref="Action{T}"/>.</typeparam>
        /// <param name="workContract">A <see cref="IWorkContract"/> to describe the workload.</param>
        /// <param name="action">The <see cref="Action{T}"/> to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the action.</param>
        /// <returns>A <see cref="IWorkerResult"/> containing information about the result.</returns>
        Task<IWorkerResult> ExecuteAsync<T>(IWorkContract workContract, Action<T> action, CancellationToken cancellationToken);

        /// <summary>
        /// Executes a cancellable, <see cref="Action{T1, T2}"/> asynchronously using information from a <see cref="IWorkContract"/>.
        /// </summary>
        /// <typeparam name="T1">The first parameter used in the <see cref="Action{T1, T2}"/>.</typeparam>
        /// <typeparam name="T2">The second parameter used in the <see cref="Action{T1, T2}"/>.</typeparam>
        /// <param name="workContract">A <see cref="IWorkContract"/> to describe the workload.</param>
        /// <param name="action">The <see cref="Action{T1, T2}"/> to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the action.</param>
        /// <returns>A <see cref="IWorkerResult"/> containing information about the result.</returns>
        Task<IWorkerResult> ExecuteAsync<T1, T2>(IWorkContract workContract, Action<T1, T2> action, CancellationToken cancellationToken);
        Task<IWorkerResult<TResult>> ExecuteAsync<TFunc, TResult>(IWorkContract workContract, Func<TResult> func, CancellationToken cancellationToken);
        Task<IWorkerResult<TResult>> ExecuteAsync<TFunc, T, TResult>(IWorkContract workContract, Func<T, TResult> func, CancellationToken cancellationToken);
        Task<IWorkerResult<TResult>> ExecuteAsync<TFunc, T1, T2, TResult>(IWorkContract workContract, Func<T1, T2, TResult> func, CancellationToken cancellationToken);
    }

    public class WorkExecutor : IWorkExecutor
    {
        private readonly IWorkBalancer workBalancer;
        private static IWorkExecutor? defaultInstance;
        public WorkExecutor(IWorkBalancer workBalancer)
        {
            this.workBalancer = workBalancer;
        }

        public static IWorkExecutor Default
        {
            get
            {
                if (defaultInstance is null)
                    defaultInstance = new WorkExecutor(new WorkBalancer());

                return defaultInstance;
            }
        }

        public async Task<IWorkerResult> ExecuteAsync(IWorkContract workContract, Action action, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncActionInternal(workContract, action, cancellationToken);
        }

        public async Task<IWorkerResult> ExecuteAsync<T>(IWorkContract workContract, Action<T> action, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncActionInternal(workContract, action, cancellationToken);
        }

        public async Task<IWorkerResult> ExecuteAsync<T1, T2>(IWorkContract workContract, Action<T1, T2> action, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncActionInternal(workContract, action, cancellationToken);
        }

        public async Task<IWorkerResult<TResult>> ExecuteAsync<TFunc, TResult>(IWorkContract workContract, Func<TResult> func, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncFunctionInternal<Func<TResult>, TResult>(workContract, func, cancellationToken);
        }

        public async Task<IWorkerResult<TResult>> ExecuteAsync<TFunc, T, TResult>(IWorkContract workContract, Func<T, TResult> func, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncFunctionInternal<Func<T, TResult>, TResult>(workContract, func, cancellationToken);
        }

        public async Task<IWorkerResult<TResult>> ExecuteAsync<TFunc, T1, T2, TResult>(IWorkContract workContract, Func<T1, T2, TResult> func, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncFunctionInternal<Func<T1, T2, TResult>, TResult>(workContract, func, cancellationToken);
        }


        private async Task<IWorkerResult> ExecuteAsyncActionInternal<T>(IWorkContract workContract, T action, CancellationToken cancellationToken)
             where T : Delegate
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

            var iterationsByThread = workBalancer.Balance(workContract);
            var tasks = GetTasks(action, iterationsByThread, cancellationToken);

            await ExecuteTasks(tasks);

            var endDate = DateTime.Now;

            var dateRange = new DateRange(startDate, endDate);
            return await Task.FromResult(new WorkerResult(workContract, tasks.Count(), dateRange));
        }

        private async Task<IWorkerResult<R>> ExecuteAsyncFunctionInternal<T, R>(IWorkContract workContract, T func, CancellationToken cancellationToken)
             where T : Delegate
        {
            if (workContract is null)
            {
                throw new ArgumentNullException(nameof(workContract));
            }
            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(workContract);
            var tasks = GetTasks(func, iterationsByThread, cancellationToken);
            try
            {
                // get the task that stores and executes all the tasks.
                await ExecuteTasks(tasks);
            }
            catch (Exception e)
            {

            }
            finally
            {

            }

            var endDate = DateTime.Now;

            var dateRange = new DateRange(startDate, endDate);
            return await Task.FromResult(new WorkerResult<R>(workContract, tasks.Count(), dateRange, new Dictionary<int, IterationResult<R>>()));
        }

        private async Task ExecuteTasks(IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
                task.Start();

            await Task.WhenAll(tasks);
        }

        private IEnumerable<Task> GetTasks<T>(T action, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
            where T : Delegate
        {
            var tasks = new List<Task>();

            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = WrapActionInIterableTask(action, iterationsByThread[i], cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }

        private Task WrapActionInIterableTask<T>(T actionToWrap, int iterationsForThisTask, CancellationToken cancellationToken)
            where T : Delegate
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    actionToWrap.DynamicInvoke();
                }
            });

            var task = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);

            return task;
        }
    }
}
