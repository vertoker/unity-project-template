---
paths:
  - "Packages/manifest.json"
  - "Packages/packages-lock.json"
  - "**/packages.config"
  - "**/NuGet.config"
---

# Bringing third-party code in

**What is installed is `Packages/manifest.json`.** There is no second list, and this file deliberately
does not keep one — a copy would drift, and the drifted copy is the one someone would read. Read the
manifest. This file is about *how* something gets added and what each route costs.

Adding, removing or upgrading any of it is the author's call, not yours — rule 11.

## Four routes

### 1. UPM from a git URL

The manifest, with a repository URL. Most of this project's non-Unity dependencies arrive this way.

Costs: pinned to a branch or tag, not a version — a `#branch` suffix tracks something that moves, so
an upgrade can arrive without anyone asking for one, and `packages-lock.json` is the only record of
what actually resolved. Nothing in the package is editable; a local fix means forking. **Anything
resolving packages needs network access** — see the `-noUpm` trap in `Docs/troubleshooting.md`.

### 2. UPM from the registry

Same file, a version instead of a URL. Semver, upgradeable, resolvable offline once cached. Prefer
this whenever the package exists on the registry.

### 3. NuGet, through NuGetForUnity

For plain .NET libraries with no Unity integration. `Assets/packages.config` and
`Assets/NuGet.config` hold the manifest and the feed; restored assemblies land in `Assets/Packages/`,
which is gitignored.

Costs: the restore has to succeed on every clone, and a package pulling a transitive dependency that
targets an unsupported framework fails at *import*, not at restore. Check the target framework first.

### 4. Vendored into `Assets/`

Source copied into the repository. Last resort — for code that must be edited, or that ships no
package at all.

Costs: it is this project's code now. Upstream fixes are merged by hand, and the line between
"theirs" and "ours" stops being visible to anyone reading a diff. If you vendor something, record
where it came from and at what revision, in a file beside it.

## Not integrations

These look like dependencies and are not. Do not document them as such, and do not treat upgrading
one as a dependency change:

- **Built-in modules** (`com.unity.modules.*`) ship with the engine. They are in the manifest because
  the project template listed them, not because anything chose them.
- **IDE packages** (`com.unity.ide.*`) affect project-file generation on one machine and nothing that
  ships.
- **`Library/PackageCache/`** is resolved output. Nothing in it is edited, and nothing in it is
  quotable by line number — the folder name carries a commit hash that differs per machine.

## Presence is not a decision

The manifest is broad because the template was broad. A package being installed is inventory, not a
choice — see "Installed but not used" in the root `CLAUDE.md`. It earns a row in the Technical
overview table the moment something uses it deliberately, and not before.
