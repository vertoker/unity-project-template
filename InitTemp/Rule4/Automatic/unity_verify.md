---
name: unity-verify
description: Check the Unity project's actual state through MCP — server reachable, project compiling, console clean, tests passing, and the game actually doing the thing in Play Mode — then report real numbers. Use after a change that touches compiled code or assets, before claiming anything works, or when the Editor is behaving strangely and you need to know what is actually true.
---

# Verifying the Unity project

This produces evidence, and under rule 4 in this project it is the **only** thing standing between a
wrong change and a wrong change nobody noticed. Nothing here needs permission; everything here needs
doing.

## 1. Is the server reachable

Query `mcpforunity://instances`, or request the URL in `.mcp.json`.

- **Answers** → go to step 2. **Start nothing.**
- **Does not answer** → before concluding it is down, compare the port: `.mcp.json` says `8080`; the
  Editor reads its own from `EditorPrefs`, shown in the MCP for Unity window. A mismatch there is the
  usual cause on a fresh clone.
- **Still nothing** → start one in a separate console window the author can close, using the command
  that window shows. Never a second instance, never a child of this session.
- **Cannot be started** → skip to the fallback at the bottom.

If two Editors are attached, select by full `Name@hash`.

## 2. Compile

Call `refresh_unity`, then **wait for the domain reload to actually finish** and confirm it did.
Anything run against a half-reloaded domain fails in ways that look unrelated to the real cause.

## 3. Read the console

Read it and report what is there.

**An empty console with a test total of `0` is not a clean project — it usually means nothing
compiled.** Check for `error CS` in `Library/Bee/tundra.log.json` before believing a silent console.

## 4. Run tests

Pick the mode by what the change could reach, not by how risky it feels — `Docs/testing.md`. Both
EditMode and PlayMode runs are yours. Report total, passed, failed and skipped, and name the mode.

If the total did not move after adding a test, the run used a stale assembly. Refresh and run again;
do not report the number.

Never edit a `.cs` while a PlayMode run is in flight — the recompile kills it.

## 5. Behaviour — see it happen

This is the step that separates "compiles" from "works", and it is not optional for a change to what
the game does.

1. **Note what is currently open** — which scene, and whether it has unsaved changes. You are putting
   this back.
2. Open the scene, enter Play Mode, and **watch the thing you changed actually happen.**
3. Take a screenshot if the result is visual. A screenshot is evidence; a description is a claim.
4. **Exit Play Mode. Reopen the scene that was open, and discard your own changes to it** unless
   changing it was the task.
5. Read the console again — Play Mode produces errors that an EditMode compile never surfaces.

**Leaving the Editor in Play Mode, or on a scene the author did not open, means this step did not
finish.** Restoring is part of the step, not cleanup after it.

## 6. Report

Numbers, not adjectives — and keep it short, because that is the deal in this mode: the author reads
a summary instead of watching each step. A summary that omits a failure is worse than no summary,
and a summary padded with small edits stops being read.

Say what you checked, what you saw in Play Mode, what you restored, and what remains unknown.

---

## Fallback: no server

A batch-mode compile answers the "does it build" half and nothing else.

**Never pass `-noUpm`.** Most dependencies here arrive over git-UPM; disabling the package manager
removes half the project, and the resulting errors describe missing types rather than the real cause.

Read `Library/Bee/tundra.log.json` for `error CS`. Say explicitly in the report that tests were not
run and behaviour was not observed — a compile is not a test run and neither is a substitute for
seeing it happen.
