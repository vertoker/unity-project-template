# Issues

Investigations, not tickets. This folder exists so that **a refutation has somewhere to live.**

`Docs/code_style.md` makes "not worth it, by measurement" the strongest available reason to refuse
something. That only works if the measurement is written down somewhere durable — otherwise the same
idea gets proposed, tried and abandoned once a year, and each round costs what the first one did.
`Docs/adr.md` records what was decided; this folder records what was *tried*.

## Four kinds of file

| Suffix | Means | Written when |
|---|---|---|
| `*_history.md` | tried, measured, and mostly refuted | after the work, whatever the outcome |
| `*_analysis.md` | feasibility; no code was written | after reading enough to answer "could we" |
| `*_spec.md` | designed, not built | the shape is settled, the work is not scheduled |
| `*_plan.md` | sequenced work, not started | the work is scheduled but not begun |

Names are lowercase `snake_case`: `burst_collision_history.md`, `netcode_analysis.md`.

## Every file starts with a status line

The first line, before the title, states what this file currently *is* — because the single most
expensive mistake a reader makes here is treating a plan as a record of something that happened.

```
> **Status:** rejected by measurement, 2026-03-14. Superseded by nothing.
> **Status:** built and shipped, 2026-05-02. Kept for the rejected alternatives.
> **Status:** designed, not built. No code exists.
```

## Every `*_history.md` carries the refutations section

```
## Hypotheses ruled out by measurement — do not re-test these

- HYPOTHESIS. Measured WHAT, got NUMBER, against THE-ALTERNATIVE-IT-LOST-TO.
  Scope: measured on HARDWARE / DATA-SET / BUILD-CONFIGURATION.
```

**The scope line is not optional.** "Refuted" is always scoped to the conditions that produced the
measurement: a technique that lost on a mid-range phone in 2026 has not been refuted for a desktop
build, and a result from a thousand-element data set says nothing about a million. Without the scope,
the section stops being evidence and becomes folklore — and folklore is harder to overturn than an
open question, because it looks like it was already settled.

Re-testing something listed here is allowed when the scope has changed. Say which part of the scope
moved, in the file, when you do.

---

## Open records

### `autoreferenced_mismatch_analysis.md` — not yet written

`Shared` and `Runtime` have `autoReferenced: false`; `Shared.Editor` and `Runtime.Editor` have it
`true`. Nothing depends on the difference today and nothing is broken by it, but the two halves of
the graph disagree for no recorded reason, which means the next person to look will have to work out
whether it was deliberate. Left as-is deliberately; a candidate for the separate cleanup pass, not
for an incidental fix.
