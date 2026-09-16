### 4. The Unity Editor: read and test freely, mutate only with permission

A `unity-mcp` server exposes the running Editor. Reading state, console, assets and types, plus
`refresh_unity` and running tests, need no permission. Small asset edits follow rule 1 and are
reported every time. Play Mode, scene mutations, menu items, deletions and arbitrary code in the
Editor are the author's to grant.

**Never leave the Editor locked.** Not in Play Mode, not mid-import, not on a scene the author did
not open. A session that ends that way has not finished.

Full text and the MCP procedure: `.claude/rules/unity_editor.md`.
