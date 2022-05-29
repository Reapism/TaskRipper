using System.Reflection;

namespace TaskRipper.Core
{
    /// <summary>
    /// A class that helps wrap the delegate to execute N times, and the optional delegate that 
    /// mutates values of the parameters of the executing delegate depending on any criteria relevant
    /// to the work being performed.
    /// </summary>
    /// 
    internal class TypeValue
    {
        /// <summary>
        /// Instantiates a <see cref="TypeValue"/> object using <see cref="ParameterInfo"/>
        /// and <see cref="ParameterValue"/>
        /// </summary>
        /// <param name="parameterInfo"></param>
        /// <param name="parameterValue"></param>
        /// <exception cref="TypeMismatchException">Thrown if the incoming <paramref name="parameterInfo"/>
        /// type does not match the <paramref name="parameterValue"/> type.</exception>
        internal TypeValue(ParameterInfo parameterInfo, object parameterValue)
        {
            var valueType = parameterValue.GetType();
            if (parameterInfo.ParameterType != valueType)
            {
                throw new TypeMismatchException(parameterInfo.ParameterType, valueType);
            }

            ParameterInfo = parameterInfo;
            ParameterValue = parameterValue;
        }

        internal ParameterInfo ParameterInfo { get; set; }
        internal object ParameterValue { get; set; }

        internal static TypeValue[] From(ParameterInfo[] parameterInfos, object[]? args)
        {
            var typeValues = new TypeValue[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                var parameterInfo = parameterInfos[i];
                var arg = args[i];

                typeValues[i] = new TypeValue(parameterInfo, arg);
            }

            return typeValues;
        }
    }

    public abstract class Work
    {
        public Work(bool? mutateAfterExecution)
        {
            MutateAfterExecution = mutateAfterExecution;
        }

        /// <summary>
        /// Determines whether the mutation function should execute before 
        /// or after an iteration of executing the <see cref="ExecutingAction"/>
        /// delegate.
        /// <para>If null, it is not applicable in a derrived type.</para>
        /// </summary>
        public bool? MutateAfterExecution { get; }

        /// <summary>
        /// Provides a contract for handling the execution of a delegate, 
        /// and the execution of the mutating state machine.
        /// </summary>
        public abstract void Execute();
        public abstract Task ExecuteAsync();
    }

    public abstract class Work<TResult>
    {
        public Work(bool? mutateAfterExecution)
        {
            MutateAfterExecution = mutateAfterExecution;
        }

        /// <summary>
        /// Determines whether the mutation function should execute before 
        /// or after an iteration of executing the <see cref="ExecutingAction"/>
        /// delegate.
        /// <para>If null, it is not applicable in a derrived type.</para>
        /// </summary>
        public bool? MutateAfterExecution { get; }

        /// <summary>
        /// Provides a contract for handling the execution of a delegate, 
        /// and the execution of the mutating state machine.
        /// </summary>
        public abstract TResult Execute();
        public abstract Task<TResult> ExecuteAsync();
    }

    public class WorkAction : Work
    {
        private readonly Action executingAction;

        public WorkAction(Action executingAction)
            : base(null)
        {
            this.executingAction = executingAction;
        }

        public override void Execute()
        {
            executingAction();
        }

        public override async Task ExecuteAsync()
        {
            executingAction();
        }
    }

    public class WorkAction<T> : Work
    {
        private readonly Action<T> executingAction;
        private readonly T param;
        private readonly Action<T>? mutatingStateMachine;

        /// <summary>
        /// Use the <see cref="WorkBuilder"/> instance to construct instances.
        /// </summary>
        /// <param name="executingAction"></param>
        /// <param name="param"></param>
        /// <param name="mutatingStateMachine"></param>
        /// <param name="mutateAfterExecution"></param>
        public WorkAction(Action<T> executingAction, T param, Action<T>? mutatingStateMachine, bool mutateAfterExecution)
            : base(mutateAfterExecution)
        {
            this.executingAction = executingAction;
            this.param = param;
            this.mutatingStateMachine = mutatingStateMachine;
            
            // TODO REMOVE NULL CHECK IF POSSIBLE?
            // At this point, we know that mutating state machine will always be null.
            // 
            if (mutatingStateMachine is null)
            {

            }
        }

        public override void Execute()
        {
            // TODO REMOVE NULL CHECK IF POSSIBLE?
            // https://stackoverflow.com/a/1861751/8705563
            // if mutating state machine is null, since its readonly, can we avoid the check?
            // what pattern would allow us to avoid this null check for N iterations?
            if (MutateAfterExecution.HasValue && MutateAfterExecution.Value)
            {
                executingAction(param);

                if (mutatingStateMachine is not null)
                    mutatingStateMachine(param);
            }
            else
            {
                if (mutatingStateMachine is not null)
                    mutatingStateMachine(param);

                executingAction(param);
            }
        }

        // TODO REMOVE NULL CHECK IF POSSIBLE?
        // Here we can execute the action internally hopefully avoiding null checks.
        private void ExecuteInternal()
        {

        }

        public override Task ExecuteAsync()
        {
            Execute();
            return Task.CompletedTask;
        }
    }

    public class WorkAction<T1, T2> : Work
    {
        private readonly Action<T1, T2> executingAction;
        private readonly T1 param;
        private readonly T2 param2;
        private readonly Action<T1, T2>? mutatingStateMachine;

        public WorkAction(Action<T1, T2> executingAction, T1 param, T2 param2, Action<T1, T2>? mutatingStateMachine, bool mutateAfterExecution)
            : base(mutateAfterExecution)
        {
            this.executingAction = executingAction;
            this.param = param;
            this.param2 = param2;
            this.mutatingStateMachine = mutatingStateMachine;
        }

        public override void Execute()
        {
            if (MutateAfterExecution.HasValue && MutateAfterExecution.Value)
            {
                executingAction(param, param2);

                if (mutatingStateMachine is not null)
                    mutatingStateMachine(param, param2);
            }
            else
            {
                if (mutatingStateMachine is not null)
                    mutatingStateMachine(param, param2);

                executingAction(param, param2);
            }
        }

        public override Task ExecuteAsync()
        {
            Execute();
            return Task.CompletedTask;
        }
    }

    public class WorkFunction<TResult> : Work<TResult>
    {
        private readonly Func<TResult> executingFunction;

        public WorkFunction(Func<TResult> executingFunction, bool mutateAfterExecution)
            : base(mutateAfterExecution)
        {
            this.executingFunction = executingFunction;
        }

        public override TResult Execute()
        {
            return executingFunction();
        }

        public override Task<TResult> ExecuteAsync()
        {
            return Task.FromResult(executingFunction());
        }
    }

    public class WorkFunction<T, TResult> : Work<TResult>
    {
        private readonly Func<T, TResult> executingFunction;
        private readonly T param;
        private readonly Action<T>? mutatingStateMachine;

        public WorkFunction(Func<T, TResult> executingFunction, T param, Action<T>? mutatingStateMachine, bool mutateAfterExecution)
            : base(mutateAfterExecution)
        {
            this.executingFunction = executingFunction;
            this.param = param;
            this.mutatingStateMachine = mutatingStateMachine;
        }

        public override TResult Execute()
        {
            if (MutateAfterExecution.HasValue && MutateAfterExecution.Value)
            {
                var result = executingFunction(param);

                if (mutatingStateMachine is not null)
                    mutatingStateMachine(param);
                return result;
            }
            else
            {
                if (mutatingStateMachine is not null)
                    mutatingStateMachine(param);

                var result = executingFunction(param);
                return result;
            }

        }

        public override Task<TResult> ExecuteAsync()
        {
            return Task.FromResult(Execute());
        }
    }

    public class WorkFunction<T1, T2, TResult> : Work<TResult>
    {
        private readonly Func<T1, T2, TResult> executingFunction;
        private readonly T1 param;
        private readonly T2 param2;
        private readonly Action<T1, T2>? mutatingStateMachine;

        public WorkFunction(Func<T1, T2, TResult> executingFunction, T1 param, T2 param2, Action<T1, T2>? mutatingStateMachine, bool mutateAfterExecution)
            : base(mutateAfterExecution)
        {
            this.executingFunction = executingFunction;
            this.param = param;
            this.param2 = param2;
            this.mutatingStateMachine = mutatingStateMachine;
        }

        public override TResult Execute()
        {
            if (MutateAfterExecution.HasValue && MutateAfterExecution.Value)
            {
                var result = executingFunction(param, param2);

                if (mutatingStateMachine is not null)
                    mutatingStateMachine(param, param2);

                return result;
            }
            else
            {
                if (mutatingStateMachine is not null)
                    mutatingStateMachine(param, param2);

                var result = executingFunction(param, param2);
                return result;
            }
        }

        public override Task<TResult> ExecuteAsync()
        {
            return Task.FromResult(Execute());
        }
    }

    /// <summary>
    /// An abstraction for representing an executing delegate and a mutating state machine. 
    /// This is the most versatile version of the <see cref="Work"/> instance. This implementation allows any delegate, however, 
    /// internally, due to this, there is a lot of boxing/unboxing of types and lots of 
    /// checks to ensure the incoming parameters are valid.
    /// <para>
    /// This should be used if you have a custom delegate that does not conform to any of
    /// <see cref="Action"/> or <see cref="Func{TResult}"/> delegate families. 
    /// </para>
    /// </summary>
    public abstract class WorkDelegate
    {
        private readonly object[]? executingDelegateArgs;
        private readonly bool mutateAfterExecution;
        private readonly DelegateInfoContext delegateInfoContext;

        private WorkDelegate(Delegate executingDelegate, object[]? executingDelegateArgs, Delegate? mutatingStateMachineDelegate, bool mutateAfterExecution)
        {
            // Using multicast delegates introduces so many more concerns.
            // have to check for parameters matching executing delegate params/return type
            // and also the incoming object[]? executingDelegateArgs...

            ExecutingDelegate = executingDelegate ?? throw new ArgumentNullException(nameof(executingDelegate));
            MutatingStateMachineDelegate = mutatingStateMachineDelegate;
            this.mutateAfterExecution = mutateAfterExecution;
            this.executingDelegateArgs = executingDelegateArgs;

            delegateInfoContext = ExtractMethodInfo(ExecutingDelegate, executingDelegateArgs);

            var i = 0;
            var typeValueDictionary = TypeValue.From
            (
                executingDelegate.Method.GetParameters(),
                executingDelegateArgs
            )
            .ToDictionary(tv => i++, tv => tv);

            var returnValue = ExecutingDelegate.DynamicInvoke(executingDelegateArgs);

            if (returnValue is null && delegateInfoContext.HasReturnType)
            {
                // return type is void, can ignore the value
            }

            // the type returned by the delegate vs the return type of the method do not match.
            // TODO, is this affected by interfaces
            if (!returnValue.GetType().Equals(delegateInfoContext.ReturnParameter.ParameterType))
            {
                throw new TypeMismatchException(delegateInfoContext.ReturnParameter.ParameterType, returnValue.GetType());
            }
        }

        private DelegateInfoContext ExtractMethodInfo(Delegate del, object[]? args)
        {
            var context = new DelegateInfoContext(del, args);
            return context;
        }

        public abstract object? Execute();

        public abstract Task<object?> ExecuteAsync();

        /// <summary>
        /// The delegate that is being executed.
        /// </summary>
        public Delegate ExecutingDelegate { get; }

        /// <summary>
        /// Gets an optional delegate that is called either before or after the <see cref="ExecutingAction"/>
        /// is executed in order to change the state of the parameters used in the <see cref="ExecutingAction"/>.
        /// <para>See <see cref="MutateAfterExecution"/> to modify whether the mutating state machine should be executed
        /// before or after the execution of the <see cref="ExecutingAction"/>.</para>
        /// </summary>
        public Delegate? MutatingStateMachineDelegate { get; }



        public DelegateInfoContext DelegateInfoContext { get; }
    }
}
