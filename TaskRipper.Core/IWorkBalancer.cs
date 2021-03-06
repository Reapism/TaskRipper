namespace TaskRipper.Core
{
    public interface IWorkBalancer
    {
        IDictionary<int, int> Balance(IWorkContract workContract);
    }

    public class WorkBalancer : IWorkBalancer
    {
        public IDictionary<int, int> Balance(IWorkContract workContract)
        {
            return BalanceInternal(workContract);
        }

        private IDictionary<int, int> BalanceInternal(IWorkContract workContract)
        {
            ValidateParameters(workContract);

            var workBalancerFunction = GetWorkBalancerFunction(workContract.ExecutionSettings.WorkBalancerOptions);
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
                WorkBalancerOptions.Min => Min,
                WorkBalancerOptions.Medium => Medium,
                WorkBalancerOptions.High => High,
                _ => throw new ArgumentOutOfRangeException(nameof(workBalancerOptions)),
            };
        }

        private IDictionary<int, int> Optimize(IWorkContract workContract)
        {
            var iterationsByThread = new Dictionary<int, int>();

            var dividend = workContract.Iterations;
            var divisor = workContract.ExecutionSettings.ThreadRange.End.Value;

            if (divisor <= 0)
                throw new ArgumentException("The divisor must be at least 1.");

            var tuple = Math.DivRem(dividend, divisor);
            var index = 0;

            // If the dividend and divisor does not divide at all, and the remainder is less than or equal to the max thread count
            // then, for each remainder, add reaminder number of threads with a single iteration in each.
            if (tuple.Quotient == 0 && tuple.Remainder <= workContract.ExecutionSettings.ThreadRange.End.Value)
            {
                for (; index < tuple.Remainder; index++)
                {
                    iterationsByThread.Add(index, 1);
                }
                return iterationsByThread;
            }

            for (; index < divisor - 1; index++)
            {
                iterationsByThread.Add(index, tuple.Quotient);
            }

            iterationsByThread.Add(index, tuple.Quotient + tuple.Remainder);

            return iterationsByThread;
        }

        private IDictionary<int, int> None(IWorkContract workContract)
        {
            var iterationsByThread = new Dictionary<int, int>();

            iterationsByThread.Add(0, workContract.Iterations);

            return iterationsByThread;
        }

        private IDictionary<int, int> Min(IWorkContract workContract)
        {
            var iterationsByThread = new Dictionary<int, int>();

            var dividend = workContract.Iterations;
            // if min thread range is greater than iterations, use number of iterations as divisor, else
            // use the min thread range.
            var divisor = workContract.ExecutionSettings.ThreadRange.Start.Value > workContract.Iterations 
                ? workContract.Iterations 
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

            var dividend = workContract.Iterations;

            var divisor = workContract.ExecutionSettings.ThreadRange.End.Value > workContract.Iterations
                ? workContract.Iterations
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

            var dividend = workContract.Iterations;
            // if max thread range is greater than iterations, use number of iterations as divisor, else
            // use the max thread range.
            var divisor = workContract.ExecutionSettings.ThreadRange.End.Value > workContract.Iterations
                ? workContract.Iterations
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

        private void ValidateParameters(IWorkContract workContract)
        {
            var isInRange = workContract.ExecutionSettings.ExecutionRange.IsInRange(workContract.Iterations);
            if (!isInRange)
                throw new IterationsOutOfRangeException(workContract, workContract.Iterations, nameof(workContract.Iterations));
        }

        private void RemoveEmptyEntries(IDictionary<int, int> keyValuePairs)
        {
            foreach (var kvp in keyValuePairs)
                if (kvp.Value == 0)
                    keyValuePairs.Remove(kvp.Key);
        }
    }
}
