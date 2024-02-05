using System.Diagnostics.Contracts;

namespace TaskRipper.Core
{
    public interface IWorkBalancer
    {
        /// <summary>
        /// Balances the number of iterations per thread given the <paramref name="workContract"/>.
        /// </summary>
        /// <param name="workContract"></param>
        /// <returns>A dictionary in this format. Thread -> Iterations</returns>
        IDictionary<int, int> Balance(IWorkContract workContract);
    }

    public sealed class WorkBalancer : IWorkBalancer
    {
        public IDictionary<int, int> Balance(IWorkContract workContract)
        {
            return BalanceInternal(workContract);
        }

        private IDictionary<int, int> BalanceInternal(IWorkContract workContract)
        {
            workContract.ValidateContract();

            var workBalancerFunction = GetWorkBalancerFunction(workContract.WorkBalancerOptions);
            var iterationsByThread = workBalancerFunction.Invoke(workContract);

            RemoveEmptyEntries(iterationsByThread);

            return iterationsByThread;
        }

        private Func<IWorkContract, IDictionary<int,int>> GetWorkBalancerFunction(WorkBalancerOptions workBalancerOptions)
        {
            return workBalancerOptions switch
            {
                WorkBalancerOptions.Optimize => Optimize,
                WorkBalancerOptions.None => None,
                WorkBalancerOptions.MinimizeThreads => Min,
                WorkBalancerOptions.MaximizeThreads => High,
                _ => throw new ArgumentOutOfRangeException(nameof(workBalancerOptions)),
            };
        }

        private IDictionary<int, int> Optimize(IWorkContract workContract)
        {
            var iterationsByThread = new Dictionary<int, int>();

            var iterations = workContract.IterationsRequested;
            var maxThreadsInRange = workContract.ExecutionSettings.ThreadRange.End.Value;

            if (maxThreadsInRange < 1)
                throw new ArgumentException($"The max threads {maxThreadsInRange} is less than 1.");

            var (numOfThreads, remainderIterations) = Math.DivRem(iterations, maxThreadsInRange);
            var index = 0;

            // If the dividend and divisor does not divide at all, and the remainder is less than or equal to the max thread count
            // then, for each remainder, add remainder number of threads with a single iteration in each.
            // Ex: 1k iterations, 16 threads, threads[0..14] 62 iterations, thread[15] 71 iterations
            
            if (numOfThreads == 0 && remainderIterations <= workContract.ExecutionSettings.ThreadRange.End.Value)
            {
                for (; index < remainderIterations; index++)
                {
                    iterationsByThread.Add(index, 1);
                }
                return iterationsByThread;
            }

            // else case
            for (; index < maxThreadsInRange - 1; index++)
            {
                iterationsByThread.Add(index, numOfThreads);
            }

            iterationsByThread.Add(index, numOfThreads + remainderIterations);

            return iterationsByThread;
        }

        private int GetThreadCount(int requestThreadCount, Range threadRange)
        {
            var maxThreadCount = TaskScheduler.Current.MaximumConcurrencyLevel;
            var minRequestedThreadCount =  Math.Min(requestThreadCount, maxThreadCount);

            var maxThreadCountInRange = threadRange.End.Value;
            return Math.Min(minRequestedThreadCount, maxThreadCountInRange);
        }

        private IDictionary<int, int> None(IWorkContract workContract)
        {
            var iterationsByThread = new Dictionary<int, int>
            {
                { 0, workContract.IterationsRequested }
            };

            return iterationsByThread;
        }

        private IDictionary<int, int> Min(IWorkContract workContract)
        {
            var iterationsByThread = new Dictionary<int, int>();

            var dividend = workContract.IterationsRequested;
            // if min thread range is greater than iterations, use number of iterations as divisor, else
            // use the min thread range.
            var divisor = workContract.ExecutionSettings.ThreadRange.Start.Value > workContract.IterationsRequested
                ? workContract.IterationsRequested 
                : workContract.ExecutionSettings.ThreadRange.Start.Value;

            if (divisor <= 0)
                throw new ArgumentException("The divisor must be at least 1.");

            var tuple = Math.DivRem(dividend, divisor);
            var index = 0;

            for (; index < divisor - 1; index++)
            {
                iterationsByThread.Add(index, tuple.Quotient);
            }

            iterationsByThread.Add(index, tuple.Quotient + tuple.Remainder);

            return iterationsByThread;
        }

        private IDictionary<int, int> Medium(IWorkContract workContract)
        {
            var iterationsByThread = new Dictionary<int, int>();

            var dividend = workContract.ExecutionSettings.ExecutionRange.End.Value;

            var divisor = workContract.ExecutionSettings.ThreadRange.End.Value > workContract.ExecutionSettings.ExecutionRange.End.Value
                ? workContract.ExecutionSettings.ExecutionRange.End.Value
                : workContract.ExecutionSettings.ThreadRange.End.Value;

            if (divisor <= 0)
                throw new ArgumentException("The divisor must be at least 1.");

            var tuple = Math.DivRem(dividend, divisor);
            var index = 0;

            for (; index < divisor - 1; index++)
            {
                iterationsByThread.Add(index, tuple.Quotient);
            }

            iterationsByThread.Add(index, tuple.Quotient + tuple.Remainder);

            return iterationsByThread;
        }

        private IDictionary<int, int> High(IWorkContract workContract)
        {
            var iterationsByThread = new Dictionary<int, int>();

            var dividend = workContract.IterationsRequested;
            // if max thread range is greater than iterations, use number of iterations as divisor, else
            // use the max thread range.
            var divisor = workContract.ExecutionSettings.ThreadRange.End.Value > workContract.IterationsRequested    
                ? workContract.IterationsRequested
                : workContract.ExecutionSettings.ThreadRange.End.Value;

            if (divisor <= 0)
                throw new ArgumentException("The divisor must be at least 1.");

            var tuple = Math.DivRem(dividend, divisor);
            var index = 0;

            for (; index < divisor - 1; index++)
            {
                iterationsByThread.Add(index, tuple.Quotient);
            }

            iterationsByThread.Add(index, tuple.Quotient + tuple.Remainder);

            return iterationsByThread;
        }

        private void RemoveEmptyEntries(IDictionary<int, int> keyValuePairs)
        {
            foreach (var kvp in keyValuePairs)
                if (kvp.Value == 0)
                    keyValuePairs.Remove(kvp.Key);
        }
    }
}
