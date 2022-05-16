namespace TaskRipper.Core
{
    internal static class TaskRetriever
    {
        internal static IEnumerable<Task> GetTasks(Action action, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            
            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = IterableDelegateWrapper.Wrap(action, new IterationThread(i, iterationsByThread[i]), cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }

        internal static IEnumerable<Task> GetTasks<T>(Action<T> action, T param, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = IterableDelegateWrapper.Wrap(action, param, new IterationThread(i, iterationsByThread[i]), cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }
        internal static IEnumerable<Task> GetTasks<T1, T2>(Action<T1, T2> action, T1 param, T2 param2, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = IterableDelegateWrapper.Wrap(action, param, param2, new IterationThread(i, iterationsByThread[i]), cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }

        internal static IEnumerable<Task<IDictionary<int, IterationResult<TResult>>>> GetTasks<TResult>(Func<TResult> func, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IDictionary<int, IterationResult<TResult>>>>();

            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = IterableDelegateWrapper.Wrap(func, new IterationThread(i, iterationsByThread[i]), cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }
        internal static IEnumerable<Task<IDictionary<int, IterationResult<TResult>>>> GetTasks<T, TResult>(Func<T, TResult> func, T param, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IDictionary<int, IterationResult<TResult>>>>();

            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = IterableDelegateWrapper.Wrap(func, param, new IterationThread(i, iterationsByThread[i]), cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }

        internal static IEnumerable<Task<IDictionary<int, IterationResult<TResult>>>> GetTasks<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param, T2 param2, IDictionary<int, int> iterationsByThread, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IDictionary<int, IterationResult<TResult>>>>();

            for (int i = 0; i < iterationsByThread.Count; i++)
            {
                var iterableTask = IterableDelegateWrapper.Wrap(func, param, param2, new IterationThread(i, iterationsByThread[i]), cancellationToken);
                tasks.Add(iterableTask);
            }

            return tasks;
        }
    }
}
