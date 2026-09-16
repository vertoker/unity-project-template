---
name: new-claude-md
description: Start or fill in a per-folder CLAUDE.md. Use when a folder has accumulated knowledge that is not recoverable from its code, when a new assembly or system needs its documentation slot, or when deciding whether a folder deserves a CLAUDE.md at all. Also covers when to leave one empty, which is a legitimate state in this project.
---

# Starting a folder's `CLAUDE.md`

## The threshold

A folder gets a `CLAUDE.md` with content when **either** holds:

1. **It contains knowledge you could not get by reading its code.** A constraint imposed from
   outside, a contract with another folder, an ordering that looks arbitrary, a trap someone already
   fell into.
2. **It is a declared slot** — a folder that exists in the layout and will hold code later. Then the
   file is created **empty**, as a reservation.

An empty `CLAUDE.md` is a normal, deliberate state in this project, not an unfinished one. Six of
them ship in the template. Do not fill one in just because it is empty, and do not delete one just
because it is empty — deleting it removes the slot, and the next person re-derives the layout.

**What disqualifies content:** anything a reader would learn by opening the folder. A list of the
files in it, a restatement of what the classes are named, a summary of what the code does. That is
not neutral filler — it is negative value, because it goes stale while reading as authoritative.

## The header

Every file below the root opens with exactly this, pointing at its nearest parent that has content:

```markdown
# CLAUDE.md — `Assets/Code/Shared/Async`

Read `Assets/Code/Shared/CLAUDE.md` first — it carries the mental model, the folder index and the
layer-wide conventions. This file is folder-local.
```

The path in the title is the real path, backticked. The parent named is the nearest ancestor with
content, which may be the root `CLAUDE.md`.

## Sections

Use the ones that have something in them. An empty section is worse than a missing one.

| Section | Holds |
|---|---|
| **Folder index** | one line per subfolder — what it owns, not what is in it |
| **Mental model** | the shape you have to hold in your head to read this code at all |
| **Conventions** | what is done consistently here and why, where it differs from the layer |
| **Traps** | what has already gone wrong here, and what the symptom looked like |
| **Files that punch above their size** | the small file everything depends on |

**Traps is usually the most valuable section and the one most often left out.** If you are writing a
file after debugging something, that is the section to write first.

## Register it

A file nobody finds does nothing. When the new file is one a session would need pointing at:

- add a row to **The map** in the root `CLAUDE.md`, in the table that matches — question, layer, or
  everything else;
- if it documents a system, add it to the table in `Assets/Code/Systems/CLAUDE.md`.

## Size

Keep it under roughly 60 lines. A folder-local file that grows past that is usually holding a
document — a decision belongs in `Docs/adr.md`, something measured and rejected belongs in
`Docs/Issues/`, and the folder file keeps one line pointing there.

## Check before you finish

- Does every sentence survive the question *"could the reader have got this from the code?"*
- Does the header name a parent that actually exists and actually has content?
- Is anything here also stated somewhere else? Two copies of a fact will disagree eventually — keep
  the one closer to what it describes and link from the other.
