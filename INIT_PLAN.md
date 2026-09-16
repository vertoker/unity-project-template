# INIT_PLAN

Run this once, in a fresh clone, before any other work. Seven phases, in order. The last one deletes
this file.

**Rule 2 applies to this plan.** It does not commit. It finishes by offering one.

---

## Phase 1 — Language

Ask one question, in English, and ask nothing else yet:

> **Which language should I use when talking to you?**

Then wait. **From your next message onwards, speak that language** — the rest of the interview, every
confirmation, every summary and every report through to phase 7.

This is a phase of its own because the answer changes how the remaining phases are *conducted*, not
just what gets recorded. Bundled in with nine other questions, it arrives too late to matter: the
other nine were already asked in the wrong language.

The answer becomes `<preferred chat language>` in rule 3. It does not change what goes **into** the
repository — everything written to disk stays English, which is the other half of the same rule.

---

## Phase 2 — Interview

Ask all of it in one pass, then confirm the derived values. Do not start filling anything in until
the answers are in — phase 4 depends on them.

**Say this before the questions: anything marked optional can be answered with a dash (`—`).** Not
every project has a genre or a store; a testbed for one mechanic has neither, and inventing an answer
puts a fiction into `CLAUDE.md` that every later session reads as fact. A dash means "this does not
apply", and phase 4 drops the line rather than writing `—` into the file.

### Asked

| # | Question | | Notes |
|---|---|---|---|
| 1 | Project name | **required** | Propose the folder name as the default |
| 2 | Description, 2–3 sentences | **required** | The pitch, not the genre |
| 3 | Genre | optional | A technical project, a testbed or a prototype may genuinely have none |
| 4 | Target platforms | optional | Unknown at the start is a normal answer |
| 5 | Distribution channels | optional | Stores, launchers, direct — and often none yet |
| 6 | Company / publisher name | optional | Default: the author's nick from #7 |
| 7 | Author: nick and full name | **required** | The nick becomes a C# identifier in `Metadata.Author.*` — no spaces or punctuation. Full name may be the nick again |
| 8 | Target locales | optional | **Defaults to `English (en)`**, which is the only locale in the project. A dash means English only |
| 9 | **Autonomy mode** | **required** | `Careful`, `Bold`, `Paranoid` or `Automatic`. Explain all four from the table below. **`Careful` is the recommendation** — offer it as the default |

### Autonomy modes

| Mode | What the session may do in the Unity Editor |
|---|---|
| **`Careful`** *(recommended)* | Reads and test runs are free. Small asset edits happen, and are reported every time. Play Mode, scene mutations, menu items and arbitrary code need permission. The Editor is never left locked. |
| **`Bold`** | The whole MCP surface without asking, including Play Mode and saving scenes — but each action is announced in one line, whatever was taken is restored, and a behaviour change is not done until it has been watched happening. Deletions, bulk edits, packages, builds and new assemblies still need permission. |
| **`Paranoid`** | Reading only. Every write — including `refresh_unity` and running tests — needs permission granted in advance for that specific action, and is reported individually afterwards. |
| **`Automatic`** | No permission boundary at all: every tool, every action, no asking. Only large changes get a brief report; small ones are not narrated. **Offer this one with the warning attached** — see below. |

**If the author picks `Automatic`, say this before accepting it:** it removes the confirmation step
from deleting assets, rewriting many files at once, changing packages and running builds. Nothing
asks twice, and a mistake lands at full speed. It is the right mode for a scratch project the author
can throw away, and the wrong one for anything whose loss would hurt. Use it at your own risk.

This choice leaves no trace in the project. Phase 3 lays down one mode's files and deletes the rest.

### Derived — show and confirm, do not ask

- Folder name, Unity version (`ProjectSettings/ProjectVersion.txt`).
- `git config user.name` and `user.email` — these go into `Docs/authors.md` and are what later
  identifies the author of a test.
- The package list and the assembly graph, as they stand.

**Not asked:** namespaces (they follow from the layout) and packages (nothing is removed).

---

## Phase 3 — Lay down the autonomy mode

Let `MODE` be the choice from phase 2.

| From | To |
|---|---|
| `InitTemp/Rule4/MODE/rule_4.md` | replaces the line `<!-- RULE_4 … -->` in `CLAUDE.md` |
| `InitTemp/Rule4/MODE/unity_editor.md` | `.claude/rules/unity_editor.md` |
| `InitTemp/Rule4/MODE/unity_verify.md` | `.claude/skills/unity-verify/SKILL.md` |
| `InitTemp/Rule4/MODE/settings.fragment.json` | merged into `.claude/settings.json` |

The fragment carries a `permissions` block for `Careful`, `Bold` and `Automatic` — narrow for the
first two, broad for the last — and is empty for `Paranoid`. Merge it as-is rather than inventing a
block.

`Automatic`'s list opens with a bare `"*"` and then names the tools individually anyway. That is
deliberate: a bare `"*"` is not attested anywhere as a whole-surface wildcard — every real allow list
uses `*` only as a name suffix (`mcp__X__*`) or inside parentheses (`Bash(ls:*)`) — so the explicit
entries are what actually carry the mode if `"*"` turns out to match nothing. Do not "tidy" the
duplication away.

Then **delete `InitTemp/` entirely.**

Check: `rg -i "careful|bold|paranoid|automatic" -g "*.md"` finds nothing outside this file. If it
does, a mode name leaked into prose and has to be rewritten as a reference to rule 4.

**Scope it to `*.md`.** Unity writes the literal word `Automatic` into `ProjectSettings.asset` eight
times over — an unscoped grep reports those and the real check gets ignored as noise.

---

## Phase 4 — Fill

### Placeholders — all thirteen

**Before replacing anything, print a table: placeholder → occurrences found.** After replacing, print
it again with occurrences replaced. The two columns must match, and the second pass must find zero.
Thirteen substitutions across a dozen files, done by hand, is exactly the situation where one gets
missed silently.

**Answers given as a dash.** For an optional question the author declined, **delete the whole line
rather than writing `—` into it.** `- **Genre:** —` is noise that reads like a missing value someone
forgot; the absent line reads like what it is. This applies to the `Genre`, `Target platforms` and
`Distribution channels` bullets in `CLAUDE.md`. The count of "occurrences replaced" includes the
deleted ones — what must reach zero is placeholders *left*, not substitutions made.

**Substituted prose gets rewrapped to 120 columns.** A 2–3 sentence description dropped into one
line leaves a 200-character line in the middle of a file where everything else wraps. Rewrap the
paragraph you just edited; do not reflow the rest of the file.

| Placeholder | Files |
|---|---|
| `<project name>` | `CLAUDE.md`, `Docs/architecture.md`, `Assets/Code/CLAUDE.md` |
| `<project description, 2-3 sentences>` | `CLAUDE.md` |
| `<genre>` | `CLAUDE.md` |
| `<target platforms>` | `CLAUDE.md` |
| `<distribution channels>` | `CLAUDE.md` |
| `<preferred chat language>` | `CLAUDE.md` (rule 3, twice) |
| `<author nick>`, `<author full name>` | `Docs/authors.md` |
| `<git user name>`, `<git user email>` | `Docs/authors.md` |
| `<systems>` | `Docs/architecture.md`, `Assets/Code/Systems/CLAUDE.md` — **table rows**, not prose: the line sits under a `\| System \| Owns \| Tests \| Editor \|` header. A fresh project has none, so replace it with the single row `\| _none yet_ \| \| \| \|` and invent nothing |
| `<target locales>` | `Assets/Addressables/CLAUDE.md` |
| `<company name>` | `ProjectSettings.asset`, in phase 5 |

**Not in scope:** `.claude/rules/*` holds no placeholders — mode variance is carried by phase 3, not
by substitution. `<summary>`, `<param>` and `<returns>` in `Docs/code_style.md` are XML doc tags, not
slots.

**Leave the six empty `CLAUDE.md` files empty** (`Shared/`, `Shared/Tests/`, `Shared/Editor/`,
`Runtime/`, `Runtime/Tests/`, `Runtime/Editor/`). They are reserved slots, and filling one in with a
description of an empty folder is the failure the `new-claude-md` skill exists to prevent.

### Code — three files

The template ships no `.cs`. Create these now, using the templates in the skills:

| File | Template | From |
|---|---|---|
| `Assets/Code/Shared/Tests/Metadata.cs` | namespace `Shared.Tests`, `Category.Self = "Shared"` | `add-assembly` |
| `Assets/Code/Runtime/Tests/Metadata.cs` | namespace `Runtime.Tests`, `Category.Self = "Runtime"` | `add-assembly` |
| `Assets/Code/Shared/Constants/EditorNames.cs` | namespace `Shared.Constants` | below |

`Metadata.Author` gets one constant per developer in `Docs/authors.md` — the nick as both the member
name and the value.

```csharp
namespace Shared.Constants
{
    public static class EditorNames
    {
        private const string ToolsPath = "Tools";

        // One entry per editor tool system: ToolsPath + "/<System>".
        // Menu paths live here and never as literals in a [MenuItem] attribute.
    }
}
```

Let Unity generate the `.meta` files — never write one by hand.

---

## Phase 5 — Rename

- `ProjectSettings/ProjectSettings.asset`: `productName` (`UndefinedGame` → the project name) and
  `companyName` (`UndefinedCompany` → the company name from phase 2).
- **Rename `vertoker_template.sln.DotSettings` to `<folder name>.sln.DotSettings`.** Unity names the
  solution after the project folder, so a `.DotSettings` named after the old one is orphaned
  silently — nothing breaks, the abbreviations simply stop applying. See `Docs/adr.md`.

The assembly names `Shared` and `Runtime` are deliberately generic and are **not** renamed. `.sln`
and `.csproj` are generated — do not touch them.

---

## Phase 6 — Verify

Follow rule 4 for the chosen mode, and the `unity-verify` skill. Under `Paranoid`, ask for the writes
this phase needs before starting it.

1. **Check the MCP port first.** Open the MCP for Unity window in the Editor and confirm its HTTP
   base URL matches `.mcp.json` (`http://127.0.0.1:8080`). They disagree whenever this machine's
   `EditorPrefs` were set for another project, and the symptom — "the server is not responding" —
   looks like a dead server rather than a mismatch. This is the most likely failure in the whole
   plan.
2. Confirm the server answers. If it does not, start one in a separate console window; never a
   second instance.
3. `refresh_unity`, then wait for the domain reload to finish.
4. Read the console. It must be clean — and an empty console with a test total of `0` means nothing
   compiled, not that everything is fine.
5. `Tools/AssemblyBuilder/Build All`, then read the console again. Confirm `rootNamespace` survived
   in all six `.asmdef` files.
6. Run the tests. Report real numbers.
7. Under `Bold`: open `Assets/Scenes/Boot.unity`, announce it, enter Play Mode, take a screenshot,
   exit, and restore whatever was open.

---

## Phase 7 — Finish

**Only if phase 4's second pass found zero remaining placeholders.** If any are left, go back.

1. Delete `INIT_PLAN.md`.
2. Remove the init banner from the top of `CLAUDE.md` — the two lines beginning
   **"This project is not initialized yet."**
3. **Replace `README.md` wholesale** with the project name as an `# H1` and the 2–3 sentence
   description from phase 2. Nothing else — no stack list, no getting-started section, no link map.

   The template README describes the template, so every line of it is false once this project exists;
   and a repository README is the one file read by people who are not in a session, for whom the
   agent layer is not the subject. `CLAUDE.md` is the map for anything else, and it stays current
   because sessions read it.

   ```markdown
   # Starfall

   A twin-stick shooter about salvaging a dead orbital station. Runs on PC and Android.
   ```

4. Confirm the cleanup:
   - `rg "<project name>"` — empty,
   - `InitTemp/` — gone,
   - `rg -i "careful|bold|paranoid|automatic" -g "*.md"` — empty,
   - `CLAUDE.md` — under 300 lines,
   - `README.md` — no longer mentions the template, `INIT_PLAN` or the autonomy modes.
5. Print a summary: what was filled in, which mode was chosen, what phase 6 actually returned, and
   anything that could not be verified.
6. **Offer a commit. Do not make one** — rule 2.
