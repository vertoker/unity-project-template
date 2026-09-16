# CLAUDE.md — `Assets/Code/Shared/Async`

Read `Assets/Code/Shared/CLAUDE.md` first — it carries the mental model, the folder index and the
layer-wide conventions. This file is folder-local.

## Mental model

**One runner holds at most one call in flight.** Every `CallAsync`/`CallTaskAsync` cancels the
previous one before starting, so the runner is a slot, not a queue. With a non-zero `delay` that is
exactly a debounce: a burst of calls runs the action once, for the arguments of the last one. With
`delay = 0` it is a plain cancel-and-restart.

The arity overloads exist only so the action can take arguments; `AsyncRunner<T1, T2>` is
`AsyncRunner` with two more parameters threaded through and nothing else.

## Conventions

- `CallTaskAsync` when the caller awaits the result, `CallAsync` when it does not. `CallAsync` is
  fire-and-forget over the same body, so the two never drift.
- The action takes a `CancellationToken` and is expected to honour it. A long action that ignores the
  token keeps running after the next call has already started a second one.

## Traps

- **`OperationCanceledException` is swallowed on purpose.** It is how a run ends when a newer one
  takes over — the normal path, not a failure. Any other exception is left alone and surfaces through
  UniTask's unhandled exception handler.
- **`Dispose` cancels, and a disposed runner silently does nothing.** Calls after it return without
  running the action and without throwing. Dispose a runner alongside the rest of its owner's
  disposables; leaving it to the GC leaks the `CancellationTokenSource`.
- **The token is captured once per call, not read from the field.** Reading the field after an await
  could already see the next call's source — the previous run would then wait on a token that nobody
  is going to cancel.
