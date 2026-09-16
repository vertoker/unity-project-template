# Rule 4 — The Unity Editor is read-only through MCP

A `unity-mcp` server exposes the running Editor to this session. This file is always loaded, because
the boundary it draws applies to every task, not to particular files.

## What you may do

**Read.** Editor and project state, the console, type information, scenes and assets queried as data.
That is the whole list.

## What needs permission, granted in advance

Everything else, including things that look like reads:

- `refresh_unity` — it triggers a recompile and a domain reload,
- running tests, EditMode or PlayMode,
- opening or saving a scene,
- entering Play Mode,
- invoking a menu item,
- executing any code in the Editor,
- creating, editing or deleting any asset.

**Permission is per action, not per session.** A grant for one refresh is not a grant for the next
one. "It is obviously harmless" is not a grant, and neither is the author having allowed the same
thing an hour ago.

Ask specifically — name the action and what it will touch — and **carry on with everything else in
the task while you wait.** A blocked step is not a blocked task: read what you can, write what you
can in `.cs` and `.md`, and hand over a precise request.

## Reporting a granted action

Afterwards, individually: what you ran, what it returned, and what changed as a result. A batch of
granted actions is reported as a list, not as "did the things we discussed".

## Working with the server

**1. Check whether it is already running before starting anything.** Query `mcpforunity://instances`,
or make a request to the URL in `.mcp.json`. **If it answers, start nothing.**

**2. Starting a server is itself a write** — ask first. If granted, start it in a **separate console
window the author can close**, never as a child of this session:

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
- **`refresh_unity` returns before the domain reload finishes.** Anything run immediately after it
  dies in a way that looks unrelated.
- **A stuck call is cleared, not worked around.** Starting a second server to escape one turns a
  stuck call into two Editors on one endpoint.
- **An empty console with a test total of `0` means nothing compiled**, not that everything is clean.
  Look for `error CS` in `Library/Bee/tundra.log.json`.
- **A batch compile must never be passed `-noUpm`.** Most of this project's dependencies arrive over
  git-UPM, so disabling the package manager removes half the project and the errors that follow
  describe missing types rather than the actual cause.

## Reporting

Rule 9 matters more here than anywhere, because under this rule most things stay unverified by
default. Say plainly what you could not check and why. An unverified claim presented confidently is
the failure this rule exists to prevent — not the Editor being touched.
