---
name: unity-md-improver
description: Audit this project's CLAUDE.md hierarchy and Docs/ for drift, contradiction and dead references, score them, report before editing, then make targeted fixes. Use when documentation has fallen behind the code, when a session followed an instruction that turned out to be wrong, after a large refactor, or when asked to check or improve the project's agent instructions.
---

# Auditing the documentation

Four phases, in order. **Phase 3 is a report, and it happens before any edit** — an audit that
silently rewrites things cannot be disagreed with.

## Phase 1 — Discover

- The root `CLAUDE.md`, and every per-folder `CLAUDE.md` under `Assets/`.
- `Docs/**`, including `Docs/Issues/`.
- `.claude/rules/*.md` and `.claude/skills/*/SKILL.md`.

Record which files are **empty**. In this project an empty `CLAUDE.md` is a deliberate reserved slot,
not a defect — see the scoring exemption below.

## Phase 2 — Score

Six criteria, 100 points, then penalties.

| # | Criterion | Points | Full marks means |
|---|---|---|---|
| 1 | Commands and processes | 20 | Everything needed to build, run and check the project is present, copy-pastable, and works as written |
| 2 | Architectural clarity | 20 | Folders, layers, dependency direction and entry points are all derivable from the file |
| 3 | Non-obvious knowledge | 15 | Traps, tool limitations, rejected options and the reason each lost |
| 4 | Concision | 15 | Dense; every line carries information |
| 5 | Currency | 15 | Matches the repository as it stands right now |
| 6 | Actionability | 15 | A reader can act on it without asking a follow-up question |

Two rules that decide most disputes:

- **A command or path that does not exist in the project is not a gap — it is misinformation, and
  that is worse than saying nothing.** Verify criteria 1 and 5 by actually checking: do the named
  folders, files, packages and menu items exist?
- **Restating what is visible in the code is negative value.** So are generic best practices. They
  consume context and dilute the real rules. Score criterion 4 against that, not against length.

Criterion 3 is the most valuable section of any instruction file: everything else a session can
derive on its own, and this it cannot.

### Penalties

| Penalty | For |
|---|---|
| −15 | A contradiction between two levels of the hierarchy — root says one thing, a folder file another |
| −10 | The root `CLAUDE.md` exceeding 300 lines |
| −10 | A dead reference: a named file, folder, menu item or command that does not exist |
| −5 | Secrets, tokens or credentials in a documentation file |

### Exemptions — do not penalise these

- **An empty `CLAUDE.md`.** It is a reserved slot for a folder that does not yet hold knowledge
  beyond its code. Score it as not-applicable, not as zero. The threshold is in the `new-claude-md`
  skill. Report it only if the folder has *since* accumulated such knowledge.
- **`Docs/Issues/` entries describing work that was never built.** That is what the folder is for;
  the status line at the top of each file says which kind it is.
- **A `README.md` that is only a title and two sentences.** That is its finished state, set by
  `INIT_PLAN.md` on the way out. The README addresses people who are not in a session; `CLAUDE.md`
  is the map for everyone who is. Do not score it against criteria 1 or 2, and never move content
  into it to make it look complete.
- **Four things that look like dead references and are not.** Check for these before writing a −10;
  a naive path check reports all of them:
  - **Unity menu paths.** `Assets/Create/Scripting/AssemblyBuilder/…` and `Tools/AssemblyBuilder/…`
    are menu items, not files. A path starting `Assets/Create/` is always a menu.
  - **Files INIT_PLAN creates.** `.claude/rules/unity_editor.md`,
    `.claude/skills/unity-verify/SKILL.md`, both `Metadata.cs` and
    `Shared/Constants/EditorNames.cs` do not exist in an uninitialised template and are supposed to.
    They are a finding only *after* initialisation.
  - **Illustrative paths.** `Shared/Async`, `Systems/ItemSystem/Models` and `Runtime/UI` appear in
    the namespace examples and are not claims that those folders exist.
  - **Example filenames** in the `Docs/Issues/` taxonomy table.

## Phase 2b — Unity-specific checks

These are what make this an audit of *this* project rather than a generic one.

1. **Manifest against the "Installed but not used" invariant.** The root file must not carry a list
   of unused packages — that would be a copy of `Packages/manifest.json`, i.e. machine-derivable. It
   must carry the invariant. Check that every package named in the Technical overview table is
   actually used by something, and that the table's "Where the reasoning lives" column holds only
   `Docs/dependencies.md`, `Docs/architecture.md` or `—`.
2. **Assembly graph against the described layers.** Compare the real `.asmdef` set and the builders
   in `Assets/Code/Architecture/` against `Docs/architecture.md`. A builder with no assembly, an
   assembly with no builder, or an assembly missing from its layer collection is a finding — all
   three are silent at runtime.
3. **`rootNamespace` present and equal to `name`** in every `.asmdef`. It is what makes the namespace
   rule real rather than declarative (`Docs/adr.md`).
4. **Every assembly folder has a `CLAUDE.md`**, empty or not. A missing one means the slot was never
   reserved.
5. **`.claude/rules/` coverage by extension.** For each rule, check its `paths` globs still match
   something, and that no major file type in the project is governed by nothing.
6. **Always-loaded set.** `.claude/rules/unity_editor.md` must have no `paths` (it loads always);
   every other rule must have them. Confirm against `/context` in a session that what loads matches
   what is expected — and say plainly if you could not run that check.
7. **Rule numbers.** The eleven are a fixed vocabulary cited from `Docs/` and the folder files. Any
   citation of a number whose heading has changed meaning is a −15 contradiction.

## Phase 3 — Report, before editing

Per file: score, the two or three findings that actually matter, and the penalty lines. Then a
summary ordered by what costs a session the most time. State explicitly what you could not verify —
anything needing a running Editor or a fresh session belongs in that list rather than being assumed.

**Do not edit yet.**

## Phase 4 — Targeted edits

Only what was reported and agreed. Rules:

- Fix dead references and contradictions first; they actively mislead.
- Cut rather than rewrite where criterion 4 failed. Deleting a paragraph that restates the code is
  the highest-value edit available.
- Never add a command you have not verified.
- Never fill an empty `CLAUDE.md` to make the audit look complete.
- Keep the root under 300 lines. If something has to give, it moves to `Docs/` and leaves a pointer.

Show the diff per file. Documentation edits are not committed — rule 2.
