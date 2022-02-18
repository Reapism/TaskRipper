namespace TaskRipper.Core
{
    public class IterationResult<T>
    {
        public IterationResult(int iteration, T result)
        {
            Iteration = iteration;
            Result = result;
        }

        public int Iteration { get; }
        public T Result { get; }
    }
}
