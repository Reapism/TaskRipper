namespace TaskRipper.Core
{
    public interface IActionContract : IWorkerParameters, IExecutionEnvironment, IActionable
    {
    }

    public interface IActionContract<in T> : IWorkerParameters, IExecutionEnvironment, IActionable<T>
    {
    }

    public interface IActionContract<in T1, in T2> : IWorkerParameters, IExecutionEnvironment, IActionable<T1, T2>
    {
    }
}
