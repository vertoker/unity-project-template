# Troubleshooting

Symptom on the left, cause on the right. Read the cause before acting — most entries here exist
because the obvious fix makes things worse.

## MCP and the Editor

| Symptom | Cause |
|---|---|
| The server does not answer on a fresh clone | `.mcp.json` expects `http://127.0.0.1:8080/mcp`, the package default. The actual port lives in `EditorPrefs`, which is per-machine and not in the repository, so a machine that was once set to something else keeps that value. Open the MCP for Unity window, read the HTTP base URL it shows, and make the two agree — **before** concluding the server is down. |
| The server does not answer, and the port is right | It is not running. Check first (`mcpforunity://instances`, or a request to the URL), and only then start one — in a **separate console window the author can close**, never as a child of the session. |
| Launching it on Windows prints `is not recognized as an internal or external command` | `cmd /k` strips the outermost pair of quotes from its argument, so a command whose first character is a quote gets mangled. Leave the `uvx` path unquoted. On macOS and Linux this does not arise — start it from an ordinary terminal. |
| `uvx` is not found | It is expected on `PATH`. Failing that the package looks in `/opt/homebrew/bin` and `/usr/local/bin` (macOS), `/usr/local/bin` and `/usr/bin` (Linux), `%LOCALAPPDATA%\Programs\uv` and `%ProgramFiles%\uv` (Windows). An override lives in the MCP for Unity Advanced Settings. |
| The launch command was reconstructed and the version is wrong | The version in the package's `package.json` is the **Unity package** version; the server is a separate PyPI package on its own version, and the two differ. Copy the command from the MCP for Unity window, which prints the real one, including whether it needs `--offline` or `--prerelease`. |
| `GET` on the endpoint returns `406` | Normal. Streamable HTTP rejects a request without the right `Accept` header — it means the server is up, not that it is broken. |
| `Multiple Unity instances are connected` | A second server was started while one was already live. Do not start another; find the one that is running. |
| Commands reach the wrong Editor | Two Editors share one server. Select with the full `Name@hash` — a short name is rejected. |
| A tool call hangs and stays stuck | Clear the stuck call rather than launching a second server. A second server turns one stuck call into two Editors on one endpoint. |
| A test run right after a refresh dies | `refresh_unity` returns before the domain reload finishes. Wait for the reload, confirm it completed, then run. |
| The console is empty and the test total is `0` | Nothing compiled — an empty console is not a clean console. Look for `error CS` in `Library/Bee/tundra.log.json`. |
| A new `.cs` never appears in its assembly | It never joined the asmdef: no errors, no tests. Rename the file to force a reimport. |

## Batch compilation

| Symptom | Cause |
|---|---|
| A batch-mode compile fails to resolve packages | `-noUpm` was passed. **Never add it.** Most of this project's dependencies arrive over git-UPM, so disabling the package manager removes half the project. |
| A batch run and the Editor fight over the project | Both hold the same `Library`. Close one, or use a ParrelSync clone. |

## Assets and the assembly graph

| Symptom | Cause |
|---|---|
| A reference reads back null at runtime, with no error anywhere | An asset GUID and a script GUID were swapped. They are the same shape and sit in the same file; only the asset's own `.asset.meta` GUID belongs in an asset reference. |
| An asset edit disappears | It was written while the Editor was running, and the Editor's next `SaveAssets` reverted it. |
| An asset is detached from everything referencing it | Its `.meta` was hand-written, so its GUID is new. Unity writes `.meta` files; nothing else should. |
| References went missing after `Build All` | A cycle failed with a `Debug.LogError` and that branch was abandoned mid-build. **Read the console before touching any `.asmdef`** — the assemblies that lost references are usually not the ones in the cycle. |
| An assembly is absent from the graph, silently | Either it has no builder asset, or its builder's `_definitions` points at a package that is not installed. An unresolvable GUID is skipped without a warning. |
| An `.asmdef` field reverted after a build | `Build` rewrites the file whole from its model. The model knows twelve fields; anything else is dropped. See `.claude/rules/assemblies.md`. |

## Content and localization

| Symptom | Cause |
|---|---|
| A scene fails to load at runtime | It is not in an Addressables group. Being in the project — or in the build settings — is not the same as being shipped. |
| A string shows as its key | The entry exists in one of the three string assets but not the others. |
| A UI element vanishes with no error | A `--` inside a `.uxml` comment broke the file as XML. Unity says nothing. |

## Quoting package code

Packages resolve into `Library/PackageCache/`, which is gitignored and carries a commit hash in its
folder name. **Cite package code by symbol, never by file-and-line** — `#beta` in the manifest tracks
a moving branch, so line numbers are wrong at the next resolve, and a path with a hash in it does not
exist on anyone else's machine.
