# CLAUDE.md — `Assets/Code/Shared/Utils`

Read `Assets/Code/Shared/CLAUDE.md` first — it carries the mental model, the folder index and the
layer-wide conventions. This file is folder-local.

## Conventions

- Static classes, one concern per file, named by the suffix taxonomy in `Docs/code_style.md`. A file
  here that would be named `*Math`, `*Rules` or `*Extensions` still belongs here; the folder is the
  layer's helper drawer, not a claim that everything in it is a `*Utils`.
- `DebugUtils.IsDebug()` is the single answer to "are checks compiled into this build". Anything that
  wants to skip work in release asks it rather than testing defines again — `ObjectPoolFactory` is
  the example to copy.

## Traps

- **`UnityFastNoise` is third-party code kept close to upstream.** Its naming, its `FN_` constants
  and its formatting are Jordan Peck's, and they stay that way so the port can be re-synced. Do not
  restyle it, and do not let a project-wide formatter near it.
- **`FactoryGo`'s name-less overloads pass `null`,** which would give Unity an empty object name
  rather than the default `GameObject`. All four families funnel through one private constructor
  helper for that reason — a new overload must go through it too.
