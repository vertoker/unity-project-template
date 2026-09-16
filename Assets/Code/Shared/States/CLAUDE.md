# CLAUDE.md — `Assets/Code/Shared/States`

Read `Assets/Code/Shared/CLAUDE.md` first — it carries the mental model, the folder index and the
layer-wide conventions. This file is folder-local.

## Mental model

A machine is a dictionary from **concrete state type** to one instance, plus a cursor. There is no
transition table: any registered state may follow any other, and a concrete machine subclasses
`StateMachine` to decide what "allowed" means by overriding `EnterInternal`.

## Conventions

- **`Enter` logs every refusal, `TryEnter` is silent.** Pick by whether failing to transition is a
  bug: a required transition uses `Enter` so the console says why it did not happen; an optional one
  uses `TryEnter` and reads the returned `bool`.
- `LogActions` is off by default and is meant to be flipped in a subclass while debugging one
  machine. A machine that transitions per frame floods the console with it on.

## Traps

- **States are keyed by their exact runtime type.** `Enter<TBase>()` where `TBase` is a base class or
  an interface never finds anything, because `Add` keyed the entry under the concrete class. The
  failure is a logged "unregistered state" for a state that is visibly registered — the fix is to
  name the concrete type, not to add another registration.
- **`Add(IState)` binds the state to the machine, `new MyState()` on its own does not.** A state
  built by hand and never registered has a null `StateMachine`, and the null surfaces inside
  `Enter()` rather than at construction.
- **Removing the current state exits it.** `Remove` calls `ExitCurrent` in that case, so the machine
  goes idle rather than holding a pointer to a state it no longer owns — a caller that expected the
  cursor to survive gets a null `CurrentState` instead.
