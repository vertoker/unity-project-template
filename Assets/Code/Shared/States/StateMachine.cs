using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Shared.States
{
    /// <summary>
    /// Dictionary-backed <see cref="IStateMachine"/>. States are keyed by their concrete type,
    /// so one machine holds at most one instance of each state class
    /// </summary>
    public abstract class StateMachine : IStateMachine
    {
        private readonly Dictionary<Type, IState> _states = new();

        /// <summary>
        /// Turns on the Enter/Exit trace; off by default because a machine that ticks per frame floods the console
        /// </summary>
        protected bool LogActions;

        /// <inheritdoc/>
        public IEnumerable<IState> States => _states.Values;

        /// <inheritdoc/>
        public IState CurrentState { get; private set; }

        /// <summary>
        /// Creates <typeparamref name="TState"/> and registers it; false when one is already registered
        /// </summary>
        public bool Add<TState>() where TState : State, new()
        {
            var state = Create<TState>();
            return Add(state);
        }

        /// <summary>
        /// Creates <typeparamref name="TState"/> bound to this machine without registering it
        /// </summary>
        public TState Create<TState>() where TState : State, new()
        {
            var state = new TState();
            state.StateMachine = this;
            return state;
        }

        /// <summary>
        /// The registered instance of <typeparamref name="TState"/>, or null with an error when there is none
        /// </summary>
        public TState Get<TState>() where TState : State
        {
            if (!_states.TryGetValue(typeof(TState), out var state))
            {
                GameLogger.LogError($"Can't get {typeof(TState).Name} from {GetType().Name}",
                    memberNameManual: GetType().Name);
                return null;
            }
            return (TState)state;
        }

        /// <summary>
        /// Registers an externally built state and binds it to this machine; false when its type is already registered
        /// </summary>
        public bool Add(IState state)
        {
            if (!_states.TryAdd(state.GetType(), state))
            {
                GameLogger.LogError($"Can't add already added {state.GetType().Name} into {GetType().Name}",
                    memberNameManual: GetType().Name);
                return false;
            }

            // Add<TState>() goes through Create<TState>(), but a hand-built state would keep a null machine
            if (state is State typedState)
                typedState.StateMachine = this;

            return true;
        }

        /// <summary>
        /// Unregisters a state, exiting it first when it is the current one; false when it was not registered
        /// </summary>
        public bool Remove(IState state)
        {
            if (!_states.Remove(state.GetType()))
            {
                GameLogger.LogError($"Can't remove already removed {state.GetType().Name} from {GetType().Name}",
                    memberNameManual: GetType().Name);
                return false;
            }

            // Otherwise CurrentState would keep pointing at a state the machine no longer owns
            if (ReferenceEquals(CurrentState, state))
                ExitCurrent();

            return true;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CurrentTypeIs<TState>() where TState : IState
        {
            return CurrentState != null && CurrentState.GetType() == typeof(TState);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CurrentTypeIs(Type stateType)
        {
            return CurrentState != null && stateType != null && CurrentState.GetType() == stateType;
        }

        /// <inheritdoc/>
        public bool TryEnter<TState>(bool allowCurrent = false) where TState : IState
        {
            if (!allowCurrent && CurrentTypeIs<TState>())
                return false;

            if (!_states.TryGetValue(typeof(TState), out var nextState))
                return false;

            EnterInternal(nextState);
            return true;
        }

        /// <inheritdoc/>
        public bool Enter<TState>(bool allowCurrent = false) where TState : IState
        {
            if (!allowCurrent && CurrentTypeIs<TState>())
            {
                GameLogger.LogWarning($"You try to reenter into current {CurrentState.GetType().Name}, " +
                                      "if this intended behavior, set allowCurrent = true",
                    memberNameManual: GetType().Name);
                return false;
            }

            var type = typeof(TState);
            if (!_states.TryGetValue(type, out var nextState))
            {
                GameLogger.LogError($"Can't enter unregistered state {type.Name} in {GetType().Name}",
                    memberNameManual: GetType().Name);
                return false;
            }

            EnterInternal(nextState);
            return true;
        }

        /// <inheritdoc/>
        public virtual void ExitCurrent()
        {
            if (CurrentState == null) return;

            if (LogActions)
                GameLogger.Log($"Exit {CurrentState.GetType().Name}", memberNameManual: GetType().Name);

            CurrentState.Exit();
            CurrentState = null;
        }

        /// <summary>
        /// The one transition path: leaves the current state, then enters <paramref name="nextState"/>.
        /// Override to hook every transition of a concrete machine
        /// </summary>
        protected virtual void EnterInternal(IState nextState)
        {
            var prevState = CurrentState;
            if (CurrentState != null)
                ExitCurrent();

            CurrentState = nextState;

            if (LogActions)
            {
                GameLogger.Log(prevState == null
                        ? $"Enter {CurrentState.GetType().Name}"
                        : $"Enter {CurrentState.GetType().Name} (prev {prevState.GetType().Name})",
                    memberNameManual: GetType().Name);
            }

            CurrentState.Enter();
        }
    }
}
