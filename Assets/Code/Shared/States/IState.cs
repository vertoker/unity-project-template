namespace Shared.States
{
    /// <summary>
    /// One state of an <see cref="IStateMachine"/>: everything it owns is set up in
    /// <see cref="Enter"/> and torn down in <see cref="Exit"/>
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Called once the machine has made this state current
        /// </summary>
        void Enter();

        /// <summary>
        /// Called before the machine leaves this state; must undo whatever <see cref="Enter"/> did
        /// </summary>
        void Exit();
    }
}
