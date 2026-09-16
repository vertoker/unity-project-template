---
paths:
  - "**/*.asset"
  - "**/*.prefab"
  - "**/*.unity"
  - "**/*.meta"
  - "**/*.mat"
---

# Editing Unity serialized assets

Rule 1 says code and text come first — put a change in `.cs` or `.md` whenever it can live there.
This file governs the case where it cannot.

Whether you may reach the Editor at all, and through which tools, is rule 4
(`.claude/rules/unity_editor.md`). This file is about the file on disk.

## What a small edit is

The list is exhaustive, and it is a list on purpose — "small" is otherwise a judgment that drifts:

- wiring up a reference,
- repairing a broken or dangling one,
- correcting a number,
- setting one field, or one array element.

**Report every such edit explicitly** — name the file and what changed, every time, no exceptions.
The report is what makes the allowance safe; an unreported small edit is not a small edit.

Anything else — creating or deleting an asset, restructuring a scene or prefab hierarchy, a bulk
edit across many assets, or any change whose effect is not obvious from a one-line description — is
the author's to grant.

## Never hand-write a `.meta`

Unity creates them. A hand-written `.meta` invents a GUID, and a GUID nothing else knows about
detaches the asset from every reference to it. The failure is silent and it survives into the
repository.

## Asset GUID is not script GUID

A reference inside a YAML asset points at the **asset** GUID from that asset's own `.asset.meta`.
The GUID in a `.cs.meta` identifies the *script* and belongs only in `m_Script`. The two are the same
shape, sit next to each other in the same file, and swapping them breaks nothing loudly — the field
simply reads back null at runtime.

## A write behind a live Editor is undone

Any write to a `.asset` while the Editor is running is reverted by its next `SaveAssets` — that
includes `git checkout` as much as an editor tool. Either go through the Editor, or make the change
while it is closed.

## Measure the asset, not the comment

Before claiming what a settings or preset asset contains, read its YAML. Documentation drifts, and a
comment describing an asset is a claim about the past. Check an enum's real serialized values rather
than assuming the ordinals match the declaration order.

## YAML merge conflicts are the author's

`.gitattributes` routes scenes, prefabs and assets through `unityyamlmerge`. When it still conflicts,
hand the conflict back — a YAML merge resolved by guessing produces a file that opens fine and is
subtly wrong, which is worse than one that fails to open.
