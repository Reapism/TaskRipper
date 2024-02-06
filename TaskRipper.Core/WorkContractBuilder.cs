namespace TaskRipper.Core
{
    public class WorkContractBuilder
    {
        private IExecutionSettings _executionSettings;
        private CancellationToken _cancellationToken;
        private int _iterationRequested;
        private WorkBalancerOptions _workBalancerOptions;

        /// <summary>
        /// Adds the specified cancellation token to the contract.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to add.</param>
        /// <returns></returns>
        public WorkContractBuilder WithCancellationToken(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            return this;
        }

        /// <summary>
        /// Adds the specified <see cref="IExecutionSettings"/> to the contract.
        /// </summary>
        /// <param name="executionSettings"></param>
        /// <returns></returns>
        public WorkContractBuilder WithExecutionSettings(IExecutionSettings executionSettings)
        {
            _executionSettings = executionSettings;
            return this;
        }

        /// <summary>
        /// Adds the specified <see cref="IExecutionSettings"/> to the contract.
        /// </summary>
        /// <param name="executionSettings"></param>
        /// <returns></returns>
        public WorkContractBuilder UseDefaultExecutionSettings()
        {
            _executionSettings = ExecutionSettings.Default;
            return this;
        }

        public WorkContractBuilder WithIterations(int iterationsRequested)
        {
            _iterationRequested = iterationsRequested;
            return this;
        }

        public WorkContractBuilder WithWorkBalancingOptions(WorkBalancerOptions options)
        {
            _workBalancerOptions = options;
            return this;
        }

        public IWorkContract Build()
        {
            if (!IsBuildable())
                throw new InvalidOperationException();

            return WorkContract.Create(_executionSettings, _workBalancerOptions, _cancellationToken, _iterationRequested);
        }

        private bool IsBuildable()
        {
            if (_executionSettings is null)
                return false;

            if (_iterationRequested < 1)
                return false;

            return true;
        }
    }
}
