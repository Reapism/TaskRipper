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
            : base(message: $"The value of [{iterations}] is out of current execution range of [{workContract.ExecutionSettings.ExecutionRange.Start}..{workContract.ExecutionSettings.ExecutionRange.End}]. Parameter name [{parameterName}]")
        {
        }
    }


    public class DateOutOfRangeException : TaskRipperException
    {
        public DateOutOfRangeException(IDateRange dateRange)
            : base(message: $"The end date: [{dateRange.EndDate}] must be greater than the start date: [{dateRange.StartDate}].")
        {

        }

        public DateOutOfRangeException(DateTime startDate, DateTime endDate)
            : base(message: $"The end date: [{endDate}] must be greater than the start date: [{startDate}].")
        {

        }
    }
}
