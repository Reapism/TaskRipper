namespace TaskRipper.Core
{
    public class ExecutionEnvironment : IExecutionEnvironment
    {
        public int ThreadCount { get => Environment.ProcessorCount; }
    }
}
