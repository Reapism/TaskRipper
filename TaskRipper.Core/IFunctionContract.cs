namespace TaskRipper.Core
{
    public interface IFunctionContract<out R> : IWorkerParameters, IExecutionEnvironment, IFunctionable<R>
    {
    }

    public interface IFunctionContract<in T, out R> : IWorkerParameters, IExecutionEnvironment, IFunctionable<T, R>
    {
    }

    public interface IFunctionContract<in T1, in T2, out R> : IWorkerParameters, IExecutionEnvironment, IFunctionable<T1, T2, R>
    {
    }
}
