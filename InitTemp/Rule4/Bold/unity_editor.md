# Rule 4 — The Unity Editor is yours: verify, then report what you saw

A `unity-mcp` server exposes the running Editor to this session. This file is always loaded, because
the boundary it draws applies to every task, not to particular files.

## Yours without asking

The whole surface: reading state and console, reflecting over types, querying scenes and assets,
`refresh_unity`, running EditMode and PlayMode tests, opening and saving scenes, entering and leaving
Play Mode, taking screenshots, executing code in the Editor.

Small serialized-asset edits still follow rule 1, including the requirement to report each one —
`.claude/rules/unity_assets.md`. This rule widens *what you may reach*, not what counts as a small
edit.

## Announce, don't ask

One line before the action, so the author knows what just took over their Editor. A notice, not a
question — do not wait for an answer:

```
Entering Play Mode on Scenes/Boot.unity to verify the boot sequence
Saving Scenes/Level01.unity
```

## Restore what you took

Exit Play Mode. Reopen whatever scene was open, and drop your own changes to it unless changing it
*was* the task. **A session that leaves the Editor in Play Mode, or on a scene the author did not
open, has not finished** — this is not a courtesy, it is the condition that makes the permission
above safe to grant.

## Verification is not optional

A change to what the game *does* is not done until you have seen it do it. "It compiles" and "it
looks correct" are not verification, and presenting them as such is a rule 9 violation. The
`unity-verify` skill is the procedure.

## Still the author's to grant

Deleting assets. Bulk edits across many assets. Package changes. Builds. Creating an `.asmdef` or a
builder asset. Running a batch tool that rewrites many assets. Anything whose effect is not obvious
from a one-line description.

The line is not about risk to the Editor — it is about whether a one-line announcement actually tells
the author what happened. When it does not, it has to be a question instead.

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
- **Never edit a `.cs` during a PlayMode run** — the recompile kills the run and the wreckage looks
  like unrelated failures.
- **A batch compile must never be passed `-noUpm`.** Most of this project's dependencies arrive over
  git-UPM, so disabling the package manager removes half the project and the errors that follow
  describe missing types rather than the actual cause.

## Reporting

Rule 9 applies with full force here, because everything in this file produces a result that is easy
to assume and cheap to check. Report the numbers the tool returned, say what you restored, and if a
step was skipped, say what that leaves unknown.
