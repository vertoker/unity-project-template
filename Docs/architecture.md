# Architecture

How <project name> is laid out, why the layers run the way they do, and what happens when you need
something the layout does not give you.

## Three layers

```
Shared      engine-light utilities, depends on nothing
  ↑
Systems     subsystems, one folder and one assembly each
  ↑
Runtime     the game: composition, screens, scenes
```

Each layer depends only on the ones below it. `Shared` holds things that would still make sense in a
different game — collections, math, async helpers, logging. `Systems` holds the subsystems this game
is made of, each in its own folder with its own assembly, none of them aware of the others unless the
graph says so. `Runtime` is the game itself: composition, screens, scenes, the glue that only makes
sense here.

**If you need a reference that points backwards, that is a layout error, not a reason to add the
reference.** A `Shared` utility that needs to know about a system is not a utility; a system that
needs `Runtime` is describing composition and belongs one layer up. The builder will let you do it —
the graph is just data — which is exactly why this has to be a rule rather than a constraint.

## Namespaces

**Namespace = the folder path, minus the `Systems` segment.**

| Folder | Namespace |
|---|---|
| `Assets/Code/Shared/Async` | `Shared.Async` |
| `Assets/Code/Systems/ItemSystem/Models` | `ItemSystem.Models` |
| `Assets/Code/Runtime/UI` | `Runtime.UI` |

The assembly name matches the namespace root: `Shared`, `ItemSystem`, `ItemSystem.Tests`,
`ItemSystem.Editor`, `Runtime`.

The `Systems` segment drops out on its own, and this is worth understanding rather than memorising:
each `.asmdef` carries a `rootNamespace` equal to its assembly name, and the IDE builds a namespace
from `rootNamespace` plus the path *below that `.asmdef`*. `Systems/` sits **above** the system's own
`.asmdef`, so it never enters the path. Nothing has to be configured per-IDE for this to hold — see
`Docs/adr.md` for why it is done this way rather than through a namespace provider.

## The assembly graph

`Assets/Code/Architecture/` is **not code and not dependency injection.** It is a parallel hierarchy
of `ScriptableObject` builder assets — one per assembly — that generates the `references` array in
every `.asmdef`. Full rules: `.claude/rules/assemblies.md`, and
`Assets/Code/Architecture/CLAUDE.md`.

The template ships with:

| Builder | Public parents |
|---|---|
| `Shared` | — |
| `Runtime` | `Shared` |
| `Editor/Shared.Editor` | `Shared` |
| `Editor/Runtime.Editor` | `Shared.Editor`, `Runtime` |
| `Tests/Shared.Tests` | `UnityEngine.TestRunner`, `UnityEditor.TestRunner`, `Shared` |
| `Tests/Runtime.Tests` | `Shared.Tests`, `Runtime` |

Plus three collections — `_AllAssemblies` (which nests `_AllTests`, `_AllEditor`, `Shared` and
`Runtime`), `_AllEditor` and `_AllTests` — and a set of read-only wrappers in `Architecture/External/`
that make third-party assemblies addressable as parents.

Every builder uses `DeepInherit`, and `_privateParents` is empty throughout: nothing in the template
yet needs a dependency it does not pass on.

## How a system gets `Tests` and `Editor`

`Shared`, `Runtime` and **every system** may have their own `Tests` and `Editor` folders. Those take
an ordinary `.asmdef` and **repeat their parent's layer and inheritance**: `X.Tests` inherits `X`,
and `X.Editor` inherits `X` plus the editor assembly of the layer below.

The procedure is the `add-assembly` skill. Do not do it from memory — the three pieces (assembly,
builder, collection entry) fail quietly and separately when one is missing.

## Composition

Entry point is `Assets/Scenes/Boot.unity` — the only scene in the build settings — which builds the
VContainer scope the rest of the game resolves from. Features that need dependencies register in the
relevant scope; they do not resolve manually and they do not reach for a singleton.

## Systems in this project

| System | Owns | Tests | Editor |
|---|---|---|---|
<systems>

A system that is not listed here does not exist as far as the graph is concerned, even if its folder
does — the table and `Assets/Code/Architecture/` have to agree. An empty table is the expected state
of a fresh project; `Systems/` starts empty on purpose.
