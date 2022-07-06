namespace TaskRipper.Core
{
    /// <summary>
    /// Purpose is to wrap incoming delegates in a specific type of iterable task
    /// whilst honoring possible cancellation per iteration and storing results of
    /// specific delegates with returning values.
    /// </summary>
    internal class IterableDelegateWrapper
    {
        internal static Task Wrap(WorkAction workAction, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Action wrappedAction = GetWrappedAction(workAction, iterationThread, cancellationToken);

            Task wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }
        internal static Task Wrap<T>(WorkAction<T> workAction, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Action wrappedAction = GetWrappedAction(workAction, iterationThread, cancellationToken);

            Task wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task Wrap<T1, T2>(WorkAction<T1,T2> workAction, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Action wrappedAction = GetWrappedAction(workAction, iterationThread, cancellationToken);

            Task wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        private static Action GetWrappedAction(IDelegateExecutor executor, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            return () =>
            {
                for (int i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    executor.Execute();
                }
            };
        }

        private static Func<IDictionary<int, IterationResult<TResult>>> GetWrappedFunction<TResult>(
            IReturnableDelegateExecutor<TResult> executor, IterationThread iterationThread,
            CancellationToken cancellationToken)
        {
            return () =>
            {
                Dictionary<int, IterationResult<TResult>> values = new();
                for (int i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    values.Add(i, new IterationResult<TResult>(i, executor.Execute()));
                }
                return values;
            };
        }
        internal static Task<IDictionary<int, IterationResult<TResult>>> Wrap<TResult>(WorkFunction<TResult> workFunction, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Func<IDictionary<int, IterationResult<TResult>>> wrappedFunction = GetWrappedFunction(workFunction, iterationThread, cancellationToken);
            Task<IDictionary<int, IterationResult<TResult>>> wrappedTask = new Task<IDictionary<int, IterationResult<TResult>>>(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IDictionary<int, IterationResult<TResult>>> Wrap<T, TResult>(WorkFunction<T, TResult> workFunction, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Func<IDictionary<int, IterationResult<TResult>>> wrappedFunction = GetWrappedFunction(workFunction, iterationThread, cancellationToken);
            Task<IDictionary<int, IterationResult<TResult>>> wrappedTask = new(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IDictionary<int, IterationResult<TResult>>> Wrap<T1, T2, TResult>(WorkFunction<T1, T2, TResult> workFunction, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Func<IDictionary<int, IterationResult<TResult>>> wrappedFunction = GetWrappedFunction(workFunction, iterationThread, cancellationToken);
            Task<IDictionary<int, IterationResult<TResult>>> wrappedTask = new (wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }
    }
}
