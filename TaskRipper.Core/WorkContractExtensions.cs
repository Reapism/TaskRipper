namespace TaskRipper.Core
{
    internal static class IWorkContractExtensions
    {
        internal static void ValidateContract(this IWorkContract contract)
        {
            // Ensure all dependencies are non null
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            if (contract.ExecutionSettings is null)
                throw new ArgumentNullException(nameof(contract.ExecutionSettings));

            if (contract.ExecutionSettings.ExecutionEnvironment is null)
                throw new ArgumentNullException(nameof(contract.ExecutionSettings.ExecutionEnvironment));

            // Ensure iterations is within execution range.
            if (contract.Iterations > contract.ExecutionSettings.ExecutionRange.End.Value ||
                contract.Iterations <= contract.ExecutionSettings.ExecutionRange.Start.Value)
            {
                throw new IterationsOutOfRangeException(contract, contract.Iterations, nameof(contract.Iterations));
            }
        }
    }
}
