namespace TaskRipper.Core
{
    public class IterationThread
    {
        public IterationThread(int threadNumber, int iterations)
        {
            ThreadNumber = threadNumber;
            Iterations = iterations;
        }

        public int ThreadNumber { get; set; }
        public int Iterations { get; set; }
    }
}
