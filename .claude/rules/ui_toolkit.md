---
paths:
  - "**/*.uxml"
  - "**/*.uss"
  - "**/*.tss"
---

# UI Toolkit: the files are editable, only placement is fixed

Unlike other Unity assets, these three are plain text and freely editable — `.uxml` is XML, `.uss`
and `.tss` are stylesheets. What is *not* free is **where** a declaration lands, and that is decided
by scope, not by convenience.

## Where a declaration goes

| File | Owns |
|---|---|
| `Assets/UI/TSS/ProjectTheme.tss` | the theme and its tokens — colours, sizes, radii, spacing |
| `Assets/UI/USS/ProjectStyle.uss` | every shared control class, i.e. what a button looks like in this project |
| `<Screen>Style.uss` next to its `.uxml` | every number that belongs to that one screen |

Two moves are always wrong:

- **Widening a shared class to serve one screen.** If a screen needs styling `ProjectStyle.uss` does
  not cover, it gets its own sibling stylesheet. A shared class edited for one caller silently
  restyles every other one.
- **Restating a token as a literal.** If a rule needs a colour, a size or a radius the theme already
  names, it reads the `var(--…)`. A raw value is a claim that no token fits, and the next reader
  will believe it.

Element and class names are `kebab-case` (`boot-screen`, `console-tab`).

## Traps

- **A `--` inside a `.uxml` comment breaks the file as XML**, and Unity says nothing. The element
  simply vanishes from the tree.
- **A freshly assigned `icon-image` reads back null** until the `.svg` is force-reimported.
- **A child element inside a `Button` kills the text measure** — the button collapses to padding plus
  border. It is not the stylesheet.
- **Rewriting `project://` references by regex** drops the closing `&quot;` and mangles names
  containing spaces. Edit those by hand.
