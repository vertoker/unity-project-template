# CLAUDE.md — `Assets/Code`

Read the root `CLAUDE.md` first — it carries the rules, the stack and the map. This file is the layer
map: where new code goes, and why the answer is never "wherever it fits".

## The three layers

| Folder | Holds | May depend on |
|---|---|---|
| `Shared/` | engine-light utilities that would survive being moved to another game | nothing |
| `Systems/` | one folder and one assembly per subsystem | `Shared`, and other systems only where the graph says so |
| `Runtime/` | the game: composition, screens, scenes | `Shared`, `Systems` |

## Where does this code go

Ask what the code would need in order to make sense somewhere else:

- **Nothing?** `Shared/`. Collections, math, async helpers, logging, extension methods.
- **A domain, but not this game's composition?** `Systems/<Name>/`. Inventory, dialogue, save data —
  a thing with its own vocabulary that could plausibly be lifted into another project of the same
  genre.
- **Only this game?** `Runtime/`. Screens, scene wiring, the scope, anything that names a specific
  level or a specific menu.

When two answers seem right, pick the lower layer only if you can name what makes it general. "It
might be reusable" is not that; "it has no reference to anything above it" is.

**A reference that points backwards is a layout error**, not a reason to add the reference — see
`Docs/architecture.md`. The builder will happily generate it, which is precisely why the constraint
has to be a decision rather than a compile error.

## Namespaces

Folder path, minus the `Systems` segment. `Shared/Async` → `Shared.Async`;
`Systems/ItemSystem/Models` → `ItemSystem.Models`; `Runtime/UI` → `Runtime.UI`.

You do not have to maintain this by hand: each `.asmdef` carries a `rootNamespace` equal to its
assembly name, and the path is composed from below that file. Setting `rootNamespace` on a new
assembly is part of `add-assembly`, and `Docs/adr.md` records why it lives there.

## `Architecture/` is not code

`Assets/Code/Architecture/` holds the AssemblyBuilder graph — it generates the `references` array of
every `.asmdef` in <project name>. It is not a layer, not a namespace and not dependency injection.
See `Assets/Code/Architecture/CLAUDE.md`.

## Tests and editor code

`Shared`, `Runtime` and every system may carry their own `Tests/` and `Editor/` folders, each with its
own assembly that repeats its parent's place in the graph. Use the `add-assembly` skill — the three
pieces it creates fail quietly and separately when one is missing.

## Empty `CLAUDE.md` files here

Several folders below this one have a `CLAUDE.md` of zero bytes. That is deliberate: the file is a
reserved slot, and it stays empty until that folder holds knowledge you could not get by reading the
code in it. Writing a summary of an empty folder would be worse than the empty file — see the
`new-claude-md` skill for the threshold.
