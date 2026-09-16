namespace Shared.States
{
    /// <summary>
    /// Base <see cref="IState"/> that knows the machine holding it.
    /// <see cref="StateMachine"/> fills the back-reference in on registration
    /// </summary>
    public abstract class State : IState
    {
        /// <summary>
        /// The machine this state is registered in, or null while it is not registered anywhere
        /// </summary>
        public IStateMachine StateMachine { get; internal set; }

        /// <summary>
        /// Leaves <see cref="StateMachine"/> unset — the machine assigns it on registration
        /// </summary>
        protected State()
        {
        }

        /// <summary>
        /// Presets <see cref="StateMachine"/> for a state built outside its machine
        /// </summary>
        protected State(IStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        /// <inheritdoc/>
        public virtual void Enter()
        {
        }

        /// <inheritdoc/>
        public virtual void Exit()
        {
        }
    }
}
