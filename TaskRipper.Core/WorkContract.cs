using System.Threading.Tasks.Dataflow;

namespace TaskRipper.Core
{
    public delegate TResult TaskRipperDelegate<TRequest, TResult>(TRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Represents the contract for executing work.
    /// </summary>
    public interface IWorkContract
    {
        IExecutionSettings ExecutionSettings { get; }
        int IterationsRequested { get; }
    }

    public sealed class DelegateBuilder<TRequest, TResult>
    {
        /// <summary>
        /// Action that runs every iteration regardless of conditions.
        /// </summary>
        private Func<TRequest,TResult> ExecutingFunction { get; set; }

        /// <summary>
        /// Action that runs every before <see cref="ExecutingFunction"/>
        /// </summary>
        private Action PreAction { get; set; }

        /// <summary>
        /// Action that runs every after <see cref="ExecutingFunction"/>
        /// </summary>
        private Action PostAction { get; set; }

        /// <summary>
        /// Action that runs every after <see cref="ExecutingFunction"/>
        /// </summary>
        private Func<TRequest, bool> ConditionalAction { get; set; }

        private CancellationToken CancellationToken { get; set; }
        private Func<TRequest> RequestBuilder { get; set; }

        /// <summary>
        /// Request 
        /// </summary>
        public DelegateBuilder<TRequest, TResult> WithRequestBuilder(Func<TRequest> requestBuilder)
        {
            RequestBuilder = requestBuilder ?? throw new ArgumentNullException(nameof(requestBuilder));
            return this;
        }

        public DelegateBuilder<TRequest, TResult> WithExecutingFunction(Func<TRequest, TResult> function)
        {
            ExecutingFunction = function ?? throw new ArgumentNullException(nameof(function));
            return this;
        }

        public DelegateBuilder<TRequest, TResult> WithPreAction(Action action)
        {
            PreAction = action ?? throw new ArgumentNullException(nameof(action));
            return this;
        }

        public DelegateBuilder<TRequest, TResult> WithPostAction(Action action)
        {
            PostAction = action ?? throw new ArgumentNullException(nameof(action));
            return this;
        }

        public DelegateBuilder<TRequest, TResult> WithConditionalAction(Func<TRequest, bool> condition)
        {
            ConditionalAction = condition ?? throw new ArgumentNullException(nameof(condition));
            return this;
        }

        public DelegateBuilder<TRequest, TResult> WithCancellationToken(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            return this;
        }

        public TaskRipperDelegate<TRequest, TResult> Build()
        {
            Guard.Against.Null(ExecutingFunction);
            return (request, token) =>
            {
                // Check if cancellation has been requested
                token.ThrowIfCancellationRequested();

                PreAction?.Invoke();

                if (ConditionalAction == null || ConditionalAction(request))
                {
                    TResult result = ExecutingFunction(request);

                    PostAction?.Invoke();

                    return result;
                }

                throw new InvalidOperationException("Conditional action failed.");
            };
        }
    }

    public class WorkContract : IWorkContract, IEquatable<WorkContract>
    {
        private WorkContract(int iterationsRequested = 1)
            : this(Core.ExecutionSettings.Default, iterationsRequested)
        { }
        private WorkContract(IExecutionSettings executionSettings, int iterationsRequested = 1)
        {
            ExecutionSettings = Guard.Against.Null(executionSettings);
            IterationsRequested = Guard.Against.NegativeOrZero(iterationsRequested);
        }

        /// <summary>
        /// The number of iterations requested to execute.
        /// </summary>
        public int IterationsRequested { get; }

        public IExecutionSettings ExecutionSettings { get; }

        /// <summary>
        /// Creates a work contract with default execution settings.
        /// <para>See <see cref="ExecutionSettings.Default"/></para> to get the default settings.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public static IWorkContract Create(int iterations)
        {
            var workContract = new WorkContract(iterations);
            return workContract;
        }

        public static IWorkContract Create([ValidatedNotNull, NotNull] IExecutionSettings executionSettings, int iterations)
        {
            var workContract = new WorkContract(executionSettings, iterations);
            return workContract;
        }

        public bool Equals(WorkContract? other)
        {
            if (other is null)
                return false;

            return IterationsRequested == other.IterationsRequested &&
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
            return HashCode.Combine(IterationsRequested, ExecutionSettings.GetHashCode());
        }
    }
}
