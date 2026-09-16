# CLAUDE.md — `Assets/Code/Shared`

Read `Assets/Code/CLAUDE.md` first — it carries the layer map and the rule for deciding which layer
new code belongs in. This file is folder-local.

## Mental model

`Shared` is the bottom of the graph: it references nothing above it, and nothing in it may name a
game concept. The test for a file arriving here is not "could this be reused" but "does it reference
anything that only exists in this game" — if the answer is no, the file has no reason to sit higher.

The flip side is that **anything placed here is load-bearing for every layer above**. A signature
change in `Shared` recompiles the whole project, so an API here is worth a minute of thought that the
same API in `Runtime` would not deserve.

## Folder index

| Folder | Owns |
|---|---|
| `Async/` | the cancel-and-restart (debounce) wrapper over a UniTask action |
| `States/` | the type-keyed finite state machine |
| `Utils/` | stateless helpers, one concern per file |

The root holds what nothing else depends on and nothing groups with: the logger, `SerializableGuid`
and the IL2CPP attribute. `Editor/` and `Tests/` are the layer's own editor and test assemblies —
same rules, one level down.

## Conventions

- **Never `Debug.Log` — use `Shared.GameLogger`.** It is the only place that can tell Editor
  formatting from device formatting, and the root `CLAUDE.md` names it as a project-wide convention.
  That includes code inside `Shared` itself and inside `Shared.Editor`. The source name comes from
  `[CallerMemberName]`, from `Log<T>(…)` when the caller is a static helper, or from
  `memberNameManual` — which is how a base class reports under `GetType().Name` instead of its own.
- Namespace is the folder path: `Shared/Utils` → `Shared.Utils`, and a file in the root is in
  `Shared`.
- The `*Utils` / `*Math` / `*Rules` / `*Extensions` suffix taxonomy from `Docs/code_style.md` is what
  decides a new static class's name, and therefore which folder it lands in.

## Traps

- **The shared `StringBuilder` makes `GameLogger` main-thread-only.** Two threads logging at once
  interleave into one buffer and produce a single garbled line. There is no guard against it; the
  symptom is a message that looks like two messages spliced together.
- **A source color is stable within one run and different in the next.** It comes from
  `string.GetHashCode`, which .NET randomizes per process. Never treat the color as an identifier,
  and do not compare console screenshots across sessions.
- **Outside the Editor the prefix loses its rich text and every line break collapses to a space.**
  Only the Editor console renders `<color=…>`, and Android LogCat cuts a message at the first line
  break — so a multi-line message would otherwise lose everything after line one.
- **`SerializableGuid` and `Shared.Editor.SerializableGuidDrawer` are a pair.** The drawer finds the
  backing field by the literal name `m_SerializedGuid`; renaming the field silently breaks the
  inspector rather than the build. That is why the field keeps its `m_` name against the project's
  own style, and why the drawer holds the name in one `const`.
- **`Il2CppSetOptionAttribute.cs` declares a type in `Unity.IL2CPP.CompilerServices`,** not in
  `Shared`. That is required, not an oversight — il2cpp matches the attribute by full metadata name
  and Unity's own copy is `internal`. The file's header comment carries the rest.
