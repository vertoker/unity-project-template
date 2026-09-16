---
name: unity-verify
description: Establish what is actually true about the Unity project — what can be read directly, and what has to be requested before it can be checked. Use after a change that touches compiled code or assets, before claiming anything works, or when the Editor is behaving strangely. Under this project's rule 4 most verification steps need permission first, so this skill is as much about producing a precise request as about producing evidence.
---

# Verifying the Unity project

This produces evidence. Rule 9 forbids reporting a result a tool did not return — and under **rule 4**
(`.claude/rules/unity_editor.md`) most of the steps that produce one need permission granted in
advance, for that specific action.

So this procedure has two halves: what you can establish now, and what to ask for. **Do the first
half fully before asking**, so the request is specific and arrives once.

## Free — do this first

**1. Is the server reachable.** Query `mcpforunity://instances`, or request the URL in `.mcp.json`.
If it does not answer, compare the port before concluding it is down: `.mcp.json` says `8080`, the
Editor reads its own from `EditorPrefs`, shown in the MCP for Unity window. A mismatch there is the
usual cause on a fresh clone. Starting a server is itself a write — it goes in the request below.

**2. Read the console as it stands.** Report what is there.

**An empty console with a test total of `0` is not a clean project — it usually means nothing
compiled.** `Library/Bee/tundra.log.json` holds `error CS` entries and can be read from disk without
touching the Editor at all. Read it before assuming silence means health.

**3. Read state.** Project and Editor state, type information, scenes and assets queried as data.
Anything that answers "what is the current shape of this" without changing it.

## Needs permission — ask once, precisely

Name each action and what it touches:

- `refresh_unity` — recompiles and reloads the domain,
- a test run — say which mode and why (`Docs/testing.md`), so the cost is visible,
- opening a scene, entering Play Mode, invoking a menu item, executing code.

Ask for what the task actually requires, not for a blanket grant. **Then carry on with everything
else meanwhile** — reading, and any change that can live in `.cs` or `.md`. A blocked step is not a
blocked task.

## After a grant

Run exactly what was granted, nothing adjacent. Report it individually: what ran, what it returned,
what changed. A grant for one action does not extend to the next one, and it does not extend to
repeating this one later.

## Report

Numbers, not adjectives — and **an explicit list of what was not checked and why**. Under this rule
most things stay unverified by default, so a report that only lists what passed is misleading even
when every line in it is true. "Compiles" was not established unless something compiled; "tests pass"
was not established unless something ran.

End with concrete manual steps for the author: which scene to open, what to click, what output
confirms it worked.

---

## Fallback: no server, or no grant

A batch-mode compile answers "does it build" and nothing else — and it is itself a write, so it needs
permission too.

**Never pass `-noUpm`.** Most dependencies here arrive over git-UPM; disabling the package manager
removes half the project, and the resulting errors describe missing types rather than the real cause.

Reading `Library/Bee/tundra.log.json` from disk needs nothing and is the one compile-adjacent check
that is always available. Use it, and say that is what you used.
