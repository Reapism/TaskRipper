namespace TaskRipper.Core
{
    public interface IFunctionable<out R>
    {
        Func<R> Function { get; }
    }
    public interface IFunctionable<in T, out R>
    {
        Func<T, R> Function { get; }
    }

    public interface IFunctionable<in T1, in T2, out R>
    {
        Func<T1, T2, R> Function { get; }
    }
}
