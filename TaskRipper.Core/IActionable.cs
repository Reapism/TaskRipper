namespace TaskRipper.Core
{
    public interface IActionable
    {
        Action Action { get; }
    }
    public interface IActionable<in T>
    {
        Action<T> Action { get; }
    }

    public interface IActionable<in T1, in T2>
    {
        Action<T1, T2> Action { get; }
    }
}
