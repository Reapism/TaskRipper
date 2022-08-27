namespace TaskRipper.Core
{
    public static class WorkExecutorExtensions
    {
        public static Task<IWorkResult> ExecuteAsync<T1, T2>(WorkAction<T1, T2> work,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public static Task<IWorkResult<TResult>> ExecuteAsync<T1, T2, TResult>(
            WorkFunction<T1, T2, TResult> work, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public interface IWorkExecutor
    {
        Task<IWorkResult> ExecuteAsync(WorkAction work, CancellationToken cancellationToken);
        Task<IWorkResult> ExecuteAsync<T>(WorkAction<T> work, CancellationToken cancellationToken);
        Task<IWorkResult<TResult>> ExecuteAsync<TResult>(WorkFunction<TResult> work, CancellationToken cancellationToken);
        Task<IWorkResult<TResult>> ExecuteAsync<T, TResult>(WorkFunction<T, TResult> work, CancellationToken cancellationToken);
    }

    public class WorkExecutor : IWorkExecutor
    {
        private readonly IWorkBalancer workBalancer;
        private static IWorkExecutor? _defaultInstance;
        public WorkExecutor(IWorkBalancer workBalancer)
        {
            this.workBalancer = workBalancer;
        }

        public static IWorkExecutor Default
        {
            get
            {
                return _defaultInstance ?? (_defaultInstance = new WorkExecutor(new WorkBalancer()));
            }
        }

        public async Task<IWorkResult> ExecuteAsync(WorkAction work, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncActionInternal(work, cancellationToken);
        }

        public async Task<IWorkResult> ExecuteAsync<T>(WorkAction<T> work, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncActionInternal(work, cancellationToken);
        }

        public async Task<IWorkResult<TResult>> ExecuteAsync<TResult>(WorkFunction<TResult> work, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncFuncInternal(work, cancellationToken);
        }

        public async Task<IWorkResult<TResult>> ExecuteAsync<T, TResult>(WorkFunction<T, TResult> work, CancellationToken cancellationToken)
        {
            return await ExecuteAsyncFuncInternal(work, cancellationToken);
        }

        private async Task<IWorkResult> ExecuteAsyncActionInternal(WorkAction work, CancellationToken cancellationToken)
        {
            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(work.WorkContract);
            var tasks = TaskRetriever.GetWrappedWorkActionTasks(work, iterationsByThread, cancellationToken);

            StartTasks(tasks);
            await WaitForAllTasks(tasks);
            HandleIncompleteTasks(tasks);
            var endDate = DateTime.Now;

            var dateRange = new DateRange(startDate, endDate);
            return await Task.FromResult(new WorkResult(work.WorkContract, tasks.Count(), dateRange));
        }

        private async Task<IWorkResult> ExecuteAsyncActionInternal<T>(WorkAction<T> work, CancellationToken cancellationToken)
        {
            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(work.WorkContract);
            var tasks = TaskRetriever.GetWrappedWorkActionTasks(work, iterationsByThread, cancellationToken);

            StartTasks(tasks);
            await WaitForAllTasks(tasks);
            HandleIncompleteTasks(tasks);
            var endDate = DateTime.Now;

            var dateRange = new DateRange(startDate, endDate);
            return await Task.FromResult(new WorkResult(work.WorkContract, tasks.Count(), dateRange));
        }

        private async Task<IWorkResult<TResult>> ExecuteAsyncFuncInternal<TResult>(WorkFunction<TResult> work, CancellationToken cancellationToken)
        {
            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(work.WorkContract);
            var tasks = TaskRetriever.GetWrappedWorkActionTasks(work, iterationsByThread, cancellationToken);

            StartTasks(tasks);
            var results = await WaitForAllTasks(tasks);
            HandleIncompleteTasks(tasks);
            var endDate = DateTime.Now;

            var dateRange = new DateRange(startDate, endDate);
            return await Task.FromResult(new WorkResult<TResult>(work.WorkContract, tasks.Count(), dateRange, results));
        }

        private async Task<IWorkResult<TResult>> ExecuteAsyncFuncInternal<T, TResult>(WorkFunction<T, TResult> work, CancellationToken cancellationToken)
        {
            var startDate = DateTime.Now;

            var iterationsByThread = workBalancer.Balance(work.WorkContract);
            var tasks = TaskRetriever.GetWrappedWorkActionTasks(work, iterationsByThread, cancellationToken);

            StartTasks(tasks);
            var results = await WaitForAllTasks(tasks);

            HandleIncompleteTasks(tasks);

            var endDate = DateTime.Now;

            var dateRange = new DateRange(startDate, endDate);
            return await Task.FromResult(new WorkResult<TResult>(work.WorkContract, tasks.Count(), dateRange, results));
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

        private async Task<IEnumerable<IEnumerable<IterationResult<TResult>>>> WaitForAllTasks<TResult>(IEnumerable<Task<IEnumerable<IterationResult<TResult>>>> tasks)
        {
            var results = await Task.WhenAll(tasks);
            return results;
        }
    }
}
