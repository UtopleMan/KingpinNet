# Plan — AI-Friendly Help (`--help-ai`)

Branch: `feature/ai-help`
Spec dir: `specs/2026-05-12-ai-help/`

Task groups are ordered by dependency. Do not start a group until every step in the preceding group is complete (or explicitly stubbed and tracked).

## Group 1 — Model & metadata surface

Foundation for the emitter. Adds the new fields and the author-facing fluent API for global sections, but does not yet emit anything.

1.1 Add `Unit` (`string?`) and `Caution` (`string?`) to `FlagItem` and `ArgumentItem`. Default to null. Wire fluent setters: `.Unit("seconds")`, `.Caution("Requires root.")` on both flag and argument builders.
1.2 Introduce a `GlobalSections` record on `KingpinApplication` holding five lists: `ExitCodes`, `Examples`, `Notes`, `Prefer`, `Avoid`, each of a small strongly-typed record:
   - `ExitCode(int code, string description)`
   - `Example(string intent, string command)`
   - `Note(string text)`
   - `Prefer(string rule, string when, string why)`
   - `Avoid(string rule, string unless, string why)`
1.3 Add fluent extension methods on `KingpinApplication` so apps can declare each section. Pattern: `app.ExitCode(0, "OK").Example("intent", "cmd").Prefer(rule: …, when: …, why: …)`. Keep chainable.
1.4 Add basic xUnit coverage in `test/UnitTests/` for the new fluent setters: setting `Unit` and `Caution` round-trips into the model; each global-section setter appends an entry.

## Group 2 — YAML emitter

The trim/AOT-safe writer. No flag wiring yet; all tests drive it directly with a constructed `KingpinApplication`.

2.1 Add `src/KingpinNet/Help/AiHelpYamlWriter.cs`. `internal sealed class` accepting a `TextWriter`. Single public entry point `Write(KingpinApplication app, CommandItem? scope = null)` — when `scope` is set, only the subtree rooted at that command is emitted.
2.2 Implement YAML primitives: indented scalar emission, list emission, block-scalar (`|`) for help text containing newlines, double-quoted scalar for strings containing `:`, `#`, leading `-`, or other YAML-significant characters. Centralise quoting in one private `WriteScalar(string key, string value)` helper.
2.3 Implement command/flag/argument walkers: `WriteRoot`, `WriteGlobalFlags`, `WriteCommand` (recursive), `WriteFlag`, `WriteArgument`. Emit fields in the order documented in `specs/roadmap.md`'s Format reference. Suppress fields that are null/empty (especially `short`, `unit`, `default`, `help`, `caution`).
2.4 Implement global-section emitters: `WriteExitCodes`, `WriteExamples`, `WriteNotes`, `WritePrefer`, `WriteAvoid`. Each writes its top-level key only if its source list is non-empty.
2.5 Emit the fixed `conventions` block at the root with:
   - `flag_form: --flag=value preferred; bare for booleans`
   - `inheritance: Commands inherit flags from each ancestor in \`inherits\` and from \`global_flags\``
2.6 Unit tests in `test/UnitTests/` covering, at minimum: scalar quoting per edge case (`:`, `#`, leading `-`, embedded newline), block-scalar selection, nested `commands:` indentation, omission of empty optional fields, omission of empty global sections, `scope` parameter restricting output to the subtree.

## Group 3 — `--help-ai` flag

Wire the emitter into the parser.

3.1 Register a built-in global flag `--help-ai` on every `KingpinApplication` and on every `CommandItem` (mirroring how `--help` is registered today; consult existing registration in `KingpinApplication.cs` / `Parser.cs`).
3.2 In the parser, intercept `--help-ai` before normal handler dispatch (same point in the pipeline as the existing `--help` short-circuit). Resolve the scope (root vs. specific subcommand based on which node the flag was attached to).
3.3 Default destination is stdout via the existing `IConsole` abstraction. Construct the writer, call `AiHelpYamlWriter.Write(app, scope)`, exit cleanly (do not invoke a command handler).
3.4 Integration test: a small in-process `KingpinApplication` invoked with `--help-ai` produces non-empty output that begins with `command:` and contains the expected subcommand names.

## Group 4 — File output mode + discovery hint

Adds the second flag and the human-help footer.

4.1 Register `--help-ai-file` as a built-in global flag that optionally takes a path. With no value, the destination is `<exename>-ai-help.md` resolved next to the running executable (use `Environment.ProcessPath` / `AppContext.BaseDirectory`).
4.2 Implement the Markdown wrapper: write `# <exename> help`, blank line, fenced ` ```yaml ` block containing the same YAML payload `--help-ai` emits, closing fence, trailing newline. Reuse `AiHelpYamlWriter` unchanged — the wrapper is purely outside the writer.
4.3 Append the single-line footer to the human-rendered help output (the existing Scriban template surface in `src/KingpinNet/Help/`). Wording: `For agents: run with --help-ai for a machine-readable description.`
4.4 Integration test: invoking the flag with no value creates the expected file in a temp directory; invoking it with an explicit path writes there. Assert the file starts with `# ` and contains a `\`\`\`yaml` fence.

## Group 5 — Test reinforcement

The validation gates that prevent regressions and authored-content drift.

5.1 Add `YamlDotNet` (or an equivalent .NET YAML parser) to the test project only — never to `src/KingpinNet/KingpinNet.csproj`. Verify with `dotnet list test/UnitTests package`.
5.2 Round-trip validity test: for a representative `KingpinApplication` fixture, emit YAML to a string, parse it with `YamlDotNet`, assert no exceptions and that a handful of expected keys (`command`, `commands`, `conventions`) are present.
5.3 Golden/snapshot test(s): one or two representative apps (one trivial, one with nested commands + all global sections) emit YAML compared against checked-in `.expected.yaml` files under `test/UnitTests/`.
5.4 `examples[].command` parse-check: a test iterates every `Example` declared on the fixture app, splits the command line, and feeds it through `Parser` / the existing token + parse pipeline. Asserts it resolves to a valid command + flag set without errors.

## Group 6 — Documentation & example app

User-facing surface.

6.1 Create `Examples/CurlAi/` by copying `Examples/Curl/`. Rename `csproj` to `CurlAi.csproj`. Update assembly name / namespace as appropriate.
6.2 Extend the program to populate every Phase 3 surface: declare at least one `caution` on a destructive flag (e.g. `--force` or `--insecure`), make at least one positional `required: true`, declare a `unit` on every value-taking flag, and call the fluent API for all five global sections.
6.3 Add `Examples/CurlAi/CurlAi.csproj` to `KingpinNet.slnx` so it builds in CI.
6.4 Update `README.md` with a new section titled "AI-friendly help (`--help-ai`)". Cover: what the flag emits, how to invoke it, file mode behavior and default location, the fluent API for the five global sections, and a short (~20-line) YAML excerpt taken from CurlAi's actual output.

## Group 7 — Final validation pass

Runs the merge checklist before the PR is raised. See `validation.md` for the exact commands.

7.1 `dotnet build` clean.
7.2 `dotnet test` clean (all existing + new tests pass).
7.3 AOT publish gate green for a sample project; no new trim warnings introduced.
7.4 Manual smoke: build CurlAi, run `--help-ai` to stdout (eyeball YAML), run `--help-ai-file` (verify `curl-ai-ai-help.md` written next to the binary with the expected wrapper).
7.5 Tick the Phase 3 checkboxes in `specs/roadmap.md` as each is satisfied. Do not tick them speculatively before the validation gates pass.
