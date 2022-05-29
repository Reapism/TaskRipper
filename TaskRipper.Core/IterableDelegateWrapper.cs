namespace TaskRipper.Core
{
    /// <summary>
    /// Purpose is to wrap incoming delegates in a specific type of iterable task
    /// whilst honoring possible cancellation per iteration and storing results of
    /// specific delegates with returning values.
    /// </summary>
    internal class IterableDelegateWrapper
    {
        internal static Task Wrap(WorkAction work, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Action wrappedAction = new Action(() =>
            {
                for (int i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    work.Execute();
                }
            });

            Task wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }
        internal static Task Wrap<T>(WorkAction<T> work, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Action wrappedAction = new Action(() =>
            {
                for (int i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    work.Execute();
                }
            });

            Task wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task Wrap<T1, T2>(WorkAction<T1, T2> work, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Action wrappedAction = new Action(() =>
            {
                for (int i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    work.Execute();
                }
            });

            Task wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IDictionary<int, IterationResult<TResult>>> Wrap<TResult>(WorkFunction<TResult> work, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Func<IDictionary<int, IterationResult<TResult>>> wrappedFunction = new Func<IDictionary<int, IterationResult<TResult>>>(() =>
            {
                Dictionary<int, IterationResult<TResult>> values = new Dictionary<int, IterationResult<TResult>>();
                for (int i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    values.Add(i, new IterationResult<TResult>(i, work.Execute()));
                }
                return values;
            });

            Task<IDictionary<int, IterationResult<TResult>>> wrappedTask = new Task<IDictionary<int, IterationResult<TResult>>>(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IDictionary<int, IterationResult<TResult>>> Wrap<T, TResult>(WorkFunction<T, TResult> work, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Func<IDictionary<int, IterationResult<TResult>>> wrappedFunction = new Func<IDictionary<int, IterationResult<TResult>>>(() =>
            {
                Dictionary<int, IterationResult<TResult>>? values = new Dictionary<int, IterationResult<TResult>>();
                for (int i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    TResult returnValue = work.Execute();
                    values.Add(i, new IterationResult<TResult>(i, returnValue));
                }
                return values;
            });

            Task<IDictionary<int, IterationResult<TResult>>> wrappedTask = new Task<IDictionary<int, IterationResult<TResult>>>(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IDictionary<int, IterationResult<TResult>>> Wrap<T1, T2, TResult>(WorkFunction<T1, T2, TResult> work, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Func<IDictionary<int, IterationResult<TResult>>> wrappedFunction = new Func<IDictionary<int, IterationResult<TResult>>>(() =>
            {
                Dictionary<int, IterationResult<TResult>> values = new Dictionary<int, IterationResult<TResult>>();
                for (int i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    values.Add(i, new IterationResult<TResult>(i, work.Execute()));
                }
                return values;
            });

            Task<IDictionary<int, IterationResult<TResult>>> wrappedTask = new Task<IDictionary<int, IterationResult<TResult>>>(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task Wrap(WorkDelegate work, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            Action wrappedAction = new Action(() =>
            {
                for (int i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    work.Execute();
                }
            });

            Task wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }
    }
}
