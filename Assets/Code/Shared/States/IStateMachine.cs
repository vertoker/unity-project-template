using System;
using System.Collections.Generic;

namespace Shared.States
{
    /// <summary>
    /// A set of registered <see cref="IState"/>s of which at most one is current.
    /// States are keyed by their concrete type, so a base type or an interface never resolves
    /// </summary>
    public interface IStateMachine
    {
        /// <summary>
        /// Every registered state, in no particular order
        /// </summary>
        IEnumerable<IState> States { get; }

        /// <summary>
        /// The state that is currently entered, or null when the machine is idle
        /// </summary>
        IState CurrentState { get; }

        /// <summary>
        /// Whether <see cref="CurrentState"/> is exactly <typeparamref name="TState"/> — a derived state does not match
        /// </summary>
        bool CurrentTypeIs<TState>() where TState : IState;

        /// <summary>
        /// Whether <see cref="CurrentState"/> is exactly <paramref name="stateType"/> — a derived state does not match
        /// </summary>
        bool CurrentTypeIs(Type stateType);

        /// <summary>
        /// Enters <typeparamref name="TState"/> and reports whether it happened. Silent —
        /// an unregistered state is an expected answer here, use it for optional transitions
        /// </summary>
        bool TryEnter<TState>(bool allowCurrent = false) where TState : IState;

        /// <summary>
        /// Enters <typeparamref name="TState"/> and reports whether it happened. Logs every refusal —
        /// use it where failing to transition is a bug
        /// </summary>
        bool Enter<TState>(bool allowCurrent = false) where TState : IState;

        /// <summary>
        /// Exits <see cref="CurrentState"/> and leaves the machine idle; does nothing when already idle
        /// </summary>
        void ExitCurrent();
    }
}
