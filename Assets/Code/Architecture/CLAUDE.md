# CLAUDE.md — `Assets/Code/Architecture`

Read `Assets/Code/CLAUDE.md` first — it carries the layer map. This file is folder-local.

**Nothing here is code, a namespace, or dependency injection.** These are `ScriptableObject` assets
from `com.vertoker.assemblybuilder`, one per assembly, forming a hierarchy parallel to the folder
tree. `Tools/AssemblyBuilder/Build All` walks that hierarchy and writes the `references` array of
every `.asmdef` it owns.

The full rules are `.claude/rules/assemblies.md`, which loads with any `.asmdef` or anything in this
folder. What follows is the shape of *this* graph.

## Layout

```
Architecture/
  _AllAssemblies.asset      collection: _AllTests, _AllEditor, Shared, Runtime
  Shared.asset              parents: —
  Runtime.asset             parents: Shared
  Editor/
    _AllEditor.asset        collection: Shared.Editor, Runtime.Editor
    Shared.Editor.asset     parents: Shared
    Runtime.Editor.asset    parents: Shared.Editor, Runtime
  Tests/
    _AllTests.asset         collection: Shared.Tests, Runtime.Tests
    Shared.Tests.asset      parents: UnityEngine.TestRunner, UnityEditor.TestRunner, Shared
    Runtime.Tests.asset     parents: Shared.Tests, Runtime
  External/                 read-only wrappers for third-party assemblies
```

`_AllAssemblies` is the root and nests the other two collections. Every builder uses `DeepInherit`,
and `_privateParents` is empty throughout — nothing here yet needs a dependency it does not pass on.

## `External/`

One read-only wrapper per third-party assembly: `_readonly: true`, both parent lists empty, and
`_definitions` pointing at the package's own `.asmdef` in `Library/PackageCache/`. Read-only means
the builder never writes that file — the wrapper exists only to make the assembly addressable as a
parent.

**This is how a third-party assembly is depended on.** Never reference one by GUID from an
`.asmdef`: a raw GUID survives until the next build and is then overwritten.

## The order of work

Changing a dependency is always: edit the builder's `_publicParents` → `Build All` → read the
console. Never the `.asmdef` directly; `references` there is generated output.

Adding an assembly is three things — the `.asmdef`, its builder, and the collection entry. Create the
builder through `Assets/Create/Scripting/AssemblyBuilder/AssemblyBuilder from AssemblyDefinition`
(`Shift+Ctrl+F11`; `Shift+Cmd+F11` on macOS) with the `.asmdef` selected, so that `_definitions` is
filled and Unity writes the `.meta`. The `add-assembly` skill carries the whole checklist.

## Constraints worth knowing before you debug something here

- **`Build` rewrites each `.asmdef` whole from its own model.** Twelve fields survive; anything the
  model does not know is dropped without a warning.
- **A cycle logs a `Debug.LogError` and abandons that branch**, without stopping the build. References
  then go missing on assemblies that are not in the cycle. Read the console before editing anything.
- **A builder whose `_definitions` GUID does not resolve is skipped silently** — no warning, no entry
  in the graph. If an assembly is mysteriously absent, check that its package is still installed.
