namespace TaskRipper.Core
{
    public interface IWorkContract : IWorkParameters
    {
        IExecutionSettings ExecutionSettings { get; }
    }
}
