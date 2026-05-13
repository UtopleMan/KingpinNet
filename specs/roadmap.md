# Roadmap

Items are ordered by dependency — earlier items unblock later ones.

## Phase 1 — Stabilization ✓

**Goal:** Clean internal foundations before adding new capabilities.

- [x] **Technical debt** — Resolve inconsistencies in `BaseItem.cs` and `Parser.cs`
- [x] Audit and tighten test coverage of parser edge cases
- [x] Align library target framework with `net10.0`

## Phase 2 — Modern .NET Compatibility ✓

**Goal:** Make KingpinNet a first-class citizen in modern .NET publish scenarios.

- [x] **AOT / trimming support** — Evaluate replacing DotLiquid with a trimming-safe templating approach (Scriban or compile-time help generation); validate with `dotnet publish -p:PublishAot=true`
- [x] Ensure single-file publish works without warnings
- [x] Update CI to include AOT publish as a build gate

## Phase 3 — AI-Friendly Help (`--help-ai`) ✓

**Goal:** Emit a machine-readable, YAML-formatted help output optimised for LLM agents that need to construct correct invocations without parsing the human-rendered help.

- [x] **`--help-ai` flag** — Built-in global flag, parallel to `--help`. Available at the root and on every subcommand; scopes output to the invoked node's subtree. Default destination is stdout.
- [x] **File output mode** — `--help-ai-file[=<path>]` (no value = default location) writes the help to `<exename>-ai-help.md` next to the executable. The file is a thin Markdown wrapper so it doubles as checked-in documentation that GitHub/IDE renderers display cleanly while staying trivially parseable for an LLM. Format:

    ````markdown
    # <exename> help

    ```yaml
    <full YAML payload as defined in the Format reference below>
    ```
    ````
- [x] **Discovery hint in `--help`** — Append a single line to the human-rendered help (e.g. `For agents: run with --help-ai for a machine-readable description.`) so an LLM that reads `--help` first finds the AI-friendly variant without external docs.
- [x] **YAML emitter** — Trim/AOT-safe. Hand-rolled writer that walks `CommandModel` / `FlagModel` / `ArgumentModel`; no reflection-based serializer. Correct quoting for help strings containing `:`, `#`, leading `-`, or newlines (use block scalars where needed).
- [x] **Command/flag schema** — For each command emit `command`/`path`, `summary`, `synopsis`, nested `commands`, and for each flag/argument: `long`, `short`, `type`, `takes_value`, `unit`, `default`, `required`, `help`, plus optional `caution` (free-form text; surface root/privilege requirements here). `unit` is a short noun naming what the value represents (e.g. `seconds`, `milliseconds`, `hours`, `count`, `packets`, `messages`, `events`, `bytes`) — replaces the syntactic placeholder so the LLM can reason about units instead of guessing from a `<…>` token.
- [x] **Global sections** — First-class fluent API on `KingpinApplication` to declare the root-level sections: `ExitCodes`, `Examples`, `Notes`, `Prefer`, `Avoid`. Emitted under the matching YAML keys.
- [x] **`conventions` block** — Emit a small fixed `conventions` block (flag form, inheritance rule) so the consuming LLM knows the schema's rules without inferring them.
- [x] **Unit tests for the YAML emitter** — Per-feature tests covering: quoting/escaping of help strings containing `:`, `#`, leading `-`, or newlines (block scalars where required); correct emission of `long`, `short`, `type`, `takes_value`, `unit`, `default`, `required`, `help`, `caution` on flags and arguments; nested `commands:` indentation and ordering; each global section (`exit_codes`, `examples`, `notes`, `prefer`, `avoid`, `conventions`) emitted only when populated; and a round-trip assertion that the produced output parses cleanly through a YAML parser (i.e. always-valid YAML, not just visually correct text).
- [x] **Tests & golden output** — Snapshot tests over a representative app covering nested subcommands, inherited flags, and all global sections.
- [x] **Example parse-check** — CI feeds each `examples[].command` back through the parser to confirm it resolves to a valid command + flag combination, so authored examples can't silently drift from the real CLI.
- [x] **README update** — Add a section to `README.md` introducing `--help-ai` and `--help-ai-file`: what they emit (machine-readable YAML for LLM agents), how to invoke each, the default file location, the fluent API for the global sections (`ExitCodes`, `Examples`, `Notes`, `Prefer`, `Avoid`), and a short YAML excerpt so readers can see the shape without leaving the README.
- [x] **`curl-ai` example project** — Add a new `Examples/CurlAi/` project alongside the existing `Examples/Curl/`. Start from `Curl/Program.cs` and exercise every new Phase 3 addition: opt into `--help-ai` / `--help-ai-file`, populate the fluent API for `ExitCodes`, `Examples`, `Notes`, `Prefer`, `Avoid`, declare at least one `caution` on a destructive flag, set `required: true` on at least one positional argument, and declare `unit` on every value-taking flag. Add it to `KingpinNet.slnx` so it builds in CI.

### Format reference

```yaml
command: execute
summary: Send ICMP ECHO_REQUEST packets to a host
synopsis: execute [commands] <arguments> [flags]

global_flags:
  - long: --interval
    short: -i
    type: float
    takes_value: true
    unit: seconds
    default: 1.0
    required: false
  - long: --timeout
    short: -t
    type: int
    takes_value: true
    unit: seconds
    required: false
    help: Exit after the given number of seconds regardless of packets received

commands:
  - path: [run]
    summary: Run an execute session
    synopsis: execute run <subcommand> [flags]
    flags:
      - long: --interval
        short: -i
        type: float
        takes_value: true
        unit: seconds
        default: 1.0
        required: false
        help: Seconds between packets
    commands:
      - path: [now]
        summary: Send packets immediately without scheduling
        synopsis: execute run now <argument> [flags]
        argument:
          - type: int
            takes_value: true
            unit: packets
            required: true
            help: Number of packets to send
        flags:
          - long: --count
            short: -c
            type: int
            takes_value: true
            unit: packets
            required: false
            help: Stop after sending the given number of packets

exit_codes:
  0: At least one reply received
  1: No replies received
  2: Error (DNS failure, invalid argument, permission denied)

examples:
  - intent: Send exactly 5 packets then stop
    command: execute --count=5 google.com

notes:
  - Flag semantics differ across platforms; this schema reflects BSD/macOS.
  - On Linux, --timeout maps to -W (per-packet) vs -w (deadline).
  - Requires raw-socket privileges (root or CAP_NET_RAW).

prefer:
  - rule: Use --count to bound runs
    when: Any non-interactive or scripted invocation
    why: Without --count, execute runs until interrupted; agents hang.

avoid:
  - rule: Don't use --flood
    unless: User explicitly asked for stress/load testing
    why: Requires root, can saturate links, may trigger IDS.

conventions:
  flag_form: --flag=value preferred; bare for booleans
  inheritance: Commands inherit flags from each ancestor in `inherits` and from `global_flags`
```

## Phase 4 — Async Execution

**Goal:** First-class `async`/`await` for command handlers.

- [ ] **Async command handlers** — Introduce `RunAsync(Func<ParseResult, CancellationToken, Task>)` or equivalent on `KingpinApplication`
- [ ] Propagate `CancellationToken` from top-level entry point through to handlers
- [ ] Preserve sync API for backwards compatibility

## Phase 5 — Source Generators & DI

**Goal:** Attribute-driven CLI definition with dependency injection support.

- [ ] **Source generators** — Design and implement `[Command]`, `[Flag]`, `[Argument]` attributes with a Roslyn source generator that emits the fluent-API registration at compile time
- [ ] **DI integration** — Register command handlers as `IHostedService`-compatible services; integrate with `Microsoft.Extensions.DependencyInjection`
- [ ] Provide a `UseKingpin()` extension on `IHostBuilder` / `IHostApplicationBuilder`
- [ ] Example project demonstrating full Generic Host + KingpinNet setup

## Unprioritized / Backlog

- Richer UI widgets (interactive prompts, selection menus, color themes)
- Structured output modes (JSON, plain) as a first-class flag convention
- .NET interactive / REPL-style command loop mode
