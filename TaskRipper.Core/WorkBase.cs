namespace TaskRipper.Core
{
    public class WorkAction : WorkBase, IDelegateExecutor
    {
        public WorkAction(IWorkContract workContract, Action executingAction)
            : base(workContract, false)
        {
            ExecutingAction = executingAction ?? throw new ArgumentNullException(nameof(executingAction));
        }

        public Action ExecutingAction { get; }
        public void Execute()
        {
            ExecutingAction();
        }
    }

    public class WorkAction<T> : WorkBase, IDelegateExecutor
    {
        public WorkAction(IWorkContract workContract, Action<T> executingAction, T param, Action<T> mutatingStateMachine, bool mutateAfterExecution)
            : base(workContract, mutateAfterExecution)
        {
            ExecutingAction = executingAction ?? throw new ArgumentNullException(nameof(executingAction));
            Param = param;
            MutatingStateMachine = mutatingStateMachine;
        }

        public Action<T> ExecutingAction { get; }
        public T Param { get; }
        public Action<T> MutatingStateMachine { get; }

        public void Execute()
        {
            if (MutateAfterExecution)
            {
                ExecutingAction(Param);
                if (MutatingStateMachine is not null)
                    MutatingStateMachine(Param);

                return;
            }

            if (MutatingStateMachine is not null)
                MutatingStateMachine(Param);

            ExecutingAction(Param);
        }
    }

    public class WorkAction<T1, T2> : WorkBase, IDelegateExecutor
    {
        public WorkAction(IWorkContract workContract, Action<T1, T2> executingAction, T1 param, T2 param2, Action<T1, T2> mutatingStateMachine, bool mutateAfterExecution)
            : base(workContract, mutateAfterExecution)
        {
            ExecutingAction = executingAction ?? throw new ArgumentNullException(nameof(executingAction));
            Param = param;
            Param2 = param2;
            MutatingStateMachine = mutatingStateMachine;
        }

        public Action<T1, T2> ExecutingAction { get; }
        public T1 Param { get; }
        public T2 Param2 { get; }
        public Action<T1, T2> MutatingStateMachine { get; }

        public void Execute()
        {
            if (MutateAfterExecution)
            {
                ExecutingAction(Param, Param2);
                if (MutatingStateMachine is not null)
                    MutatingStateMachine(Param, Param2);
                return;
            }

            if (MutatingStateMachine is not null)
                MutatingStateMachine(Param, Param2);

            ExecutingAction(Param, Param2);
        }
    }

    public class WorkFunction<TResult> : WorkBase, IReturnableDelegateExecutor<TResult>
    {
        public WorkFunction(IWorkContract workContract, Func<TResult> executingFunction)
            : base(workContract, false)
        {
            ExecutingFunction = executingFunction ?? throw new ArgumentNullException(nameof(executingFunction));
        }

        public Func<TResult> ExecutingFunction { get; }

        public TResult Execute()
        {
            return ExecutingFunction();
        }
    }

    public class WorkFunction<T, TResult> : WorkBase, IReturnableDelegateExecutor<TResult>
    {
        public WorkFunction(IWorkContract workContract, Func<T, TResult> executingFunction, T param, Action<T> mutatingStateMachine, bool mutateAfterExecution)
            : base(workContract, mutateAfterExecution)
        {
            ExecutingFunction = executingFunction ?? throw new ArgumentNullException(nameof(executingFunction));
            Param = param;
            MutatingStateMachine = mutatingStateMachine;
        }

        public Func<T, TResult> ExecutingFunction { get; }
        public T Param { get; }
        public Action<T> MutatingStateMachine { get; }

        public TResult Execute()
        {
            if (MutateAfterExecution)
            {
                TResult? resultBeforeMutation = ExecutingFunction(Param);
                if (MutatingStateMachine is not null)
                    MutatingStateMachine(Param);

                return resultBeforeMutation;
            }

            if (MutatingStateMachine is not null)
                MutatingStateMachine(Param);
            TResult? resultAfterMutation = ExecutingFunction(Param);
            return resultAfterMutation;
        }
    }

    public class WorkFunction<T1, T2, TResult> : WorkBase, IReturnableDelegateExecutor<TResult>
    {
        public WorkFunction(IWorkContract workContract, Func<T1, T2, TResult> executingFunction, T1 param, T2 param2, Action<T1, T2> mutatingStateMachine, bool mutateAfterExecution)
            : base(workContract, mutateAfterExecution)
        {
            ExecutingFunction = executingFunction ?? throw new ArgumentNullException(nameof(executingFunction));
            Param = param;
            Param2 = param2;
            MutatingStateMachine = mutatingStateMachine;
        }

        public Func<T1, T2, TResult> ExecutingFunction { get; }
        public T1 Param { get; }
        public T2 Param2 { get; }
        public Action<T1, T2> MutatingStateMachine { get; }

        public TResult Execute()
        {
            if (MutateAfterExecution)
            {
                TResult? resultBeforeMutation = ExecutingFunction(Param, Param2);
                if (MutatingStateMachine is not null)
                    MutatingStateMachine(Param, Param2);

                return resultBeforeMutation;
            }

            if (MutatingStateMachine is not null)
                MutatingStateMachine(Param, Param2);

            TResult? resultAfterMutation = ExecutingFunction(Param, Param2);
            return resultAfterMutation;
        }
    }

    public abstract class WorkBase
    {
        public WorkBase(IWorkContract workContract, bool mutateAfterExecution)
        {
            workContract.ValidateContract();

            WorkContract = workContract;
            MutateAfterExecution = mutateAfterExecution;
        }

        /// <summary>
        /// The work contract.
        /// </summary>
        public IWorkContract WorkContract { get; }

        /// <summary>
        /// Determines whether the mutation function should execute before 
        /// or after an iteration of executing the <see cref="ExecutingAction"/>
        /// delegate.
        /// <para>If null, it is not applicable in a derived type.</para>
        /// </summary>
        public bool MutateAfterExecution { get; }
    }

    public interface IReturnableDelegateExecutor<TResult>
    {
        TResult Execute();
    }

    public interface IDelegateExecutor
    {
        void Execute();
    }
}
