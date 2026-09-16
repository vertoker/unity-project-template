# Authors

Who works on this project. This is a working table, not credits — two mechanisms read it.

| Nick | Full name | Role | Area | `git user.name` | `git user.email` |
|---|---|---|---|---|---|
| `<author nick>` | `<author full name>` | `<author role>` | `<author area>` | `<git user name>` | `<git user email>` |

**Nick** is also the member name in `Metadata.Author.*`, so it is a valid C# identifier: no spaces, no
punctuation, starts with a letter.

## What reads this file

- **`[Author]` on a test** (`.claude/rules/tests.md`). One row means the author is unambiguous and
  nobody gets asked. Several rows mean the current developer is identified by matching
  `git config user.name` / `user.email` against the last two columns — which is why those columns are
  not decoration. Only an unmatched git identity produces a question.
- **Anything that needs to know who owns an area** — the Area column is what makes "ask the person
  who owns this" answerable without asking around.

## Adding a developer

One edit, two places, in the same change:

1. a row here,
2. a constant in **every** `Metadata.cs` — `Assets/Code/Shared/Tests/Metadata.cs`,
   `Assets/Code/Runtime/Tests/Metadata.cs`, and one per system that has tests.

Doing only the first produces a name the test attributes cannot reference; doing only the second
produces an author nobody can resolve to a git identity. Neither failure is loud.
