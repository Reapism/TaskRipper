namespace TaskRipper.Core
{
    /// <summary>
    /// Purpose is to wrap incoming delegates in a specific type of iterable task
    /// whilst honoring possible cancellation per iteration and storing results of
    /// specific delegates with returning values.
    /// </summary>
    internal class IterableDelegateWrapper
    {
        internal static Task Wrap(Action action, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action();
                }
            });

            var wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task Wrap<T>(Action<T> action, T param, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action(param);
                }
            });

            var wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task Wrap<T1, T2>(Action<T1, T2> action, T1 param, T2 param2, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action(param, param2);
                }
            });

            var wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IDictionary<int, IterationResult<TResult>>> Wrap<TResult>(Func<TResult> func, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            var wrappedFunction = new Func<IDictionary<int, IterationResult<TResult>>>(() =>
            {
                var values= new Dictionary<int, IterationResult<TResult>>();
                for (var i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    values.Add(i, new IterationResult<TResult>(i, func()));
                }
                return values;
            });

            var wrappedTask = new Task<IDictionary<int, IterationResult<TResult>>> (wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IDictionary<int, IterationResult<TResult>>> Wrap<T, TResult>(Func<T, TResult> func, T param, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            var wrappedFunction = new Func<IDictionary<int, IterationResult<TResult>>>(() =>
            {
                var values= new Dictionary<int, IterationResult<TResult>>();
                for (var i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var returnValue = func(param);
                    values.Add(i, new IterationResult<TResult>(i, returnValue));
                }
                return values;
            });

            var wrappedTask = new Task<IDictionary<int, IterationResult<TResult>>>(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IDictionary<int, IterationResult<TResult>>> Wrap<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param, T2 param2, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            var wrappedFunction = new Func<IDictionary<int, IterationResult<TResult>>>(() =>
            {
                var values = new Dictionary<int, IterationResult<TResult>>();
                for (var i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    values.Add(i, new IterationResult<TResult>(i, func(param, param2)));
                }
                return values;
            });

            var wrappedTask = new Task<IDictionary<int, IterationResult<TResult>>>(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static MulticastDelegate GetMulticastDelegate(WorkDelegate work)
        {
            work.
        }

        internal static Task Wrap(WorkDelegate work, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action();
                }
            });

            var wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task Wrap<T>(Action<T> action, T param, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action(param);
                }
            });

            var wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task Wrap<T1, T2>(Action<T1, T2> action, T1 param, T2 param2, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action(param, param2);
                }
            });

            var wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IDictionary<int, IterationResult<TResult>>> Wrap<TResult>(Func<TResult> func, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            var wrappedFunction = new Func<IDictionary<int, IterationResult<TResult>>>(() =>
            {
                var values = new Dictionary<int, IterationResult<TResult>>();
                for (var i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    values.Add(i, new IterationResult<TResult>(i, func()));
                }
                return values;
            });

            var wrappedTask = new Task<IDictionary<int, IterationResult<TResult>>>(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IDictionary<int, IterationResult<TResult>>> Wrap<T, TResult>(Func<T, TResult> func, T param, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            var wrappedFunction = new Func<IDictionary<int, IterationResult<TResult>>>(() =>
            {
                var values = new Dictionary<int, IterationResult<TResult>>();
                for (var i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var returnValue = func(param);
                    values.Add(i, new IterationResult<TResult>(i, returnValue));
                }
                return values;
            });

            var wrappedTask = new Task<IDictionary<int, IterationResult<TResult>>>(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IDictionary<int, IterationResult<TResult>>> Wrap<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param, T2 param2, IterationThread iterationThread, CancellationToken cancellationToken)
        {
            var wrappedFunction = new Func<IDictionary<int, IterationResult<TResult>>>(() =>
            {
                var values = new Dictionary<int, IterationResult<TResult>>();
                for (var i = 0; i < iterationThread.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    values.Add(i, new IterationResult<TResult>(i, func(param, param2)));
                }
                return values;
            });

            var wrappedTask = new Task<IDictionary<int, IterationResult<TResult>>>(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }
    }
}
