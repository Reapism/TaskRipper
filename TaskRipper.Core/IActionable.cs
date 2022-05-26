namespace TaskRipper.Core
{
    public interface IActionable
    {
        Action Action { get; }
    }

    public interface IActionable<T>
    {
        Action<T> Action { get; }
        T Param { get; }
    }

    public interface IActionable<T1, T2>
    {
        Action<T1, T2> Action { get; }
        T1 Param { get; }
        T2 Param2 { get; }
    }
}
