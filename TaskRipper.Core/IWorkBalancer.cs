namespace TaskRipper.Core
{
    public interface IWorkBalancer
    {
        IDictionary<int, int> Balance(int iterations, int numberOfThreads);
    }

    public class WorkBalancer : IWorkBalancer
    {
        public IDictionary<int, int> Balance(int iterations, int numberOfThreads)
        {
            var iterationsByThread = new Dictionary<int, int>();
            var divident = iterations;
            var divisor = numberOfThreads;

            if (divisor <= 0)
                throw new ArgumentException("The divisor must be at least 1.");


            // iterations 1010
            // threads 10
            // we need 10 entires in the dictionary
            // the first 9 items in the dictionary will have 100 iterations each
            // the last one will have 100 + remainder which is 110.

            var tuple = Math.DivRem(iterations, numberOfThreads);
            var index = 0;
            for (; index < divisor - 1; index++)
            {
                iterationsByThread.Add(index, tuple.Quotient);
            }

            iterationsByThread.Add(index, tuple.Quotient + tuple.Remainder);

            return iterationsByThread;
        }
    }
}
