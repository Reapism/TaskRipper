namespace TaskRipper.Core
{
    public interface IWorkExecutor
    {
        Task<TRequest> ExecuteAsync<TRequest,TResult> (IWorkContract contract, TaskRipperDelegate<TRequest, TRequest> del, TRequest request, CancellationToken cancellationToken);
    }

    public sealed class WorkExecutor : IWorkExecutor
    {
        private readonly IWorkBalancer workBalancer;
        private static IWorkExecutor? _defaultInstance;
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

        public Task<TRequest> ExecuteAsync<TRequest, TResult>(IWorkContract contract, TaskRipperDelegate<TRequest, TRequest> del, TRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        //public async Task<IWorkResult> ExecuteAsync(IWorkContract workContract, CancellationToken cancellationToken)
        //{
        //    return await ExecuteAsyncActionInternal(work, cancellationToken);
        //}

        //private async Task<IWorkResult> ExecuteAsyncActionInternal(WorkAction work, CancellationToken cancellationToken)
        //{
        //    var startDate = DateTime.Now;

        //    var iterationsByThread = workBalancer.Balance(work.WorkContract);
        //    var tasks = TaskRetriever.GetWrappedWorkActionTasks(work, iterationsByThread, cancellationToken);

        //    StartTasks(tasks);
        //    await WaitForAllTasks(tasks);
        //    HandleIncompleteTasks(tasks);
        //    var endDate = DateTime.Now;

        //    return await Task.FromResult(new WorkResult(work.WorkContract, tasks.Count()));
        //}

        //private async Task<IWorkResult> ExecuteAsyncActionInternal<T>(WorkAction<T> work, CancellationToken cancellationToken)
        //{
        //    var startDate = DateTime.Now;

        //    var iterationsByThread = workBalancer.Balance(work.WorkContract);
        //    var tasks = TaskRetriever.GetWrappedWorkActionTasks(work, iterationsByThread, cancellationToken);

        //    StartTasks(tasks);
        //    await WaitForAllTasks(tasks);
        //    HandleIncompleteTasks(tasks);
        //    var endDate = DateTime.Now;

        //    return await Task.FromResult(new WorkResult(work.WorkContract, tasks.Count()));
        //}

        //private async Task<IWorkResult<TResult>> ExecuteAsyncFuncInternal<TResult>(WorkFunction<TResult> work, CancellationToken cancellationToken)
        //{
        //    var startDate = DateTime.Now;

        //    var iterationsByThread = workBalancer.Balance(work.WorkContract);
        //    var tasks = TaskRetriever.GetWrappedWorkActionTasks(work, iterationsByThread, cancellationToken);

        //    StartTasks(tasks);
        //    var results = await WaitForAllTasks(tasks);
        //    HandleIncompleteTasks(tasks);
        //    var endDate = DateTime.Now;

        //    return await Task.FromResult(new WorkResult<TResult>(work.WorkContract, tasks.Count(), results));
        //}

        //private async Task<IWorkResult<TResult>> ExecuteAsyncFuncInternal<T, TResult>(WorkFunction<T, TResult> work, CancellationToken cancellationToken)
        //{
        //    var startDate = DateTime.Now;

        //    var iterationsByThread = workBalancer.Balance(work.WorkContract);
        //    var tasks = TaskRetriever.GetWrappedWorkActionTasks(work, iterationsByThread, cancellationToken);

        //    StartTasks(tasks);
        //    var results = await WaitForAllTasks(tasks);

        //    HandleIncompleteTasks(tasks);

        //    var endDate = DateTime.Now;

        //    return await Task.FromResult(new WorkResult<TResult>(work.WorkContract, tasks.Count(), results));
        //}

        //private static void HandleIncompleteTasks(IEnumerable<Task> tasks)
        //{
        //    if (tasks.Any(e => e.IsCanceled || !e.IsCompletedSuccessfully || e.Exception is not null))
        //    {
        //        // At this point, the task was cancelled either due to an exception
        //        // TODO handle each case differently, for now, it will just fail

        //        var exceptionalTasks = tasks.Where(e => e.Exception is not null).ToArray();
        //        var cancelledTasks = tasks.Where(e => e.IsCanceled).ToArray();
        //        var incompleteTasks = tasks.Where(e => !e.IsCompletedSuccessfully).ToArray();
        //    }
        //}

        //private void StartTasks(IEnumerable<Task> tasks)
        //{
        //    foreach (var task in tasks)
        //        task.Start();
        //}

        //private void StartTasks<TResult>(IEnumerable<Task<IDictionary<int, IterationResult<TResult>>>> tasks)
        //{
        //    foreach (var task in tasks)
        //        task.Start();
        //}

        //private async Task WaitForAllTasks(IEnumerable<Task> tasks)
        //{
        //    await Task.WhenAll(tasks);
        //}

        //private async Task<IEnumerable<IEnumerable<IterationResult<TResult>>>> WaitForAllTasks<TResult>(IEnumerable<Task<IEnumerable<IterationResult<TResult>>>> tasks)
        //{
        //    var results = await Task.WhenAll(tasks);
        //    return results;
        //}
    }
}
