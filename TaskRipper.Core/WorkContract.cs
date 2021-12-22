namespace TaskRipper.Core
{

    public class WorkContract : IWorkContract
    {
        public WorkContract(string description, int iterations = 1)
            : this(Core.ExecutionSettings.Default, description, iterations)
        {

        }
        public WorkContract(IExecutionSettings executionSettings, string description, int iterations = 1)
        {
            ExecutionSettings = executionSettings ?? throw new ArgumentNullException(nameof(executionSettings));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Iterations = iterations > 0 ? iterations : 1;
        }

        public string Description { get; }

        public int Iterations { get; }

        public IExecutionSettings ExecutionSettings { get; }
    }
}
