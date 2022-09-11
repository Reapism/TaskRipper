namespace TaskRipper.Core
{
    internal static class IWorkContractExtensions
    {
        /// <summary>
        /// Validates a <see cref="IWorkContract"/>.
        /// Ensures all dependencies are non null, and are
        /// in range.
        /// </summary>
        /// <param name="contract">The <see cref="IWorkContract"/> instance to validate.</param>
        /// <exception cref="ArgumentNullException">If any dependency is null.</exception>
        /// <exception cref="IterationsOutOfRangeException">Thrown if the iteration are out of range.</exception>
        /// <exception cref="ThreadOutOfRangeException">Thrown if the environment thread count is out of range.</exception>
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

        }
    }
}
