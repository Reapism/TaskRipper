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
            ValidateParameters(workContract);
            var iterationsByThread = new Dictionary<int, int>();
            var dividend = workContract.Iterations;
            var divisor = workContract.ExecutionSettings.ExecutionEnvironment.ThreadCount;


            if (divisor <= 0)
                throw new ArgumentException("The divisor must be at least 1.");

            var tuple = Math.DivRem(dividend, divisor);
            var index = 0;

            for (; index < divisor - 1; index++)
            {
                iterationsByThread.Add(index, tuple.Quotient);
            }

            iterationsByThread.Add(index, tuple.Quotient + tuple.Remainder);

            RemoveEmptyEntries(iterationsByThread);

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
