namespace TaskRipper.Core
{
    public interface IWorkContract : IWorkerParameters
    {
        IExecutionSettings ExecutionSettings { get; }
    }
}
