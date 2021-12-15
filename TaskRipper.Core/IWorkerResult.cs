namespace TaskRipper.Core
{
    public interface IWorkerResult
    {
        IWorkContract WorkContract { get; }
        TimeSpan Duration { get; }
        int ThreadsUsed { get; }
    }

    public class WorkerResult : IWorkerResult
    {
        public WorkerResult(IWorkContract workContract, int threadsUsed)
        {
            WorkContract = workContract;
            ThreadsUsed = threadsUsed;
            Duration = workContract.EndDate - workContract.StartDate;
        }
        public IWorkContract WorkContract { get; }

        public TimeSpan Duration { get; }

        public int ThreadsUsed { get; }
    }
}
