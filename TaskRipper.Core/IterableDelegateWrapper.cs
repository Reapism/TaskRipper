namespace TaskRipper.Core
{
    /// <summary>
    /// Purpose is to wrap incoming delegates in a specific type of iterable task
    /// whilst honoring possible cancellation per iteration.
    /// </summary>
    internal class IterableDelegateWrapper
    {
        internal static Task WrapAndExecute(Action action, int iterationsForThisTask, CancellationToken cancellationToken)
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action();
                }
            });

            var wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            var result = Task.Factory.StartNew(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning, Task.Factory.Scheduler ?? TaskScheduler.Default);
            return wrappedTask;
        }

        internal static Task WrapAndExecute<T>(Action<T> action, T param, int iterationsForThisTask, CancellationToken cancellationToken)
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action(param);
                }
            });

            var wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            var result = Task.Factory.StartNew(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning, Task.Factory.Scheduler ?? TaskScheduler.Default);
            return wrappedTask;
        }

        internal static Task WrapAndExecute<T1, T2>(Action<T1, T2> action, T1 param, T2 param2, int iterationsForThisTask, CancellationToken cancellationToken)
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action(param, param2);
                }
            });

            var wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            var result = Task.Factory.StartNew(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning, Task.Factory.Scheduler ?? TaskScheduler.Default);
            return wrappedTask;
        }

        internal static Task<IEnumerable<TResult>> WrapAndExecute<TResult>(Func<TResult> func, int iterationsForThisTask, CancellationToken cancellationToken)
        {
            // need a wrapped function to store the value of each func return and
            // enqueue it. 
            // then start the task. and expect the IEnumerable<TReturn> back.
            var wrappedFunc = new Func<IEnumerable<TResult>>(() =>
            {
                var funcReturns = new Queue<TResult>(iterationsForThisTask);
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    funcReturns.Enqueue(func());
                }
                return funcReturns;
            });

            var wrappedTask = new Task<IEnumerable<TResult>>(wrappedFunc, cancellationToken, TaskCreationOptions.LongRunning);
            var result = Task.Factory.StartNew(wrappedFunc, cancellationToken, TaskCreationOptions.LongRunning, Task.Factory.Scheduler ?? TaskScheduler.Default);
            return wrappedTask;
        }

        internal static Task<IEnumerable<TResult>> WrapAndExecute<T, TResult>(Func<T, TResult> func, T param, int iterationsForThisTask, CancellationToken cancellationToken)
        {
            // need a wrapped function to store the value of each func return and
            // enqueue it. 
            // then start the task. and expect the IEnumerable<TReturn> back.
            var wrappedFunc = new Func<IEnumerable<TResult>>(() =>
            {
                var funcReturns = new Queue<TResult>(iterationsForThisTask);
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    funcReturns.Enqueue(func(param));
                }
                return funcReturns;
            });

            var wrappedTask = new Task<IEnumerable<TResult>>(wrappedFunc, cancellationToken, TaskCreationOptions.LongRunning);
            var result = Task.Factory.StartNew(wrappedFunc, cancellationToken, TaskCreationOptions.LongRunning, Task.Factory.Scheduler ?? TaskScheduler.Default);
            return wrappedTask;
        }

        internal static Task<IEnumerable<TResult>> WrapAndExecute<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param, T2 param2, int iterationsForThisTask, CancellationToken cancellationToken)
        {
            // need a wrapped function to store the value of each func return and
            // enqueue it. 
            // then start the task. and expect the IEnumerable<TReturn> back.
            var wrappedFunc = new Func<IEnumerable<TResult>>(() =>
            {
                var funcReturns = new Queue<TResult>(iterationsForThisTask);
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    funcReturns.Enqueue(func(param, param2));
                }
                return funcReturns;
            });

            var wrappedTask = new Task<IEnumerable<TResult>>(wrappedFunc, cancellationToken, TaskCreationOptions.LongRunning);
            var result = Task.Factory.StartNew(wrappedFunc, cancellationToken, TaskCreationOptions.LongRunning, Task.Factory.Scheduler ?? TaskScheduler.Default);
            return wrappedTask;
        }

        internal static Task Wrap(Action action, int iterationsForThisTask, CancellationToken cancellationToken)
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action();
                }
            });

            var wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task Wrap<T>(Action<T> action, T param, int iterationsForThisTask, CancellationToken cancellationToken)
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action(param);
                }
            });

            var wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task Wrap<T1, T2>(Action<T1, T2> action, T1 param, T2 param2, int iterationsForThisTask, CancellationToken cancellationToken)
        {
            var wrappedAction = new Action(() =>
            {
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action(param, param2);
                }
            });

            var wrappedTask = new Task(wrappedAction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IEnumerable<TResult>> Wrap<TResult>(Func<TResult> func, int iterationsForThisTask, CancellationToken cancellationToken)
        {
            var wrappedFunction = new Func<IEnumerable<TResult>>(() =>
            {
                var queue = new Queue<TResult>();
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    queue.Enqueue(func());
                }
                return queue;
            });

            var wrappedTask = new Task<IEnumerable<TResult>>(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IEnumerable<TResult>> Wrap<T, TResult>(Func<T, TResult> func, T param, int iterationsForThisTask, CancellationToken cancellationToken)
        {
            var wrappedFunction = new Func<IEnumerable<TResult>>(() =>
            {
                var queue = new Queue<TResult>();
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    queue.Enqueue(func(param));
                }
                return queue;
            });

            var wrappedTask = new Task<IEnumerable<TResult>>(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }

        internal static Task<IEnumerable<TResult>> Wrap<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param, T2 param2, int iterationsForThisTask, CancellationToken cancellationToken)
        {
            var wrappedFunction = new Func<IEnumerable<TResult>>(() =>
            {
                var queue = new Queue<TResult>();
                for (var i = 0; i < iterationsForThisTask; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    queue.Enqueue(func(param, param2));
                }
                return queue;
            });

            var wrappedTask = new Task<IEnumerable<TResult>>(wrappedFunction, cancellationToken, TaskCreationOptions.LongRunning);
            return wrappedTask;
        }
    }
}
