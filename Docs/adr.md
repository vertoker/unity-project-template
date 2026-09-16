# Architecture decision records

One entry per decision that would otherwise be re-litigated. Newest first.

An entry earns its place when the decision is **not** recoverable from the code — when the next
reader would look at the result and see an arbitrary choice, or a mistake. A decision whose reasoning
is obvious from the diff does not need one.

Dates are absolute. "Recently" and "currently" rot.

## Format

```
## YYYY-MM-DD — TITLE

**Context.** What was true that forced a choice. The constraint, not the wish.

**Decision.** What was chosen, stated so it can be checked against the code.

**Alternatives.** What else was on the table and the specific reason each lost. An alternative with
no stated reason reads as one nobody thought of, and gets proposed again.

**Consequences.** What this costs, what it now forbids, and what would have to change to revisit it.
```

Long reasoning belongs here rather than in a comment: the code keeps a one-line pointer, this file
keeps the argument. Something that was tried, measured and rejected belongs in `Docs/Issues/`
instead — that folder exists so a refutation has somewhere to live.

---

## 2026-09-16 — `rootNamespace` in the `.asmdef`, not a namespace provider in the IDE

**Context.** The layout rule is "namespace = folder path, minus the `Systems` segment". Something has
to actually enforce that, or it stays a sentence in a document while the IDE generates
`Systems.ItemSystem.Models` on every new file and each developer silently fixes it by hand. The
donor project this template draws from solved it with per-solution ReSharper settings that declare
which folders become namespace segments.

**Decision.** Set `rootNamespace` on all six `.asmdef` files to the assembly's own name. The IDE then
composes `rootNamespace` + the path *below that `.asmdef`*, and `Systems/` — which sits above the
system's `.asmdef` — never enters the path. The `.DotSettings` file is kept, but only for project
abbreviations.

**Alternatives.**

- *A namespace provider in `.DotSettings`.* Works, but only in Rider/ReSharper, and only for
  developers who have the shared settings file loaded. It also means the rule lives in IDE
  configuration, where nothing in the build can check it.
- *Leaving `rootNamespace` empty and relying on discipline.* This was the state before this entry.
  It made the rule a declaration that the IDE actively argued with on every new file.

**Consequences.** `AssemblyBuilder` knows `rootNamespace` and preserves it across `Build All`, so the
setting survives regeneration — but it survives *because* the field is one of the twelve the builder's
model knows, which is the same reason unknown fields are dropped. A new assembly must set its own
`rootNamespace`; the `add-assembly` skill carries that step. Revisiting this means moving the rule
back into IDE configuration and accepting that it stops applying outside one IDE.

The surviving `.DotSettings` file is named after the solution, which Unity names after the **project
folder**. Renaming the project therefore orphans it silently — nothing breaks, the abbreviations
simply stop applying. Renaming that file is a step in `INIT_PLAN.md` phase 5 for exactly this reason.
