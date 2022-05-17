namespace TaskRipper.Core
{

    public class WorkContract : IWorkContract, IEquatable<WorkContract>
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

        /// <summary>
        /// Creates a work contract with default execution settings.
        /// <para>See <see cref="ExecutionSettings.Default"/></para> to get the default settings.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public static IWorkContract Create(string description, int iterations)
        {
            var workContract = new WorkContract(Core.ExecutionSettings.Default, description, iterations);
            return workContract;
        }

        public static IWorkContract Create(IExecutionSettings executionSettings, string description, int iterations)
        {
            var workContract = new WorkContract(executionSettings, description, iterations);
            return workContract;
        }

        public bool Equals(WorkContract? other)
        {
            if (other is null)
                return false;

            return Iterations == other.Iterations &&
                Description.Equals(other.Description) &&
                ExecutionSettings.Equals(other.ExecutionSettings);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (obj is WorkContract other)
                return Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Iterations, Description, ExecutionSettings.GetHashCode());
        }
    }
}
