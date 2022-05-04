namespace TaskRipper.Core
{
    /// <summary>
    /// A class that helps wrap the delegate to execute N times, and the optional delegate that 
    /// mutates values of the parameters of the executing delegate depending on any criteria relevant
    /// to the work being performed.
    /// </summary>
    public class Work<T>
    {
        public Work(Action<T> executingAction, Action<T>? mutatingStateMachine)
        {
            ExecutingAction = executingAction;
            MutatingStateMachine = mutatingStateMachine;
        }
        Action<T> ExecutingAction { get; }
        /// <summary>
        /// Gets an optional delegate that is called either before or after the <see cref="ExecutingAction"/>
        /// is executed in order to change the state of the parameters used in the <see cref="ExecutingAction"/>.
        /// <para>See <see cref="MutateAfterExecution"/> to modify whether the mutating state machine should be executed
        /// before or after the execution of the <see cref="ExecutingAction"/>.</para>
        /// </summary>
        Action<T>? MutatingStateMachine { get; }


        /// <summary>
        /// Determines whether the mutation function should execute before 
        /// or after an iteration of executing the <see cref="ExecutingAction"/>
        /// delegate.
        /// </summary>
        bool MutateAfterExecution { get; }
    }
}
