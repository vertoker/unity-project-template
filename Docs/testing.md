# Testing

Writing a test is `.claude/rules/tests.md`, which loads when you touch one. This file is about
choosing a run, and about what the suite does not cover.

Whether you may run tests at all, and through what, is **rule 4**.

## Four run modes

| Mode | What it runs | When |
|---|---|---|
| **Pinpoint** | the specific tests covering what changed | during the change, repeatedly |
| **Pinpoint assemblies** | the whole assembly (or two) the change lives in | once the change compiles |
| **Broad** | everything except the slowest category | before handing work back |
| **Everything** | the whole suite, all categories | before a release, and after anything touching the assembly graph |

**The mode is not chosen by how risky the change feels.** It is chosen by what the change could
reach. A one-line edit inside `Shared` is a Broad change, because everything depends on `Shared`; a
hundred-line edit confined to one screen is a Pinpoint-assemblies change. Feeling careless is not a
reason to run more, and feeling confident is not a reason to run less — trace the references.

After anything that touched the assembly graph, run **Everything** once: a lost reference compiles
fine in the assembly that still has it and fails only where it does not.

## Excluding the slowest tests

There is no exclude parameter when filtering by category — neither over MCP nor in batch mode. "Run
everything except `5_extreme`" is therefore spelled as an **include list of the other four**:
`1_very_easy`, `2_easy`, `3_normal`, `4_hard`.

Get that list wrong by one and the run silently covers less than you think, which is worse than not
running it, because the number it reports still looks like a pass. The scale itself, and the
five-second threshold that decides what is `5_extreme`, are in `.claude/rules/tests.md`.

## Reporting a run

Report the numbers the tool returned — total, passed, failed, skipped — and name the mode. "Tests
pass" without numbers is a claim rule 9 does not permit. If the total did not change after adding a
test, the run used a stale assembly and the result means nothing.

## What stays outside the suite

Automated tests answer whether the code does what it was written to do. They do not answer whether
it looks right, feels right, reads right on a device, or holds up in Play Mode. That half stays with
the author, so **end a task with concrete manual steps**: which scene to open, what to click, what
output confirms it worked.

Also outside: anything whose only failure mode is visual, anything requiring a real device or store
backend, and performance work — a test can tell you a regression happened, but the profiler is what
tells you where.
