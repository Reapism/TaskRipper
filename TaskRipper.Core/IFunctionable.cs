namespace TaskRipper.Core
{
    public interface IFunctionable<out R>
    {
        Func<R> Function { get; }
    }

    public interface IFunctionable<T, out R>
    {
        Func<T, R> Function { get; }
        T Param { get; }
    }

    public interface IFunctionable<T1, T2, out R>
    {
        Func<T1, T2, R> Function { get; }
        T1 Param { get; }
        T2 Param2 { get; }
    }
}
