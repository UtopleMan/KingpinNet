# Requirements — AI-Friendly Help (`--help-ai`)

Branch: `feature/ai-help`
Roadmap phase: Phase 3 in `specs/roadmap.md`
Date opened: 2026-05-12

## Goal

Emit a machine-readable, YAML-formatted description of a KingpinNet application's command tree so that LLM-driven agents can construct correct invocations without parsing the human-rendered help. The work spans the data model, a new YAML emitter, two new global flags (`--help-ai`, `--help-ai-file`), an author-facing fluent API for global metadata sections, tests, documentation, and a worked example.

## Context

- **Mission alignment.** The mission (`specs/mission.md`) commits to "convention over configuration" and "consistent help formatting … out of the box." `--help-ai` extends that promise to a second audience — LLM agents — without authors having to hand-roll machine-readable docs.
- **Tech-stack alignment.** `specs/tech-stack.md` notes that Phase 2 closed the AOT/trimming gap by replacing DotLiquid with Scriban. The new emitter must not reopen that gap. The model classes (`CommandItem`, `FlagItem`, `ArgumentItem` in `src/KingpinNet/`) already carry most flag metadata; this phase extends them with the few fields the AI schema needs that aren't already present.
- **Target framework.** `net10.0`. Test framework xUnit 2.8.0.

## Scope (in)

All four scope groups confirmed in the kickoff Q&A:

1. **Core emitter + `--help-ai` flag.**
   - Hand-rolled `AiHelpYamlWriter` (internal class) that walks `KingpinApplication` / `CommandItem` / `FlagItem` / `ArgumentItem` and writes YAML to a `TextWriter`.
   - Built-in global `--help-ai` flag, parallel to `--help`. Available at the root and on every subcommand; scopes the output to the invoked node's subtree. Default destination: stdout.
   - Schema fields per flag/argument: `long`, `short`, `type`, `takes_value`, `unit`, `default`, `required`, `help`, optional `caution`.
   - `unit` is a short noun (`seconds`, `milliseconds`, `count`, `bytes`, `messages`, …) replacing the syntactic placeholder so the LLM reasons about units rather than `<n>` tokens.
   - Fixed `conventions` block emitted at the root: flag form and inheritance rule.

2. **Global sections fluent API.**
   - New fluent extension surface on `KingpinApplication` for: `ExitCodes`, `Examples`, `Notes`, `Prefer`, `Avoid`. Each emitted only when populated.
   - Strongly-typed shapes:
     - `ExitCode(int code, string description)`
     - `Example(string intent, string command)`
     - `Note(string text)`
     - `Prefer(string rule, string when, string why)`
     - `Avoid(string rule, string unless, string why)`

3. **File output + discovery hint.**
   - `--help-ai-file[=<path>]` global flag. With no value, writes to `<exename>-ai-help.md` next to the executable. With a value, writes to that path.
   - Output is a thin Markdown wrapper:
     ````markdown
     # <exename> help

     ```yaml
     <YAML payload>
     ```
     ````
   - Single-line footer appended to the human `--help` output: `For agents: run with --help-ai for a machine-readable description.`

4. **Tests, README, and `CurlAi` example.**
   - Unit tests for the emitter (per-feature).
   - Golden/snapshot tests over a representative app covering nested subcommands, inherited flags, and all global sections.
   - Round-trip validity assertion: emitter output parsed by a third-party YAML parser in tests (test-only NuGet reference, e.g. `YamlDotNet`).
   - CI parse-check that re-feeds each `examples[].command` through the parser.
   - `README.md` section introducing both flags, the fluent API, and a short YAML excerpt.
   - `Examples/CurlAi/` project cloned from `Examples/Curl/`, wired into `KingpinNet.slnx`, exercising every Phase 3 surface (both flags, all five global sections, at least one `caution`, at least one `required: true` positional, `unit` on every value-taking flag).

## Decisions

- **Emitter is hand-rolled, not library-backed.** Confirmed in kickoff. A focused `AiHelpYamlWriter` walks the model and writes YAML primitives directly to a `TextWriter`. Rationale: zero new runtime dependencies, trim/AOT-safe by construction (no reflection), and the YAML subset we need is small enough that hand-rolled quoting/block-scalar logic is straightforward to unit-test. Rejected alternatives: YamlDotNet (reflection-heavy, would jeopardise the Phase 2 AOT gate), Scriban template (YAML quoting rules don't fit a template engine cleanly — every edge case leaks into the template).
- **`unit` replaces the angle-bracket placeholder.** A field like `unit: seconds` is more directly machine-actionable than `placeholder: <seconds>`.
- **`caution` is free-form text.** Root/privilege requirements are surfaced via `caution` rather than a separate `requires_root` boolean (decided pre-spec).
- **Global sections are root-only.** `exit_codes`, `examples`, `notes`, `prefer`, `avoid`, and `conventions` appear only at the top of the YAML document, not per-subcommand. Per-command versions can be added in a follow-up phase if demand emerges.
- **Discovery via `--help` footer only.** No manifest file; rely on the LLM reading `--help` first and following the footer hint. (Decided pre-spec.)
- **Examples are parse-checked, not executed.** CI re-parses every `examples[].command` to confirm it resolves to a valid command + flag combination; nothing is actually run.
- **YamlDotNet is permitted as a *test-only* dependency.** Used solely to assert round-trip validity of the emitter's output. It must not appear in the runtime `KingpinNet.csproj`.

## Out of scope

- Flag relationships (`requires`, `conflicts`, `implies`, mutex groups). Deferred to a later phase; LLMs will continue to occasionally produce invalid flag combos until then.
- Sibling emission targets: Anthropic / OpenAI tool-use JSON schema, MCP server stub, AGENTS.md generator. Deferred until the YAML schema settles.
- Per-subcommand `examples` / `prefer` / `avoid` sections (root-only for this phase).
- Localisation of help text.
- Executing examples end-to-end (parse-check only).
- Changes to the human-rendered `--help` output beyond the single footer line.

## References

- `specs/mission.md` — convention-driven help, .NET ecosystem fit.
- `specs/tech-stack.md` — `net10.0`, xUnit 2.8.0, AOT/trim constraint that motivated the hand-rolled emitter.
- `specs/roadmap.md` — Phase 3 bullets and Format reference (canonical YAML shape).
- `Examples/Curl/Program.cs` — starting point for the `CurlAi` example.
- `src/KingpinNet/KingpinApplication.cs`, `CommandItem.cs`, `FlagItem.cs`, `ArgumentItem.cs` — model classes the emitter walks.
- `src/KingpinNet/Help/` — existing help-rendering surface the new emitter sits alongside.
