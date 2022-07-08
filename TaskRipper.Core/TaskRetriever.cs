namespace TaskRipper.Core
{
    internal static class TaskRetriever
    {
        internal static IEnumerable<Task> GetWrappedWorkActionTasks(WorkAction work, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            
            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = IterableDelegateWrapper.Wrap(work, new IterationThread(i, iterationsByThread[i]), cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }

        internal static IEnumerable<Task> GetWrappedWorkActionTasks<T>(WorkAction<T> work, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = IterableDelegateWrapper.Wrap(work, new IterationThread(i, iterationsByThread[i]), cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }
        internal static IEnumerable<Task> GetWrappedWorkActionTasks<T1, T2>(WorkAction<T1, T2> work, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = IterableDelegateWrapper.Wrap(work, new IterationThread(i, iterationsByThread[i]), cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }

        internal static IEnumerable<Task<IEnumerable<IterationResult<TResult>>>> GetWrappedWorkActionTasks<TResult>(WorkFunction<TResult> work, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IEnumerable<IterationResult<TResult>>>>();

            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = IterableDelegateWrapper.Wrap(work, new IterationThread(i, iterationsByThread[i]), cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }
        internal static IEnumerable<Task<IEnumerable<IterationResult<TResult>>>> GetWrappedWorkActionTasks<T, TResult>(WorkFunction<T, TResult> work, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IEnumerable<IterationResult<TResult>>>>();

            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = IterableDelegateWrapper.Wrap(work, new IterationThread(i, iterationsByThread[i]), cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }

        internal static IEnumerable<Task<IEnumerable<IterationResult<TResult>>>> GetWrappedWorkActionTasks<T1, T2, TResult>(WorkFunction<T1, T2, TResult> work, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IEnumerable<IterationResult<TResult>>>>();

            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = IterableDelegateWrapper.Wrap(work, new IterationThread(i, iterationsByThread[i]), cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }
    }
}
