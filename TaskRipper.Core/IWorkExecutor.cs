namespace TaskRipper.Core
{
    //TODO add a new Execute overload that takes in a func that helps mutate parameter values before or after an execution of a specific iteration
    // of running the action/func.
    // maybe instead of taking in a Action or Func delegate directly into the executor, it takes in a wrapper class
    // that has the exeuting Action and the action that mutates the value of the parameters if needed.
    public interface IWorkExecutor
    {
        /// <summary>
        /// Executes a cancellable, <see cref="Action"/> asynchronously using information from a <see cref="IWorkContract"/>.
        /// </summary>
        /// <param name="workContract">A <see cref="IWorkContract"/> to describe the workload.</param>
        /// <param name="action">The <see cref="Action"/> to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the action.</param>
        /// <returns>A <see cref="IWorkResult"/> containing information about the result.</returns>
        Task<IWorkResult> ExecuteAsync(IWorkContract workContract, Action action, CancellationToken cancellationToken);

        /// <summary>
        /// Executes a cancellable, <see cref="Action{T}"/> asynchronously using information from a <see cref="IWorkContract"/>.
        /// </summary>
        /// <typeparam name="T">A parameter used in the <see cref="Action{T}"/>.</typeparam>
        /// <param name="workContract">A <see cref="IWorkContract"/> to describe the workload.</param>
        /// <param name="action">The <see cref="Action{T}"/> to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the action.</param>
        /// <returns>A <see cref="IWorkResult"/> containing information about the result.</returns>
        Task<IWorkResult> ExecuteAsync<T>(IWorkContract workContract, Action<T> action, T param, CancellationToken cancellationToken);

        /// <summary>
        /// Executes a cancellable, <see cref="Action{T1, T2}"/> asynchronously using information from a <see cref="IWorkContract"/>.
        /// </summary>
        /// <typeparam name="T1">The first parameter used in the <see cref="Action{T1, T2}"/>.</typeparam>
        /// <typeparam name="T2">The second parameter used in the <see cref="Action{T1, T2}"/>.</typeparam>
        /// <param name="workContract">A <see cref="IWorkContract"/> to describe the workload.</param>
        /// <param name="action">The <see cref="Action{T1, T2}"/> to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the action.</param>
        /// <returns>A <see cref="IWorkResult"/> containing information about the result.</returns>
        Task<IWorkResult> ExecuteAsync<T1, T2>(IWorkContract workContract, Action<T1, T2> action, T1 param, T2 param2, CancellationToken cancellationToken);
        Task<IWorkResult<TResult>> ExecuteAsync<TResult>(IWorkContract workContract, Func<TResult> func, CancellationToken cancellationToken);
        Task<IWorkResult<TResult>> ExecuteAsync<T, TResult>(IWorkContract workContract, Func<T, TResult> func, T param, CancellationToken cancellationToken);
        Task<IWorkResult<TResult>> ExecuteAsync<T1, T2, TResult>(IWorkContract workContract, Func<T1, T2, TResult> func, T1 param, T2 param2, CancellationToken cancellationToken);
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

        public async Task<IWorkResult> ExecuteAsync(IWorkContract workContract, Action action, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncActionInternal(workContract, action, cancellationToken);
        }

        public async Task<IWorkResult> ExecuteAsync<T>(IWorkContract workContract, Action<T> action, T param, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncActionInternal(workContract, action, param, cancellationToken);
        }

        public async Task<IWorkResult> ExecuteAsync<T1, T2>(IWorkContract workContract, Action<T1, T2> action, T1 param, T2 param2, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncActionInternal(workContract, action, param, param2, cancellationToken);
        }

        public async Task<IWorkResult<TResult>> ExecuteAsync<TResult>(IWorkContract workContract, Func<TResult> func, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncFuncInternal(workContract, func, cancellationToken);
        }

        public async Task<IWorkResult<TResult>> ExecuteAsync<T, TResult>(IWorkContract workContract, Func<T, TResult> func, T param, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncFuncInternal(workContract, func, param, cancellationToken);
        }

        public async Task<IWorkResult<TResult>> ExecuteAsync<T1, T2, TResult>(IWorkContract workContract, Func<T1, T2, TResult> func, T1 param, T2 param2, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncFuncInternal(workContract, func, param, param2, cancellationToken);
        }

        private async Task<IWorkResult> ExecuteAsyncActionInternal(IWorkContract workContract, Action action, CancellationToken cancellationToken)
        {
            ValidateContract(workContract, action);

            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(workContract);
            var tasks = TaskRetriever.GetTasks(action, iterationsByThread, cancellationToken);

            StartTasks(tasks);
            await WaitForAllTasks(tasks);
            HandleIncompleteTasks(tasks);
            var endDate = DateTime.Now;

            var dateRange = new DateRange(startDate, endDate);
            return await Task.FromResult(new WorkResult(workContract, tasks.Count(), dateRange));
        }

        private async Task<IWorkResult> ExecuteAsyncActionInternal<T>(IWorkContract workContract, Action<T> action, T param, CancellationToken cancellationToken)
        {
            ValidateContract(workContract, action);

            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(workContract);
            var tasks = TaskRetriever.GetTasks(action, param, iterationsByThread, cancellationToken);

            StartTasks(tasks);
            await WaitForAllTasks(tasks);
            HandleIncompleteTasks(tasks);
            var endDate = DateTime.Now;

            var dateRange = new DateRange(startDate, endDate);
            return await Task.FromResult(new WorkResult(workContract, tasks.Count(), dateRange));
        }

        private async Task<IWorkResult> ExecuteAsyncActionInternal<T1, T2>(IWorkContract workContract, Action<T1, T2> action, T1 param, T2 param2, CancellationToken cancellationToken)
        {
            ValidateContract(workContract, action);

            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(workContract);
            var tasks = TaskRetriever.GetTasks(action, param, param2, iterationsByThread, cancellationToken);

            StartTasks(tasks);
            await WaitForAllTasks(tasks);
            HandleIncompleteTasks(tasks);
            var endDate = DateTime.Now;

            var dateRange = new DateRange(startDate, endDate);
            return await Task.FromResult(new WorkResult(workContract, tasks.Count(), dateRange));
        }

        private async Task<IWorkResult<TResult>> ExecuteAsyncFuncInternal<TResult>(IWorkContract workContract, Func<TResult> func, CancellationToken cancellationToken)
        {
            ValidateContract(workContract, func);

            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(workContract);
            var tasks = TaskRetriever.GetTasks(func, iterationsByThread, cancellationToken);

            StartTasks(tasks);
            await WaitForAllTasks(tasks);
            HandleIncompleteTasks(tasks);
            var endDate = DateTime.Now;

            var dateRange = new DateRange(startDate, endDate);
            return await Task.FromResult(new WorkResult<TResult>(workContract, tasks.Count(), dateRange, null));
        }

        private async Task<IWorkResult<TResult>> ExecuteAsyncFuncInternal<T, TResult>(IWorkContract workContract, Func<T, TResult> func, T param, CancellationToken cancellationToken)
        {
            ValidateContract(workContract, func);

            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(workContract);
            var tasks = TaskRetriever.GetTasks(func, param, iterationsByThread, cancellationToken);
            try
            {
                StartTasks(tasks);
                await WaitForAllTasks(tasks);

            }
            catch(Exception ex)
            {
                HandleIncompleteTasks(tasks);
            }
            var endDate = DateTime.Now;

            var dateRange = new DateRange(startDate, endDate);
            return await Task.FromResult(new WorkResult<TResult>(workContract, tasks.Count(), dateRange, null));
        }

        private async Task<IWorkResult<TResult>> ExecuteAsyncFuncInternal<T1, T2, TResult>(IWorkContract workContract, Func<T1, T2, TResult> func, T1 param, T2 param2, CancellationToken cancellationToken)
        {
            ValidateContract(workContract, func);

            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(workContract);
            var tasks = TaskRetriever.GetTasks(func, param, param2, iterationsByThread, cancellationToken);

            StartTasks(tasks);
            var results = await WaitForAllTasks(tasks);
            HandleIncompleteTasks(tasks);
            var endDate = DateTime.Now;

            var dateRange = new DateRange(startDate, endDate);
            return await Task.FromResult(new WorkResult<TResult>(workContract, tasks.Count(), dateRange, results));
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

        private static void ValidateContract<T>(IWorkContract workContract, T @delegate)
            where T : Delegate
        {
            if (workContract is null)
            {
                throw new ArgumentNullException(nameof(workContract));
            }

            if (@delegate is null)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }
        }

        private void StartTasks(IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
                task.Start();
        }

        private void StartTasks<TResult>(IEnumerable<Task<IDictionary<int, IterationResult<TResult>>>> tasks)
        {
            foreach (var task in tasks)
                task.Start();
        }

        private async Task WaitForAllTasks(IEnumerable<Task> tasks)
        {
            await Task.WhenAll(tasks);
        }

        private async Task<IEnumerable<IDictionary<int, IterationResult<TResult>>>> WaitForAllTasks<TResult>(IEnumerable<Task<IDictionary<int, IterationResult<TResult>>>> tasks)
        {
            var results = await Task.WhenAll(tasks);
            return results;
        }
    }
}
