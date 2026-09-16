# Rule 4 — Everything is yours: move, and report the large things

A `unity-mcp` server exposes the running Editor to this session. This file is always loaded, because
the boundary it draws — or rather, the absence of one — applies to every task.

**The author chose this mode knowing what it removes.** Do not re-litigate it by asking for
permission anyway; a mode that asks is one of the other three.

## Yours without asking

All of it. Reading and writing state, the console, types, scenes and assets; `refresh_unity`; EditMode
and PlayMode tests; opening, saving and mutating scenes; entering and leaving Play Mode; screenshots;
executing code in the Editor; deleting assets; bulk edits; creating assemblies, builders and
collection entries; package changes; builds; the shell.

## Report the large things

Not an announcement beforehand — a short note afterwards, one or two lines, for anything that:

- deletes,
- rewrites many files at once,
- changes `Packages/manifest.json` or the assembly graph,
- changes what the game *does*,
- or would surprise the author when they next open the project.

Small edits are not narrated. The point of this mode is that the author reads a summary, not a
transcript — so the summary has to be worth reading, which means leaving the small things out.

## Restore what you took

Exit Play Mode. Reopen whatever scene was open, and drop your own changes to it unless changing it
*was* the task. **A session that leaves the Editor in Play Mode, or on a scene the author did not
open, has not finished.** Permission was removed; tidiness was not.

## Verification is not optional — it matters more here

This mode removes the confirmation step, which was also the step where a human might have caught a
bad call before it landed. Nothing catches it now except you checking.

A change to what the game does is not done until you have seen it do it. "It compiles" and "it looks
correct" are not verification. Rule 9 — never claim a result the tool did not return — is the only
remaining safeguard, so it applies harder here, not less. The `unity-verify` skill is the procedure.

## Still worth a question

Not a permission boundary, a judgement one. Ask when the *intent* is unclear rather than when the
action is dangerous: an ambiguous requirement, two reasonable designs, a change that would throw away
work the author may still want. Acting confidently on a misread request costs more in this mode than
in any other, because nothing interrupts it.

## Working with the server

**1. Check whether it is already running before starting anything.** Query `mcpforunity://instances`,
or make a request to the URL in `.mcp.json`. **If it answers, start nothing.**

**2. Only if it does not answer, start one** — in a **separate console window the author can close**,
never as a child of this session:

```
uvx --offline --from "mcpforunityserver==10.1.2" mcp-for-unity --transport http --http-url http://127.0.0.1:8080 --project-scoped-tools
```

**That is the shape, not the string.** The version, the `--offline` / `--prerelease` flags and the
path to `uvx` all come from the MCP for Unity window in the Editor, which prints the exact command.
Do not reconstruct it from `package.json` — the version there is the *Unity package* version, not the
server package version, and they differ. The builder is `ServerCommandBuilder.TryBuildCommand`.

**Opening that window is platform-specific.** On macOS and Linux, start it from an ordinary
terminal. On Windows, use `cmd /k` and **leave the `uvx` path unquoted** — `cmd` strips the
outermost pair of quotes from its argument, so a command whose first character is a quote comes
back as `is not recognized as an internal or external command`.

`uvx` is normally on `PATH`. When it is not, the package looks in `/opt/homebrew/bin` and
`/usr/local/bin` on macOS, `/usr/local/bin` and `/usr/bin` on Linux, and `%LOCALAPPDATA%\Programs\uv`
on Windows.

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
