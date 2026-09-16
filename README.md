# Unity project template

A Unity project starting point with the agent layer already built: documentation, rules that load with
the files they govern, skills and a one-shot initialisation plan. Clone it, run the plan, start writing
the game

> **This README is scaffolding.** `INIT_PLAN.md` replaces it in its last phase with the project name and
> a 2-3 sentence description, nothing else. Everything below describes the template and stops being true
> the moment the template becomes a project

## Getting started

1. Clone, open in Unity `6000.6.0f1` or newer, let packages resolve
2. Open the project in Claude Code
3. Say "run INIT_PLAN.md"

The plan asks which language you want to talk in first, on its own, and switches to it before anything
else. Then it interviews you once: name, pitch, genre, platforms, stores, company, author details,
locales, how much autonomy the agent gets in the Unity Editor and how much of the starter code you
want to keep. Only the name, the pitch, the author, the autonomy mode and the starter-code answer are
required, anything else can be answered with a dash

After the interview it throws away the starter code you did not keep, fills in 13 placeholders,
creates 3 more `.cs` files, renames the product, verifies that the project compiles and deletes
itself. It does not commit, that part stays yours

Namespaces and packages are not asked about: the first follows from the folder layout, and nothing is
removed from the manifest

## What is worth knowing

**It is built for agentic development, and specifically for Claude.** Rule loading by `paths`,
`SKILL.md`, `PreToolUse` hooks and `.mcp.json` are Claude Code mechanisms. Another agent can read the
markdown here, but nothing will attach itself to the right files on its own

**It is assembled from practice, not designed up front.** Every rule earned its place on one of my 3
working projects, and most of them earned it by something going wrong first. I can't speak for how well
this transfers to a giant teams more than 3-4 person, I have never run it that way

**New Unity where performance is the point, boring tech where it is not.** DOTS (`Entities`, `Burst`,
`Collections`, `Mathematics`) and UI Toolkit carry the parts that have to be fast. The managed side is
`VContainer`, `UniTask` and `UniRx`, picked because they have shipped in production and behave
predictably, not because they are new. Nothing experimental sits on the critical path

**This is my stack, not a community one.** If you want a different one - **fork it**, that is the intended
use and it costs you nothing. Pull requests here I accept only for fixes that are useful to me,
anything that widens the template toward somebody else's taste gets declined

**Effectively MIT, with no LICENSE file.** A template gets copied whole, and a license copied along with
it becomes a claim inside the new project that nobody meant to make. So the file is left out on purpose:
treat the contents as MIT, do whatever you want with them, attribution not needed

**`Docs/Plans/` holds every plan, and it is also a money decision.** A capable model writes the plan, a
cheaper one executes it, and the plan is the handoff between the two. Plans stay after execution with
their status in the filename, because a finished plan answers why the code is shaped this way and a diff
can't. Ordinary bug fixes skip the plan and go straight to the cheaper model

**`Assets/Code/Shared` ships a small toolkit, and you are asked whether to keep it.** A logger, a
serializable `Guid` with its drawer, an IL2CPP check attribute, a debounce runner, a state machine and
a few `*Utils`. Nothing above it references any of it, so all 3 answers - keep it, keep only the
`Shared` root, throw it all out - leave a project that compiles. The 3 assemblies stay either way

**An empty `CLAUDE.md` is normal.** 5 of them ship that way. It is a reserved slot for a folder that
holds no knowledge beyond its own code yet, not an unfinished file and not something to fill in to make
the repository look tidy

**Every `.asmdef` is generated from the [AssemblyBuilder](https://github.com/vertoker/assemblybuilder)
graph, and it works.** The `references` arrays are never hand-edited. The graph is a parallel tree of
assets under `Assets/Code/Architecture/`, and `Tools/AssemblyBuilder/Build All` rewrites all 6
assemblies from it. Adding an assembly means adding a builder, not editing a list of GUIDs

**The agent writes, the author commits.** Rule 2, no exceptions, not even for the initialisation plan
itself

**A claim has to be backed by tool output.** "It compiles" means a compile that returned, and an empty
Unity console with a test total of `0` usually means nothing was built at all

**Context is loaded on demand.** The root file is kept under 300 lines, rules attach themselves to the
file types they describe, long explanations live in `Docs/` and leave a one-line pointer in the code

**The Editor is driven over MCP**, inside one of the 4 autonomy modes you pick at initialisation, from
read-only up to no permission boundary at all

Everything written into the repository is English, the agent talks to you in whichever language you
picked during the interview

## Architecture

| | |
|---|---|
| `CLAUDE.md` | The navigator: 11 numbered rules, the stack, the architecture and a map to everything else. Kept under 300 lines because it loads into every session |
| `.claude/rules/` | 7 rules that load **only** with the files they govern - assets, UI Toolkit, tests, assemblies, editor code, dependencies - plus one always-on rule for the Unity Editor |
| `.claude/skills/` | `add-assembly`, `unity-verify`, `unity-md-improver`, `new-claude-md` |
| `.claude/hooks/` | Auto-approves tool calls in plan mode, where nothing can be written anyway |
| `Docs/` | Architecture, code style, testing, troubleshooting, decision records, authors, `Plans/` for feature work and `Issues/` for things that were tried and measured |
| `Assets/Code/` | 3 layers: `Shared`, `Systems`, `Runtime`, with 6 assemblies wired through AssemblyBuilder assets instead of hand-edited `.asmdef` references |
| `.mcp.json` | Points Claude Code at the Unity MCP server, so a fresh clone needs no per-machine setup |
| `INIT_PLAN.md` | The initialisation plan. Deletes itself when done |

A namespace is the folder path minus the `Systems` segment, and every `.asmdef` carries the matching
`rootNamespace`, so the layout enforces the rule instead of relying on anyone remembering it

Stack: URP, Entities, Addressables, Localization, Input System, VContainer, UniTask, UniRx,
NaughtyAttributes, ParrelSync, NuGetForUnity. **Nothing in the manifest is load-bearing at the start**,
a package being installed is inventory and not a decision

## Requirements

Unity `6000.6.0f1` or newer · Claude Code · Node.js for the hook ·
`uv` / `uvx` for the Unity MCP server · Git and Git LFS

Works on Windows, macOS and Linux
