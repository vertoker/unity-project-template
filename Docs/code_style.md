# Code style

Read this before writing the first line of a new subsystem. It is the long document on purpose —
most of it is about what to refuse, and refusals are the part that has to be argued rather than
asserted.

## The hierarchy of requirements

Three layers. **A lower layer never justifies breaking a higher one.**

1. **Correctness and stability** — a machine requirement.
2. **Performance** — a machine requirement.
3. **Simplicity** — the human requirement.

The first two are older than any human preference and do not negotiate: code that is wrong, or that
drops frames, is not redeemed by being elegant.

Simplicity is the **primary human requirement**, and it outranks every other human one — habit,
symmetry, "extensibility", pattern purity, taste, consistency with a thing you wrote last year. Those
are not a fourth layer. They are arguments that have to earn their place by reducing to one of the
three above.

The test: **can you state a machine reason?** A layer with no machine reason behind it is a human
preference wearing a machine's coat. That is allowed — humans write this code — but it loses to
simplicity every time, and it must not be presented as a technical requirement.

## Three reasons to refuse — and they are not "too abstract"

"Too abstract" is unfalsifiable, so it loses every argument it enters. These three are checkable:

### A second source of truth

Two places that can disagree. The moment a fact is written down twice, one copy will be updated and
the other will not, and the bug that follows will look like anything except a duplication problem.

Ask: *where is the one place this is decided?* If the answer names two, that is the finding. Say it
that way — "this makes the menu path a second source of truth" is a fact someone can check, unlike
"this feels redundant".

### A category with exactly one member

An abstraction is a claim that a category exists. If the category has one member — one
implementation, one caller, one shape it will ever take — then the claim is false, and the
abstraction is a name for a thing that already had one.

The giveaway is that you cannot describe the category without describing the single member.

### Not worth it, by measurement

The strongest refusal available, and the only one that survives disagreement. It requires a number:
this allocated, that took this long, this made no difference. Record the measurement in
`Docs/Issues/*_history.md` so it does not get re-tested — and record what it was measured *on*, since
a refutation is scoped to the conditions that produced it.

## Abstractions

**An interface is justified by a seam, not by the importance of the type.** A seam genuinely exists
when:

- two implementations actually exist — not "might",
- a test or a DI scope is required to substitute the thing,
- the boundary crosses into another assembly,
- or the platform differs underneath.

Four niches where interfaces are the right answer: real format polymorphism; `unmanaged` constraints
so Burst can work without boxing; marker interfaces replacing a `switch`; genuine platform seams.

### Deleted without discussion

- an interface with one implementation, added "for the future",
- a `Service` that forwards to another type,
- a `Factory` wrapping a single constructor,
- a `Provider` that returns a field,
- a base class with one subclass,
- a wrapper that renames someone else's API.

**Testability does not require any of these.** It is achieved with an optional constructor parameter
and pure functions: the test writes `new Thing()`, the optional dependency arrives null, and the
type is exercised without a mock, a container or an interface. If a type cannot be constructed in a
test, that is a fact about its constructor, not an argument for an interface.

### The unit of reuse is a `static class`, not a hierarchy

Fixed taxonomy — the suffix says what is inside:

| Suffix | Contains |
|---|---|
| `*Math` | pure arithmetic |
| `*Utils` | procedures |
| `*Rules` | tables of `const` |
| `*Extensions` | `this`-methods |

Inheritance is for the rare case where the base class holds state the subclasses genuinely share.
Reaching for it to share three methods produces a hierarchy that has to be understood before any one
of those methods can be read.

## Comments

The goal is **dense and frequent, not long and rare.**

- **`///` on everything worth naming.** One or two lines, written directly inside the tag, without
  the `<param>`/`<returns>` ceremony — add those only for a genuinely ambiguous parameter. The doc
  sits flush against the member, with only attributes between them.
- **Never wedge a `//` block between a `<summary>` and the signature.** It buries the declaration and
  breaks the shape every file follows.
- **A short note in the margin** where the code is rigid for a machine reason and would otherwise
  read as a mistake. This is the highest-value comment in the codebase and the one most often
  missing.
- **Long explanations do not live in code.** They go to `Docs/adr.md` (a decision) or
  `Docs/Issues/*_history.md` (something measured and rejected), and the file keeps a single line
  pointing there. A block comment that grows past a few lines is a document that ended up in the
  wrong file — and a codebase where those blocks accumulate becomes one you have to read around.

What deserves a block at all: a non-obvious invariant; a rejected alternative and why; a decision the
next reader will mistake for a bug; a contract between files that must stay in lockstep. Never narrate
what the code already says.

## Syntax — defaults, not configuration

There is no `.editorconfig`, and the `.DotSettings` file configures exactly one thing: the
abbreviations this project treats as words. Everything else is the Rider/ReSharper default, deliberately
— a formatting rule that has to be configured is a rule someone has to maintain, argue about, and
re-apply after every tooling update.

- 120 columns, Allman braces, 4 spaces.
- `var` by default; the explicit type when the right-hand side does not name it.
- No naming rules beyond the defaults and the suffix taxonomy above.

Naming, in the form wrong → right:

- `dataManager` → `itemRegistry` — say what it holds, not that it manages.
- `ProcessData()` → `RebuildIndex()` — say what changes.
- `flag`, `temp`, `obj` → the thing they are.
- `GetX()` that computes → `CalculateX()`; `GetX()` is for retrieval.
- `IsValid` returning a reason → `TryValidate(out string reason)`.

## Errors

**`TryXxx(out …) → bool` in application layers.** An expected failure is a return value; exceptions
are for guard clauses and for the boundary where someone else's data is parsed. A per-frame path
should not be throwing at all — an exception there is a performance decision as much as a
correctness one.

**Sentinel or null**, decided by how many states exist:

- **Two states** — the field has a natural "off" inside its own domain: use the sentinel. A radius of
  zero already means "no radius".
- **Three states** — "the author never touched this" must be distinguishable from every legal value:
  use null.

**A sentinel you had to invent is a smell.** If the value has no meaning in the domain and exists
only so something could be non-null, the type wanted null, or it wanted a different shape.

## Make illegal states unrepresentable

- **The constructor clamps, it does not validate.** Validating pushes the failure to every call site
  and makes each one decide what to do; clamping makes the bad state impossible to hold. Where
  clamping would hide a real error, take the input as a type that cannot express it.
- **Store fields offset so that the zero bit pattern is legal.** A default-constructed struct is
  going to happen — in an array, in a native collection, in a `default` expression — and the only
  question is whether it lands in a valid state or a state nothing checks for.

This is a machine requirement, not a stylistic one: it is measurably cheaper than validating at every
call site, and it removes a class of bug rather than detecting it.
