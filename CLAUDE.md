# CLAUDE.md — `<project name>`

> **This project is not initialized yet.** Read `INIT_PLAN.md` and run it before doing any other
> work. Everything below still contains `<placeholders>`.

This file is a navigator, not an encyclopedia. It carries what every session needs and points at
where the rest lives; it is loaded into every session, so it stays short on purpose. The detail sits
in `Docs/`, in `.claude/rules/` (which load with the files they govern) and in the per-folder
`CLAUDE.md` files. Start from **The map** at the bottom.

## Project overview

`<project name>` — `<project description, 2-3 sentences>`

- **Genre:** `<genre>`
- **Target platforms:** `<target platforms>`
- **Distribution channels:** `<distribution channels>`
- **Engine:** Unity 6000.6.0f1

## Technical overview

| Area | Choice | Where the reasoning lives |
|---|---|---|
| Render pipeline | URP 17.6 | `Docs/dependencies.md` |
| Data-oriented core | Entities 6.6 + Entities.Graphics, Burst, Collections, Mathematics | `Docs/dependencies.md` |
| Content delivery | Addressables 2.11 | `Docs/dependencies.md` |
| Localization | `com.unity.localization` 1.5, string tables under `Assets/Addressables/` | `Docs/dependencies.md` |
| UI | UI Toolkit — `.uxml` / `.uss` / `.tss` | `Docs/architecture.md` |
| Input | Input System 1.20 | — |
| Dependency injection | VContainer | `Docs/architecture.md` |
| Async | UniTask | `Docs/dependencies.md` |
| Events | UniRx | `Docs/architecture.md` |
| Inspector attributes | NaughtyAttributes | — |
| Assembly graph | `com.vertoker.assemblybuilder` | `Docs/architecture.md` |
| Tests | Unity Test Framework 1.8 (NUnit) | `Docs/testing.md` |
| Third-party via NuGet | NuGetForUnity | `Docs/dependencies.md` |
| Multiple editors on one clone | ParrelSync | `Docs/dependencies.md` |
| Editor automation | `com.coplaydev.unity-mcp` | `Docs/troubleshooting.md` |

### Installed but not used

The package list is not a list of decisions. This project starts with a broad manifest and an empty
`Assets/Code`, so at the outset **nothing in it is load-bearing** — a package being present is not
permission to build a solution on it, and the absence of a row above is not a gap to fill. A package
earns its row in the table the moment it is first used deliberately, and the row names where the
reasoning lives. Until then, treat the manifest as inventory and `Docs/dependencies.md` as the record.

## Rules

**The eleven rules keep their numbers** — they are cited by number from `Docs/`, from
`.claude/rules/` and from the per-folder `CLAUDE.md` files, so the numbering is a fixed vocabulary.
Some are stated here in full; the rest have a one-line stub here and load with the files they govern.

### 1. Code and text first

`.cs` code and text (mostly `.md`) are the primary medium — put a change there whenever it can live
there. Unity serialized assets are edited in small, reported increments under
`.claude/rules/unity_assets.md`, which loads when you touch one.

### 2. No commit

**Never commit. Only the author commits.** This holds for every task, including ones that finish
cleanly and including `INIT_PLAN.md` itself. Offer a commit and stop there; the author decides
whether the work is worth one and what the message says.

### 3. Only English in the repo, `<preferred chat language>` in conversation

Everything written into the repository — comments, `.md`, identifiers, commit-ready text — is
English. Everything said to the author is `<preferred chat language>`. This is mandatory, not a
preference, and it does not soften when the source material is in another language.

<!-- RULE_4 — INIT_PLAN phase 3 replaces this line with InitTemp/Rule4/MODE/rule_4.md -->

### 5. Assemblies only through AssemblyBuilder

`references` in an `.asmdef` are generated, never hand-edited; the graph lives in
`Assets/Code/Architecture/`. Full rule: `.claude/rules/assemblies.md`.

### 6. Editor tooling: where it lives, how it is named, who runs it

An editor assembly sits next to the system it serves, its menu path is a constant, and a tool that
rewrites many assets is launched by a human. Full rule: `.claude/rules/editor_assemblies.md`.

### 7. UI Toolkit: the files are editable, only placement is fixed

`.uxml`, `.uss` and `.tss` are plain text and freely editable; what is fixed is **where** a
declaration lands. Full rule: `.claude/rules/ui_toolkit.md`.

### 8. Every test carries its own attributes

`[Author]` plus both `[Category]`s, and no `Tests/` folder exists without its own `Metadata.cs`.
A requirement at generation time, not a cleanup pass. Full rule: `.claude/rules/tests.md`.

### 9. Never claim a result the tool did not return

Report what you actually saw. "The tests pass" requires a run with numbers; "it compiles" requires a
compile that returned. An empty console is not a clean console — it is just as likely that nothing
was built. If a step was skipped or a tool failed, say so plainly and say what that leaves unknown.
A confident summary of an unverified state is the one failure mode that costs the author real time.

### 10. Response style: compressed shorthand

Dense, telegraphic replies — drop pronouns, articles and connective filler; expand only when asked.
No politeness formulas, no apologies, no narrating what you are about to do. Abbreviate rather than
spell out. This applies to reasoning shown to the author as much as to conclusions.

### 11. The formatter and the dependencies are not yours to initiate

Do not reformat files you were not asked to change, do not run a formatter across the project, and
do not add, remove or upgrade a package, a NuGet dependency or a Unity version on your own
initiative. Propose it, name the cost, and let the author decide. A diff whose bulk is reformatting
is an unreviewable diff.

### Rules that load with the files they govern

| File | Loads when you touch | Carries |
|---|---|---|
| `.claude/rules/unity_editor.md` | *always* | rule 4 in full, and the MCP procedure |
| `.claude/rules/unity_assets.md` | `.asset` `.prefab` `.unity` `.meta` `.mat` | what a small edit is, and what must be reported |
| `.claude/rules/ui_toolkit.md` | `.uxml` `.uss` `.tss` | where a declaration lands; the traps |
| `.claude/rules/tests.md` | `**/Tests/**/*.cs` | the attributes, `Metadata.cs`, the difficulty scale |
| `.claude/rules/assemblies.md` | `.asmdef`, `Assets/Code/Architecture/**` | the generated graph and how to change it |
| `.claude/rules/editor_assemblies.md` | `**/Editor/**/*.cs`, `*.Editor.asmdef` | placement, naming, menus, the editor-only fork |

## Architecture

Three layers, each depending only on the ones below it:

```
Shared      engine-light utilities, depends on nothing
  ↑
Systems     subsystems, one folder and one assembly each
  ↑
Runtime     the game: composition, screens, scenes
```

`Shared`, `Runtime` and **every system** may have their own `Tests` and `Editor` assemblies, which
repeat their parent's layer and inheritance.

**Namespace = the folder path, minus the `Systems` segment.** `Shared/Async` → `Shared.Async`;
`Systems/ItemSystem/Models` → `ItemSystem.Models`; `Runtime/UI` → `Runtime.UI`. The assembly name
matches the namespace root. This is not a convention you have to remember — each `.asmdef` carries
the matching `rootNamespace`, and the `Systems` segment drops out because it sits *above* the
system's own `.asmdef`.

**`Assets/Code/Architecture/` is not code and not DI.** It holds AssemblyBuilder assets — the graph
that generates `references` across every `.asmdef`. Details: `Assets/Code/Architecture/CLAUDE.md`.

Composition starts at `Assets/Scenes/Boot.unity` and flows into the VContainer scope.

## Conventions

- Events go through UniRx, not plain C# `event Action<T>`:
  `private readonly Subject<T> _x = new(); public IObservable<T> X => _x;` — and dispose the
  `Subject` alongside the type's other disposables.
- Features that need dependencies register in the relevant scope rather than resolving manually.
- Assembly reference changes go through `Assets/Code/Architecture/`, never by editing GUID lists.
- Logging goes through one project-wide entry point rather than scattered `Debug.Log` calls — it is
  the only place that can tell Editor formatting from device formatting. Introduce it in `Shared`
  the first time anything needs to log.
- Prefer generic, data-driven implementations over per-case special handling.
- Style, naming, comments, error handling: `Docs/code_style.md`. It is the long one, and it is the
  file to read before writing the first line of a new subsystem.

## The map

Read the row that matches the question you arrived with.

### Questions

| The question | The file |
|---|---|
| How do the layers fit together, and where does new code go? | `Docs/architecture.md` |
| How do I write, place, categorise and run a test? | `Docs/testing.md` |
| How should this code look — naming, abstractions, comments, errors? | `Docs/code_style.md` |
| How do I add a third-party library, and which mechanism? | `Docs/dependencies.md` |
| Something in Unity or MCP is behaving strangely | `Docs/troubleshooting.md` |
| Why was it built this way, and what was rejected? | `Docs/adr.md` |
| Was this already tried and measured? | `Docs/Issues/README.md` |
| Who is on this project, and whose name goes on a test? | `Docs/authors.md` |

### Layers

| Working in | Read |
|---|---|
| anything under `Assets/Code` | `Assets/Code/CLAUDE.md` — the layer map |
| a subsystem, or adding one | `Assets/Code/Systems/CLAUDE.md` |
| the assembly graph, an `.asmdef`, a builder asset | `Assets/Code/Architecture/CLAUDE.md` |
| **any UI string, key or string table** | `Assets/Addressables/CLAUDE.md` — read it first, every time |

### Everything else

| What | Where |
|---|---|
| Adding an assembly — system, `Tests` or `Editor` | skill `add-assembly` |
| Checking the Editor state and the project's health | skill `unity-verify` |
| Auditing this documentation for drift | skill `unity-md-improver` |
| Starting a `CLAUDE.md` for a folder | skill `new-claude-md` |
| A folder whose `CLAUDE.md` is empty | that is deliberate — a slot reserved for knowledge that does not exist yet |
