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
        private static IExecutionSettings defaultInstance;
        public static IExecutionSettings Default
        {
            get 
            {
                if (defaultInstance is null)
                    defaultInstance = new ExecutionSettings(new ExecutionEnvironment());

                return defaultInstance;
            }
        }

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
