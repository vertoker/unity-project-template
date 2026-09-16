# Authors

**This table exists to tie a nickname to a git identity.** That is its whole job — it is not credits,
not a roster, and not a record of who owns what. Keep it to the four columns.

| Nickname | Full name | `git user.name` | `git user.email` |
|---|---|---|---|
| `<author nick>` | <author full name> | `<git user name>` | `<git user email>` |

**Nickname** is also the value of a `Metadata.Author.*` constant, so it has to survive being written
into C#: no spaces, no punctuation, starts with a letter. The constant's *name* is that nickname in
PascalCase — `vertoker` becomes `Vertoker = "vertoker"`.

## What reads it

`[Author]` on a test, via `.claude/rules/tests.md`. One row means the author is unambiguous and
nobody gets asked. Several rows mean the current developer is resolved by matching
`git config user.name` / `user.email` against the last two columns — which is the only reason those
columns exist. An unmatched git identity is the one case that produces a question.

## Adding a developer

One change, two places:

1. a row here,
2. a constant in **every** `Metadata.cs` — `Assets/Code/Shared/Tests/Metadata.cs`,
   `Assets/Code/Runtime/Tests/Metadata.cs`, and one per system that has tests.

Doing only the first gives a name the test attributes cannot reference; doing only the second gives
an author nobody can resolve to a git identity. Neither failure is loud.
