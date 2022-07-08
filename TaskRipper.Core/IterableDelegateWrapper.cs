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

            Task wrappedTask = new(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }
        internal static Task Wrap<T>(WorkAction<T> workAction, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Action wrappedAction = GetWrappedAction(workAction, iterationThread, cancellationToken);

            Task wrappedTask = new(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task Wrap<T1, T2>(WorkAction<T1,T2> workAction, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Action wrappedAction = GetWrappedAction(workAction, iterationThread, cancellationToken);

            Task wrappedTask = new(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
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

        private static Func<IEnumerable<IterationResult<TResult>>> GetWrappedFunction<TResult>(
            IReturnableDelegateExecutor<TResult> executor, IterationThread iterationThread,
            CancellationToken cancellationToken)
        {
            return () =>
            {
                IterationResult<TResult>[] values = new IterationResult<TResult>[iterationThread.Iterations];
                for (int i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    values[i] = new IterationResult<TResult>(iterationThread.ThreadNumber, i, executor.Execute());
                }
                return values;
            };
        }
        internal static Task<IEnumerable<IterationResult<TResult>>> Wrap<TResult>(WorkFunction<TResult> workFunction, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Func<IEnumerable<IterationResult<TResult>>> wrappedFunction = GetWrappedFunction(workFunction, iterationThread, cancellationToken);
            Task<IEnumerable<IterationResult<TResult>>> wrappedTask = new(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IEnumerable<IterationResult<TResult>>> Wrap<T, TResult>(WorkFunction<T, TResult> workFunction, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Func<IEnumerable<IterationResult<TResult>>> wrappedFunction = GetWrappedFunction(workFunction, iterationThread, cancellationToken);
            Task<IEnumerable<IterationResult<TResult>>> wrappedTask = new(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IEnumerable<IterationResult<TResult>>> Wrap<T1, T2, TResult>(WorkFunction<T1, T2, TResult> workFunction, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Func<IEnumerable<IterationResult<TResult>>> wrappedFunction = GetWrappedFunction(workFunction, iterationThread, cancellationToken);
            Task<IEnumerable<IterationResult<TResult>>> wrappedTask = new (wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }
    }
}
