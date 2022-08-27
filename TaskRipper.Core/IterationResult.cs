using System.Diagnostics;

namespace TaskRipper.Core
{
    [DebuggerDisplay("[Thread # {ThreadNumber} Iteration {Iteration}]: {Result}")]
    public class IterationResult<TResult>
    {
        public IterationResult(short threadNumber, int iteration, TResult result)
        {
            ThreadNumber = threadNumber;
            Iteration = iteration;
            Result = result;
        }

        public short ThreadNumber { get; }
        public int Iteration { get; }
        public TResult Result { get; }
    }
}
