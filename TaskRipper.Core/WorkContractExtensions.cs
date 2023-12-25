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
            Guard.Against.Null(contract);
            Guard.Against.Null(contract.ExecutionSettings);
            Guard.Against.Null(contract.ExecutionSettings.ExecutionEnvironment);

            // Ensure iterations is within execution range.
            if (!contract.ExecutionSettings.ExecutionRange.IsInRange(contract.IterationsRequested))
            {
                throw new IterationsOutOfRangeException(contract, contract.IterationsRequested, nameof(contract.IterationsRequested));
            }
        }
    }
}
