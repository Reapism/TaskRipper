namespace TaskRipper.Core
{
    /// <summary>
    /// The base exception class for all <see cref="TaskRipper"/>
    /// exceptions.
    /// </summary>
    public class TaskRipperException : Exception
    {
        public TaskRipperException(string message)
            : base(message)
        {
        }
        public TaskRipperException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class IterationsOutOfRangeException : TaskRipperException
    {
        public IterationsOutOfRangeException(IWorkContract workContract, int iterations, string parameterName)
            : base(message: $"The value of [{iterations}] is out of range. Parameter name [{parameterName}]")
        {
        }
    }
}
