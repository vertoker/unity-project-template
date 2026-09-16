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
the answers are complete — phase 4 depends on every one of them.

### Asked

| # | Question | Notes |
|---|---|---|
| 1 | Project name | Propose the folder name as the default |
| 2 | Description, 2–3 sentences | The pitch, not the genre |
| 3 | Genre | |
| 4 | Target platforms | |
| 5 | Distribution channels | Stores, launchers, direct |
| 6 | Company / publisher name | Default: the author's nick from #7 |
| 7 | Author: nick, full name, role, area of responsibility | The nick becomes a C# identifier in `Metadata.Author.*` — no spaces or punctuation |
| 8 | Target locales | Only `English (en)` exists in the project today |
| 9 | **Autonomy mode: `Careful`, `Bold` or `Paranoid`** | Explain all three from the table below. **`Careful` is the recommendation** — offer it as the default |

### Autonomy modes

| Mode | What the session may do in the Unity Editor |
|---|---|
| **`Careful`** *(recommended)* | Reads and test runs are free. Small asset edits happen, and are reported every time. Play Mode, scene mutations, menu items and arbitrary code need permission. The Editor is never left locked. |
| **`Bold`** | The whole MCP surface without asking, including Play Mode and saving scenes — but each action is announced in one line, whatever was taken is restored, and a behaviour change is not done until it has been watched happening. Deletions, bulk edits, packages, builds and new assemblies still need permission. |
| **`Paranoid`** | Reading only. Every write — including `refresh_unity` and running tests — needs permission granted in advance for that specific action, and is reported individually afterwards. |

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

The fragment carries a `permissions` block for `Careful` and `Bold`, and is empty for `Paranoid` —
merge it as-is rather than inventing a block.

Then **delete `InitTemp/` entirely.**

Check: `rg -i "careful|bold|paranoid"` finds nothing outside this file. If it does, a mode name leaked
into prose and has to be rewritten as a reference to rule 4.

---

## Phase 4 — Fill

### Placeholders — all fifteen

**Before replacing anything, print a table: placeholder → occurrences found.** After replacing, print
it again with occurrences replaced. The two columns must match, and the second pass must find zero.
Fifteen substitutions across a dozen files, done by hand, is exactly the situation where one gets
missed silently.

| Placeholder | Files |
|---|---|
| `<project name>` | `CLAUDE.md`, `Docs/architecture.md`, `Assets/Code/CLAUDE.md` |
| `<project description, 2-3 sentences>` | `CLAUDE.md` |
| `<genre>` | `CLAUDE.md` |
| `<target platforms>` | `CLAUDE.md`, `Docs/dependencies.md` |
| `<distribution channels>` | `CLAUDE.md` |
| `<preferred chat language>` | `CLAUDE.md` (rule 3, twice) |
| `<author nick>`, `<author full name>`, `<author role>`, `<author area>` | `Docs/authors.md` |
| `<git user name>`, `<git user email>` | `Docs/authors.md` |
| `<systems>` | `Docs/architecture.md`, `Assets/Code/Systems/CLAUDE.md` — an empty table is correct in a fresh project; say so rather than inventing rows |
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
   - `rg -i "careful|bold|paranoid"` — empty,
   - `CLAUDE.md` — under 300 lines,
   - `README.md` — no longer mentions the template, `INIT_PLAN` or the autonomy modes.
5. Print a summary: what was filled in, which mode was chosen, what phase 6 actually returned, and
   anything that could not be verified.
6. **Offer a commit. Do not make one** — rule 2.
