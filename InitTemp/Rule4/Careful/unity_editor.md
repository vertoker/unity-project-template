# Rule 4 — The Unity Editor: read and test freely, mutate only with permission

A `unity-mcp` server exposes the running Editor to this session. This file is always loaded, because
the boundary it draws applies to every task, not to particular files.

## Free, no permission needed

Reading Editor and project state, reading the console, reflecting over types, querying scenes and
assets without opening them, `refresh_unity`, and running EditMode tests.

Small serialized-asset edits are rule 1's allowance, not this rule's — the list of what counts as
small, and the requirement to report each one, is `.claude/rules/unity_assets.md`. Going through MCP
does not widen it: the allowance covers *what* may change, not *how*.

## The author's to grant

Entering Play Mode. Opening or saving a scene. Mutating a scene or prefab hierarchy. Invoking menu
items. Deleting assets. Bulk edits across many assets. Package changes and builds. Creating an
`.asmdef` or a builder asset. Running a batch tool that rewrites assets. Executing arbitrary code in
the Editor. Anything whose effect is not obvious from a one-line description.

Ask once, specifically, and proceed with everything else in the task meanwhile. A blocked step is not
a blocked task.

## Never leave the Editor locked

Not in Play Mode, not mid-import, not on a scene the author did not open, not with a modal dialog
waiting. If something you started left the Editor in an unusual state, restore it before reporting,
and say what you restored.

## Working with the server

**1. Check whether it is already running before starting anything.** Query `mcpforunity://instances`,
or make a request to the URL in `.mcp.json`. **If it answers, start nothing.**

**2. Only if it does not answer, start one** — in a **separate console window the author can close**,
never as a child of this session:

```
uvx --from <source> mcp-for-unity --transport http --http-url http://127.0.0.1:8080 --project-scoped-tools
```

The exact command, including the path to `uvx`, is shown by the MCP for Unity window in the Editor;
it is assembled by `ServerCommandBuilder.TryBuildCommand` in `com.coplaydev.unity-mcp`. Take it from
the window rather than reconstructing it.

**3. Never start a second one.** Two servers on one endpoint produce
`Multiple Unity instances are connected`, and from then on it is unclear which Editor a command
reached.

The port is the one place this commonly goes wrong on a fresh clone: `.mcp.json` names `8080`, the
package default, while the Editor reads its own from `EditorPrefs`, which is per-machine and not in
the repository. Compare the two before concluding the server is down — `Docs/troubleshooting.md`.

## Traps

- **Two Editors on one server.** Select the target by its full `Name@hash`; a short name is rejected.
- **`refresh_unity` returns before the domain reload finishes.** A test run started immediately after
  it dies in a way that looks unrelated. Wait, confirm, then run.
- **A stuck call is cleared, not worked around.** Starting a second server to escape one turns a
  stuck call into two Editors on one endpoint.
- **An empty console with a test total of `0` means nothing compiled**, not that everything is clean.
  Look for `error CS` in `Library/Bee/tundra.log.json`.
- **A batch compile must never be passed `-noUpm`.** Most of this project's dependencies arrive over
  git-UPM, so disabling the package manager removes half the project and the errors that follow
  describe missing types rather than the actual cause.

## Reporting

Rule 9 applies with full force here, because everything in this file produces a result that is easy
to assume and cheap to check. Report the numbers the tool returned. If a step was skipped, or the
server was unreachable and you fell back to a batch compile, say so and say what that leaves unknown.
