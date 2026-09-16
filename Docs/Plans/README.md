# Plans

**This is how a feature gets built here.** A capable model writes the plan; a cheaper, faster model
executes it. The plan is the handoff document between the two, and it is written for someone who was
not in the conversation that produced it.

A plan is not required for ordinary bug fixing — a mid-tier model reads the code, finds the cause and
fixes it. Reach for a plan when the work spans several files, changes an interface, adds a system, or
has an order that matters.

## Status lives in the filename

| Filename | Means |
|---|---|
| `NAME_plan.md` | **active** — being executed, or waiting to be |
| `NAME_plan_completed.md` | executed and verified |
| `NAME_plan_failed.md` | attempted, did not work, reason recorded inside |
| `NAME_plan_cancelled.md` | dropped before or during execution |

No suffix means active. That is the only state you have to remember; everything else is a suffix
added when the plan stops being work-in-progress.

**Completed plans stay.** A finished plan is the record of *why* a feature is shaped the way it is —
which is the question a diff cannot answer. Deleting it costs that, and `Docs/adr.md` is for
decisions, not for the sequence of work that implemented one.

Names are lowercase `snake_case`, matching `Docs/Issues/`.

## What a plan contains

Written on the assumption that the executor has the repository and nothing else:

- **Goal** — one sentence on what will be true afterwards.
- **Context** — why this is being built, what it replaces, what it must not break.
- **Files** — exact paths, created or modified.
- **Steps** — small, ordered, each independently checkable. A step that cannot be verified is a step
  that will be reported done without being done.
- **Verification** — the commands and the expected output. Not "make sure it works".
- **Out of scope** — what a reasonable reader might assume is included and is not.

No placeholders, no "TBD", no "handle edge cases". A step the executing model has to interpret is a
step the planning model did not finish.

## Executing one

Work top to bottom. When the plan turns out to be wrong — and it sometimes is, because it was written
without running the code — **stop and say so rather than improvising around it.** The cheaper model
guessing at intent is exactly the failure this split is meant to prevent.

Mark the status in the filename when execution ends, whichever way it ended. A plan left active after
the work is done is one the next session will try to execute again.
