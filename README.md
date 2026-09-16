# Unity project template

A Unity project starting point with the agent layer already built: documentation, rules that load with the
files they govern, skills, and a one-shot initialisation plan. Clone it, run the plan, start writing
the game.

> **This README is scaffolding.** `INIT_PLAN.md` replaces it in its last phase with the project's
> name and a short description — nothing else. Everything below describes the template, and stops
> being true the moment the template becomes a project.

## Getting started

1. Clone, open in Unity (at least **6000.6.0f1**), let packages resolve.
2. Open the project in Claude Code.
3. Say **"run INIT_PLAN.md"**.

**It asks which language you want to talk in first, on its own, and switches to it before anything
else** — then interviews you once: name, pitch, genre, platforms, stores, company, author details,
locales, and **how much autonomy the agent gets in the Unity Editor**. Only the name, the pitch, the
author and the autonomy mode are required; anything else can be answered with a dash. After that it
fills in every
placeholder, creates the three starter `.cs` files, renames the product, verifies the project
compiles, and deletes itself. It does not commit; that stays yours.

Namespaces and packages are not asked about: the first follows from the folder layout, and nothing
is removed from the manifest.

## What is in here

| | |
|---|---|
| `CLAUDE.md` | The navigator — eleven numbered rules, the stack, the architecture, and a map to everything else. Kept under 300 lines because it loads into every session. |
| `.claude/rules/` | Seven rules that load **only** with the files they govern — assets, UI Toolkit, tests, assemblies, editor code, dependencies — plus one always-on rule for the Unity Editor. |
| `.claude/skills/` | `add-assembly`, `unity-verify`, `unity-md-improver`, `new-claude-md`. |
| `.claude/hooks/` | Auto-approves tool calls in plan mode, where nothing can be written anyway. |
| `Docs/` | Architecture, code style, testing, troubleshooting, decision records, authors, `Plans/` for feature work and `Issues/` for things that were tried and measured. |
| `Assets/Code/` | Three layers — `Shared`, `Systems`, `Runtime` — with six assemblies wired through AssemblyBuilder assets rather than hand-edited `.asmdef` references. |
| `.mcp.json` | Points Claude Code at the Unity MCP server, so a fresh clone needs no per-machine setup. |
| `INIT_PLAN.md` | The initialisation plan. Deletes itself when done. |

Stack: URP, Entities, Addressables, Localization, Input System, VContainer, UniTask, UniRx,
NaughtyAttributes, ParrelSync, NuGetForUnity. **Nothing in the manifest is load-bearing at the
start** — a package being installed is inventory, not a decision.

## The workflow it assumes

- **The agent writes, the author commits.** Rule 2, and it has no exceptions.
- **Context is loaded on demand.** The root file stays small; rules attach themselves to the file
  types they describe; long explanations live in `Docs/` and leave a one-line pointer in the code.
- **Features are planned by a capable model and executed by a cheaper one**, with the plan in
  `Docs/Plans/` as the handoff. Ordinary bugs skip the plan and go straight to the cheaper model.
- **The Unity Editor is driven over MCP**, within whichever of four autonomy modes you pick at
  initialisation — from read-only, through the recommended middle ground and full Editor control
  with announcements, to no permission boundary at all.
- **Claims are backed by tool output.** "It compiles" is not a test run, and an empty console usually
  means nothing was built.
- **Assemblies change through the graph**, never by editing a generated `references` array.
- **An empty `CLAUDE.md` is a reserved slot**, not an unfinished file. Six of them ship this way.

Documentation and prose in the repository are English; the agent talks to you in whichever language
you pick during the interview.

## Requirements

Unity 6000.6.0f1 · Claude Code · Node.js (for the hook) · `uv`/`uvx` (for the Unity MCP server) ·
Git LFS, which `.gitattributes` expects for binary assets.

Works on Windows, macOS and Linux.
