---
paths:
  - "**/*.asmdef"
  - "Assets/Code/Architecture/**"
---

# Assemblies only through AssemblyBuilder

`Assets/Code/Architecture/` holds a parallel hierarchy of builder assets — one per assembly, plus one
collection per layer. `com.vertoker.assemblybuilder` walks that graph and writes the `references`
array of every `.asmdef` it owns. The graph is the source of truth; the `.asmdef` is its output.

## What is generated and what is yours

| Field | Who owns it |
|---|---|
| `references` | **generated** — edit the builder, never the file |
| `includePlatforms`, `excludePlatforms`, `defineConstraints`, `allowUnsafeCode`, `versionDefines`, `autoReferenced`, `noEngineReferences`, `overrideReferences`, `precompiledReferences`, `rootNamespace`, `name` | yours, edited in the `.asmdef` |

A hand-edited `references` array survives exactly until the next `Tools/AssemblyBuilder/Build All` —
**treat it as already lost**. The channel for changing a dependency is the builder's `_publicParents`
or `_privateParents`, followed by a build.

**`Build` does not patch the file — it rewrites it whole from its own model.** The model knows the
twelve fields above; anything it does not know disappears without a warning. If a future Unity adds
an `.asmdef` field, that field will not survive a build, and the loss will be silent.

## Adding an assembly

Three things, and all three are required:

1. the `.asmdef`,
2. its builder asset in `Assets/Code/Architecture/`, with `_publicParents` pointing at the parents,
3. an entry in that layer's collection — `_AllAssemblies`, `_AllTests` or `_AllEditor`.

Skip the builder and the assembly drops out of `Build All` — it keeps compiling, so nothing looks
broken, while its references quietly stop tracking the graph. Skip the collection entry and the same
thing happens one level up.

**Create the builder through the menu, not by hand:**
`Assets/Create/Scripting/AssemblyBuilder/AssemblyBuilder from AssemblyDefinition`
(`Shift+Ctrl+F11`; `Shift+Cmd+F11` on macOS) with the `.asmdef` selected.
It fills `_definitions` for you and lets Unity write the `.meta` — which
is what keeps this compatible with the "never hand-write a `.meta`" rule. Then set the parents.

The full checklist, including the test and editor variants, is the `add-assembly` skill.

## Parents, private parents, and outsiders

- `_publicParents` — a dependency that flows on to whoever depends on this assembly.
- `_privateParents` — a dependency this assembly needs for itself and does not pass along.
- `_inheritMode` is `DeepInherit` throughout; the builder expands public parents recursively.
- **A third-party assembly is wrapped**, not referenced by GUID: a builder in
  `Architecture/External/` with `_readonly: true` and `_definitions` pointing at the package's own
  `.asmdef`. Read-only means the builder never writes that file — it only makes the assembly
  addressable as a parent.

## Menus

`Tools/AssemblyBuilder/Build All` regenerates everything; `Build Selected` does one subtree.

## Traps

- **A cycle fails with a `Debug.LogError` and abandons that branch.** The build does not stop, so the
  visible symptom is references that went missing on assemblies far from the cycle. If references
  vanished after a build, read the console *before* touching any `.asmdef`.
- **A builder whose `_definitions` points at a package that is no longer installed** contributes
  nothing and says nothing. The GUID does not resolve, the builder is skipped silently, and the
  assembly it was supposed to expose is simply absent from the graph.
