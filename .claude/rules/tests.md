---
paths:
  - "**/Tests/**/*.cs"
  - "**/Tests/*.cs"
---

# Writing a test

**Which tests you may RUN, and through what, is rule 4** — that is a fact about a phase of work and
about the Editor, not about a file, so it does not live here. Which mode to pick for a given change
is `Docs/testing.md`. This file is about writing one.

## Every test carries its own attributes — no exceptions

A generation-time requirement, not a cleanup pass. A test missing any of them is unfinished work,
exactly like one that does not compile.

```csharp
[Test]
[Author(Metadata.Author.Nickname)]
[Category(Metadata.Category.Self)]
[Category(Metadata.Category.Normal)]
public void Foo_DoesBar() { }
```

- `[Author(...)]` — who wrote it. See "Choosing the author" below.
- `[Category(Metadata.Category.Self)]` — which layer the test belongs to, so one filter selects or
  excludes a whole assembly's worth.
- `[Category(Metadata.Category.*)]` — exactly one difficulty, never two, never none.

The NUnit attribute stays on top; the metadata attributes sit directly above the signature. A
parameterized method gets one set for the whole method, not one per `[TestCase]` — and it carries
`[TestCase]` **alone**, never `[Test]` as well. NUnit reads a bare `[Test]` as a case with no
arguments and errors on the method, so the pair does not merely read oddly, it fails to run.

## Choosing the author

1. If `Docs/authors.md` lists **one** developer, use them. Do not ask.
2. If it lists several, match the current one by `git config user.name` / `user.email` against the
   columns in that table.
3. Only if neither matched, ask.

## Every `Tests/` folder owns a `Metadata.cs`

Two things differ between copies — the `namespace`, and `Category.Self`, which is that namespace
minus the trailing `.Tests` (`Shared.Tests` → `"Shared"`, `ItemSystem.Tests` → `"ItemSystem"`). The
formula is restated as a comment inside the file; keep it there.

A new test folder gets its `Metadata.cs` **before** its first test. The `add-assembly` skill carries
the full file.

## Difficulty scale — runtime cost first, setup weight second

| Category | What lands here |
|---|---|
| `VeryEasy` | Pure arithmetic and struct logic. No allocation, no Unity objects, microseconds. |
| `Easy` | A handful of asserts over small managed objects, or one small native collection. |
| `Normal` | Native collections with an `Allocator`, `GameObject` or UI Toolkit elements, editor operations, loops over dozens of items. |
| `Hard` | Growth and defragment paths, randomized or invariant-sweeping runs, disk IO, whole-model serialization or validation. |
| `Extreme` | Stress runs over thousands of elements, full round trips, anything a corpus drives. |

Judge per test, not per file — a randomized stress case inside an otherwise `Easy` fixture is `Hard`
on its own.

**`Extreme` is a MEASURED threshold: longer than five seconds is `Extreme`, whatever the test looks
like.** Categories are this project's only mechanism for keeping a slow test out of a fast lane, so
the line has to be a number rather than a judgment. A test that crosses it is promoted even when
nothing about it reads as stressful, and a fixture whose cases straddle the line splits across two
categories. Measure before promoting rather than guessing.

The constants carry an ordinal prefix in their *string* value (`1_very_easy` … `5_extreme`) purely so
the Test Runner's category dropdown lists them cheapest-first instead of alphabetically. Code refers
to them by member name; the prefix only matters where the raw string surfaces, i.e. in a filter.
There is no exclude parameter when filtering by category — "everything except the slowest" is spelled
as an include list of the other four.

**Categories are the only mechanism for marking a heavy test.** `[Explicit]`, `[Ignore]` and
`[ConditionalIgnore]` are deliberately not used: a test that never runs rots silently, whereas a
categorized one stays in the suite and is merely filtered out of the narrower runs.

## An expected error is declared, not tolerated

A test that legitimately provokes a logged error declares it with `LogAssert.Expect` and stays green.
A red test nobody intends to fix teaches the next reader that red is normal, which costs more than
the test is worth.

## Where a test goes

In its assembly's own `Tests/` folder, alongside that assembly's `*.Tests.asmdef`. The template ships
two: `Assets/Code/Shared/Tests` (`Shared.Tests`) and `Assets/Code/Runtime/Tests` (`Runtime.Tests`),
both EditMode. A system under `Assets/Code/Systems/` gets its own the same way — use the
`add-assembly` skill, which also covers why a PlayMode assembly is not an EditMode one with the name
changed.

**A `Tests/` folder without its own `.asmdef` compiles INTO the assembly above it**, which drags
NUnit into a runtime assembly and breaks the build. Create the assembly, do not skip it.

**The word "Test" in a path does not make a folder an NUnit suite.** Scratch folders for manual or
PlayMode experiments exist; they have no asmdef and are not an automated suite. Do not extend this
rule to cover one just because its name matched.

## Traps

- **A new `.cs` may never join its asmdef** — no compile errors, and a test total of zero. Rename the
  file to force the import.
- **If the total test count did not move, the run used a stale assembly.** Refresh first, and confirm
  the refresh returned before running.
- **Never edit a `.cs` during a PlayMode run** — the recompile kills the run, and the wreckage looks
  like unrelated failures.
