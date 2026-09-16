# CLAUDE.md — `Assets/Code/Systems`

Read `Assets/Code/CLAUDE.md` first — it carries the layer map and the rule for deciding which layer
code belongs to. This file is folder-local.

One subsystem per folder, one assembly per subsystem. A system owns a domain and its vocabulary; it
does not own the game's composition, which is `Runtime/`.

## The systems

| System | Owns | Tests | Editor |
|---|---|---|---|
<systems>

An empty table is the expected state of a fresh project. A system that is not listed here does not
exist as far as the graph is concerned, even if its folder does.

## Adding one

Use the **`add-assembly`** skill. It exists because the operation has three parts that fail
separately and quietly:

1. the `.asmdef` — with `rootNamespace` set to the system's name,
2. its builder asset in `Assets/Code/Architecture/`, created through
   `Assets/Create/Scripting/AssemblyBuilder/AssemblyBuilder from AssemblyDefinition`,
3. an entry in the layer collection.

Miss the second and the system still compiles, while its references quietly stop tracking the graph.
Miss the third and the same happens one level up. Neither produces an error.

Then add a row to the table above, and one to the map in the root `CLAUDE.md` if the system is
something a session would need pointing at.

## Naming and namespaces

The folder name **is** the assembly name and the namespace root — `Systems/ItemSystem/` gives
assembly `ItemSystem` and namespace `ItemSystem.*`. The `Systems` segment does not appear in the
namespace because it sits above the system's own `.asmdef`.

Name the system after its domain, not after its mechanism: `Inventory`, not `ItemListManager`.

## Systems referring to each other

Allowed, but it is a decision, not a convenience — it goes through the builder's `_publicParents`
like any other dependency, and it makes the two systems one unit for every purpose except the folder
layout. Before adding one, check whether the thing both need actually belongs in `Shared/`, or
whether the coordination belongs in `Runtime/`.

A cycle between two systems fails with a `Debug.LogError` during `Build All` and abandons that branch
mid-build, so the visible damage is missing references elsewhere. Read the console first.
