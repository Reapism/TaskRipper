namespace TaskRipper.Core
{
    public interface IExecutionSettings
    {
        int MinThreadCount { get; }
        int MaxThreadCount { get; }

        int MaxExecutionCount { get; }
        
        int MinExecutionCount { get; }

        IExecutionEnvironment ExecutionEnvironment { get; }
    }

    public class ExecutionSettings : IExecutionSettings
    {
        public ExecutionSettings(IExecutionEnvironment executionEnvironment)
        {
            ExecutionEnvironment = executionEnvironment;
        }

        public int MinThreadCount { get; }

        public int MaxThreadCount { get; }

        public int MaxExecutionCount { get; }

        public int MinExecutionCount { get; }

        public IExecutionEnvironment ExecutionEnvironment { get; }
    }
}
