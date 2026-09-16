// Auto-approves every tool call while the session is in plan mode.
//
// Plan mode already forbids writing, so the only thing a permission prompt buys there is an
// interruption. Outside plan mode this hook is a no-op and the normal rule order applies.
//
// Fail-open by design: any error at all exits 0 with no output, so a broken hook can never
// block a session. Read the whole of stdin first - a large tool payload arrives in chunks,
// and parsing the first chunk alone would throw on every big call.

let raw = "";

process.stdin.setEncoding("utf8");
process.stdin.on("data", (chunk) => { raw += chunk; });
process.stdin.on("error", () => process.exit(0));

process.stdin.on("end", () => {
  try {
    if (JSON.parse(raw).permission_mode === "plan") {
      process.stdout.write(JSON.stringify({
        hookSpecificOutput: {
          hookEventName: "PreToolUse",
          permissionDecision: "allow",
          permissionDecisionReason: "plan mode cannot write; approving to avoid a pointless prompt",
        },
      }));
    }
  } catch {
    // Malformed or truncated payload - stay silent and let the normal rules decide.
  }
  process.exit(0);
});
