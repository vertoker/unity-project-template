### 4. The Unity Editor is read-only through MCP

A `unity-mcp` server exposes the running Editor, and through it you may **only read**: state, console,
types, scenes and assets as data.

**Every write needs permission granted in advance, for that specific action** — including
`refresh_unity`, running tests, opening a scene, Play Mode, menu items and any code execution. A
granted action is reported afterwards, individually, with what actually changed.

Permission for one action is never permission for the next one, and "it is obviously harmless" is not
a grant.

Full text and the MCP procedure: `.claude/rules/unity_editor.md`.
