---
paths:
  - "**/Editor/**/*.cs"
  - "**/*.Editor.asmdef"
---

# Editor tooling: where it lives, how it is named, who runs it

## Placement

An editor assembly lives **next to the system it serves** — `Assets/Code/Systems/ItemSystem/Editor/`,
not in a project-wide editor root. It is named `<Parent>.Editor`, carries
`includePlatforms: ["Editor"]`, and its builder in `Architecture/Editor/` inherits the runtime parent
plus the editor assembly of the layer below. `Runtime.Editor` inherits `Runtime` and `Shared.Editor`;
a system's editor assembly follows the same shape.

A project-wide editor root collects tools that belong to different systems into one assembly, and
then every one of them recompiles when any one changes. Keep them apart.

## When an editor-only assembly is impossible

An `Editor/` folder does **not** always mean an editor-only assembly. Three cases force the code back
into the runtime assembly behind `#if UNITY_EDITOR`:

- **A `MonoBehaviour` in the folder.** Its type does not exist in a build, so Unity refuses the
  `AddComponent` and the component is lost from any scene that used it.
- **A dependency cycle** — the editor code needs a runtime type that in turn needs it.
- **Gizmos, `OnValidate`, and editor-only members of a `ScriptableObject`**, which have to compile as
  part of the type they belong to.

Decide which of the two shapes applies *before* writing the file; converting later means moving types
between assemblies, which moves their GUIDs.

**Inside an assembly that is already editor-only, `#if UNITY_EDITOR` is not written.** The platform
constraint has already done that job, and the directive only suggests to the next reader that the
file might compile elsewhere. Use it exclusively in the runtime-assembly case above.

## Menus

- Path shape: `Tools/<System>/<Action>`.
- **Paths come from constants in `Shared/Constants/EditorNames`, never string literals in the
  attribute.** A literal is a second source of truth for a name that appears in several places, and
  renaming a menu then half-renames it. The first editor tool in a project is what creates that file.
- Priorities only where the order actually matters. A priority on every item is noise that has to be
  maintained.

## Toolbars and overlays

Extend the Scene view with `[Overlay(typeof(SceneView), …)]`. Do not reflect into
`UnityEditor.Toolbar` — it is internal, and it breaks on the first Unity update that touches it.

## A batch tool is launched by a human

A tool that rewrites many assets is run by the developer, from the menu, deliberately. Never call one
from code, never attach it to an import hook, a `didReloadScripts` callback or any other automatic
trigger. This holds regardless of what rule 4 otherwise permits — the constraint is about a batch
rewrite being reviewable, not about who is allowed to touch the Editor.
