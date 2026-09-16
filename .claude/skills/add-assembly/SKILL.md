---
name: add-assembly
description: Add an assembly to the project — a new system under Assets/Code/Systems, or the Tests or Editor assembly of an existing one. Use when a Tests/ folder has no .asmdef, when tests compile into a runtime assembly and drag NUnit into the build, when editor code needs its own assembly, or when a new subsystem needs a place to live. Covers the .asmdef, the AssemblyBuilder asset, the layer collection entry and Metadata.cs, all of which fail quietly and separately when one is missing.
---

# Adding an assembly

Three cases, one procedure: a **new system**, a system's **`Tests`**, a system's **`Editor`**. They
differ only in the asmdef fields and in which parent the builder points at.

The procedure has four parts. **Each one fails quietly on its own**, which is why this is a skill and
not a paragraph: a missing builder still compiles, a missing collection entry still compiles, and a
missing `Metadata.cs` only shows up when someone writes the second test.

Background rules: `.claude/rules/assemblies.md`, `.claude/rules/tests.md`,
`.claude/rules/editor_assemblies.md`.

---

## 1. The `.asmdef`

Place it in the folder it owns: `Assets/Code/Systems/<Name>/`, `.../Tests/`, `.../Editor/`.

**Every case:** `name` and `rootNamespace` are both the assembly name, and they match. `references`
is left as it is — the builder writes it.

**A system:**

```json
{
    "name": "ItemSystem",
    "rootNamespace": "ItemSystem",
    "references": [],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": false,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

**A `Tests` assembly** — the three fields that make it a test assembly, all required together:

```json
    "overrideReferences": true,
    "precompiledReferences": [ "nunit.framework.dll" ],
    "defineConstraints": [ "UNITY_INCLUDE_TESTS" ],
```

`UNITY_INCLUDE_TESTS` is what keeps the assembly out of a player build; `overrideReferences` plus
`precompiledReferences` is what lets it see NUnit at all. Two of the three is not a working test
assembly — it is a broken build in one configuration only.

**An `Editor` assembly:** `"includePlatforms": [ "Editor" ]`.

Copy the shape from a file that already works — `Assets/Code/Shared/Tests/Shared.Tests.asmdef` or
`Assets/Code/Runtime/Editor/Runtime.Editor.asmdef` — rather than from memory.

## 2. The builder asset

**Through the menu, never by hand:** select the new `.asmdef`, then
`Assets/Create/Scripting/AssemblyBuilder/AssemblyBuilder from AssemblyDefinition` (`Shift+Ctrl+F11`).
This fills `_definitions` and lets Unity write the `.meta` — a hand-written `.meta` invents a GUID and
detaches the asset from everything.

Put it where its kind lives, and set `_publicParents`:

| Assembly | Builder goes in | Public parents |
|---|---|---|
| `<Name>` | `Architecture/` | `Shared`, plus any system it genuinely depends on |
| `<Name>.Tests` | `Architecture/Tests/` | `UnityEngine.TestRunner`, `UnityEditor.TestRunner`, `<Name>` |
| `<Name>.Editor` | `Architecture/Editor/` | `<Name>`, plus the editor assembly of the layer below |

`_inheritMode` stays `DeepInherit`. Use `_privateParents` only for a dependency this assembly must
not pass on to its own dependents.

## 3. The collection entry

Add the builder to its layer's collection: `_AllAssemblies`, `Tests/_AllTests` or
`Editor/_AllEditor`. Without it the assembly is skipped by `Build All` — it keeps compiling with
whatever references it already had, and quietly stops tracking the graph from then on.

Then run `Tools/AssemblyBuilder/Build All` and **read the console**. A cycle logs a `Debug.LogError`
and abandons that branch without failing the build, so the damage shows up as missing references on
assemblies that are not the ones you touched.

## 4. `Metadata.cs` — test assemblies only

Before the first test, not after. Two things change per copy: the `namespace`, and `Category.Self`,
which is that namespace minus the trailing `.Tests`.

```csharp
namespace ItemSystem.Tests
{
    internal static class Metadata
    {
        public static class Author
        {
            public const string Nickname = "nickname";
        }

        public static class Category
        {
            // Self = namespace of this file, minus the ".Tests" suffix
            public const string Self = "ItemSystem";

            public const string VeryEasy = "1_very_easy";
            public const string Easy = "2_easy";
            public const string Normal = "3_normal";
            public const string Hard = "4_hard";
            public const string Extreme = "5_extreme";
        }
    }
}
```

The `Author` block lists every developer in `Docs/authors.md`, one constant each — nick as the member
name, nick as the value. Keep the `Self` comment; it is the formula, and it is the field people get
wrong.

## 5. Finish

- A `CLAUDE.md` in the new folder. It may be **empty** — that is the normal state until the folder
  holds something you could not learn by reading its code. See the `new-claude-md` skill.
- A row in `Assets/Code/Systems/CLAUDE.md` for a new system.

## Traps

- **A `Tests/` folder without its own `.asmdef` compiles into the assembly above it**, dragging NUnit
  into a runtime assembly and breaking the player build. This is the failure the skill exists for.
- **An EditMode asmdef copied to make a PlayMode one silently loses `[UnityTest]`.** The two are not
  the same file with a different name.
- **A new `.cs` may never join its assembly** — no errors, and a test total that does not move.
  Rename the file to force the reimport.
- **An assembly without a builder is invisible to `Build All`** and looks completely healthy.
