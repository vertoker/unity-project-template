---
name: unity-verify
description: Check the Unity project's actual state through MCP — server reachable, project compiling, console clean, tests passing — and report real numbers. Use after a change that touches compiled code or assets, before claiming anything works, or when the Editor is behaving strangely and you need to know what is actually true.
---

# Verifying the Unity project

This produces evidence. Rule 9 forbids reporting a result a tool did not return, and this is the
procedure that gets you one.

What you may do here is bounded by **rule 4** (`.claude/rules/unity_editor.md`). This skill does not
widen it — the steps below stay inside what rule 4 already allows, and the one step that does not is
marked.

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

Pick the mode by what the change could reach, not by how risky it feels — `Docs/testing.md`. Report
total, passed, failed and skipped, and name the mode.

If the total did not move after adding a test, the run used a stale assembly. Refresh and run again;
do not report the number.

## 5. Behaviour — the author's step

A change to what the game *does* is not verified by compiling and reading tests. Entering Play Mode
and opening scenes need the author's permission under rule 4, so this step is handed over rather than
performed: **end with concrete manual steps** — which scene to open, what to click, what output
confirms it worked. If you want to check it yourself, ask; the ask is cheap and specific.

## 6. Report

Numbers, not adjectives. Name what you checked, what you did not, and what remains unknown. "It
compiles" and "it looks correct" are not verification and must not be presented as such.

---

## Fallback: no server

A batch-mode compile answers the "does it build" half and nothing else.

**Never pass `-noUpm`.** Most dependencies here arrive over git-UPM; disabling the package manager
removes half the project, and the resulting errors describe missing types rather than the real cause.

Read `Library/Bee/tundra.log.json` for `error CS`. Say explicitly in the report that tests were not
run and why — a compile is not a test run, and the difference is exactly what rule 9 is about.
