namespace TaskRipper.Core
{
    internal static class IWorkContractExtensions
    {
        internal static void ValidateContract(this IWorkContract contract)
        {
            // Ensure all dependencies are non null
            if (contract is null)
                throw new ArgumentNullException(nameof(contract));

            if (contract.ExecutionSettings is null)
                throw new ArgumentNullException(nameof(contract.ExecutionSettings));

            if (contract.ExecutionSettings.ExecutionEnvironment is null)
                throw new ArgumentNullException(nameof(contract.ExecutionSettings.ExecutionEnvironment));

            // Ensure iterations is within execution range.
            if (!contract.ExecutionSettings.ExecutionRange.IsInRange(contract.Iterations))
            {
                throw new IterationsOutOfRangeException(contract, contract.Iterations, nameof(contract.Iterations));
            }

            // Ensure the execution environment thread count is in range of the declared thread range in the contract.
            if (!contract.ExecutionSettings.ThreadRange.IsInRange(contract.ExecutionSettings.ExecutionEnvironment.ThreadCount))
            {
                throw new ThreadOutOfRangeException(contract,
                    contract.ExecutionSettings.ExecutionEnvironment.ThreadCount,
                    nameof(contract.ExecutionSettings.ExecutionEnvironment.ThreadCount));
            }
        }
    }
}
