# Dependencies

How third-party code gets into this project, which mechanism to pick, and what each one costs.

Adding, removing or upgrading any of this is the author's call, not yours — rule 11.

## Four ways in

### 1. UPM from a git URL

`Packages/manifest.json`, with the repository URL. Most of this project's non-Unity dependencies
arrive this way.

Costs: the package is pinned to a branch or a tag, not a version — `#beta` and `#upm` in the manifest
below both track a moving branch, so an upgrade can arrive without anyone asking for one, and the
lock file is the only record of what was actually resolved. Nothing in the package is editable; a
local fix means forking the repository. **Anything that resolves packages must be allowed to reach
the network** — see the `-noUpm` trap in `Docs/troubleshooting.md`.

### 2. UPM from the registry

Same file, a version number instead of a URL. Semver, upgradeable, resolvable offline once cached.
Prefer this whenever the package exists on the registry.

### 3. NuGet, through NuGetForUnity

For plain .NET libraries with no Unity integration. `packages.config` and `NuGet.config` sit at the
repository root; restored assemblies land in `Assets/Packages/`, which is gitignored.

Costs: the restore has to succeed on every clone, and a NuGet package that pulls a transitive
dependency targeting an unsupported framework fails at import rather than at restore. Check the
target framework before adding one.

### 4. Vendored into `Assets/`

Source copied into the repository. Last resort, for code that must be edited, or that ships no
package at all.

Costs: it is now this project's code — upstream fixes have to be merged by hand, and the boundary
between "theirs" and "ours" stops being visible to anyone reading the diff. If you vendor something,
say where it came from and at what revision, in a file next to it.

## What is installed

Versions are as pinned; git entries track a branch and have no version.

### From git

| Package | Source | What it is |
|---|---|---|
| `com.coplaydev.unity-mcp` | CoplayDev/unity-mcp `#beta` | exposes the running Editor to an agent |
| `com.cysharp.unitask` | Cysharp/UniTask | allocation-free async for Unity |
| `com.dbrizov.naughtyattributes` | dbrizov/NaughtyAttributes `#upm` | inspector attributes |
| `com.github-glitchenzo.nugetforunity` | GlitchEnzo/NuGetForUnity | the NuGet path above |
| `com.neuecc.unirx` | neuecc/UniRx | reactive streams; the project's event mechanism |
| `com.veriorpies.parrelsync` | VeriorPies/ParrelSync | a second Editor against one clone |
| `com.vertoker.assemblybuilder` | vertoker/assemblybuilder | generates the `.asmdef` graph |
| `jp.hadashikick.vcontainer` | hadashiA/VContainer | dependency injection |

### From the registry

| Package | Version | | Package | Version |
|---|---|---|---|---|
| `com.unity.render-pipelines.universal` | 17.6.0 | | `com.unity.recorder` | 5.1.7 |
| `com.unity.entities` | 6.6.0 | | `com.unity.timeline` | 6.6.0 |
| `com.unity.entities.graphics` | 6.6.0 | | `com.unity.ugui` | 2.6.0 |
| `com.unity.addressables` | 2.11.2 | | `com.unity.ai.navigation` | 2.0.14 |
| `com.unity.addressables.android` | 1.1.0 | | `com.unity.dedicated-server` | 3.0.0 |
| `com.unity.localization` | 1.5.13 | | `com.unity.memoryprofiler` | 1.1.12 |
| `com.unity.inputsystem` | 1.20.0 | | `com.unity.performance.profile-analyzer` | 1.4.0 |
| `com.unity.test-framework` | 1.8.0 | | `com.unity.project-auditor-rules` | 1.0.3 |
| `com.unity.2d.psdimporter` | 15.0.1 | | `com.unity.mobile.android-logcat` | 1.4.7 |
| `com.unity.sharp-zip-lib` | 1.4.2 | | `com.unity.pipeline` | 0.7.0-exp.1 |
| `com.unity.ide.rider` | 3.0.40 | | `com.unity.ide.visualstudio` | 2.0.28 |

Plus 30 built-in `com.unity.modules.*` at 1.0.0. `scopedRegistries` is empty.

**Presence is not a decision.** This manifest is broad because the template is broad; see "Installed
but not used" in the root `CLAUDE.md`. A package listed here has not been chosen for
`<target platforms>` until something deliberately uses it.

## Not integrations

These look like dependencies and are not — do not document them as such, and do not treat an upgrade
to one as a dependency change:

- **Built-in modules** (`com.unity.modules.*`) ship with the engine. They are present because the
  project template listed them, not because anything selected them.
- **IDE packages** (`com.unity.ide.rider`, `com.unity.ide.visualstudio`) affect project file
  generation on a developer's machine and nothing that ships.
- **`Library/PackageCache/`** is resolved output. Nothing in it is edited, and nothing in it is
  quotable by line number — see `Docs/troubleshooting.md`.
