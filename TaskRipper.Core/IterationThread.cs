namespace TaskRipper.Core
{
    public class IterationThread
    {
        public IterationThread(short threadNumber, int iterations)
        {
            ThreadNumber = threadNumber;
            Iterations = iterations;
        }

        public short ThreadNumber { get; set; }
        public int Iterations { get; set; }
    }
}
