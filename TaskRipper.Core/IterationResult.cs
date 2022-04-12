namespace TaskRipper.Core
{
    public class IterationResult<TResult>
    {
        public IterationResult(int iteration, TResult result)
        {
            Iteration = iteration;
            Result = result;
        }

        public int Iteration { get; }
        public TResult Result { get; }
    }
}
