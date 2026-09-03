# Validation — Space-Separated Flag Values (`--flag value`)

Branch: `feature/space-separated-flags`
Spec dir: `specs/2026-09-03-space-separated-flags/`
Validated: 2026-09-03, .NET SDK 10.0.301, target `net10.0`, xUnit 2.9.3

This branch is mergeable only when **every** box below is ticked locally and the same checks pass in CI.

## Pre-flight

- [x] On `feature/space-separated-flags`, branched off `master`.
- [x] Every Phase 6 checkbox in `specs/roadmap.md` is ticked for items actually implemented.
- [ ] `git status` is clean at PR time (no stray scratch files, no unrelated edits).

## Build

```sh
dotnet build KingpinNet.slnx
```

- [x] Zero errors, zero warnings:

  ```
  Build succeeded.
      0 Warning(s)
      0 Error(s)
  ```

  All 7 projects build: `KingpinNet`, `TestKingpinNet`, `Motif`, `CurlAi`, `Curl`, `Ping`, `Tests`.

- [x] No new warnings relative to `master`. `master` could not complete a solution build at all —
  three `NU1605` package-downgrade errors in `Examples/Curl`, `Examples/CurlAi`, and
  `Examples/TestKingpinNet` (each referenced `Microsoft.Extensions.Configuration` 10.0.6 while
  `KingpinNet` requires >= 10.0.9, with `TreatWarningsAsErrors` promoting the downgrade warning
  to an error). Fixed on this branch by bumping those three references to 10.0.9. Clearing that
  unmasked two `CS0122` errors in `Examples/TestKingpinNet/Program.cs` (`runCommand.Path` became
  `protected` in commit `ef73ede`); both call sites now use `runCommand.Name`, which is what
  `ParseResult.Result["Command"]` actually holds. Both fixes are pre-existing breakage, not
  consequences of this feature.

## Tests

```sh
dotnet test test/UnitTests/Tests.csproj
```

- [x] All tests pass:

  ```
  Passed!  - Failed:     0, Passed:   139, Skipped:     0, Total:   139
  ```

  114 pre-existing + 25 added on this branch.

- [x] New parser tests present and meaningful — 21 in `test/UnitTests/ParserTests.cs`:
  - 14 happy-path: global string flag, command-scoped string flag, short flag, Int, Float,
    Duration, Date, Url, Ip, Tcp, Enum, `ListOfString`, mixed forms (`run --a=1 --b 2`),
    repeated-flag last-wins.
  - 4 negative, all `ParseException`: `--flag` alone at end of args, `--host --port=1`,
    `--flag x1x` for an Int flag, `--items` alone for a `ListOfString` flag.
  - 2 bool-exemption: `{"--verbose","run"}` still runs `run`; `{"--bool","true"}` throws because
    `true` is left over as an unexpected argument.
  - 1 command-shadow: `--host deploy` sets `host="deploy"` and leaves `Command` unset.

- [x] New suggestion tests present — 2 in `test/UnitTests/SuggestionTests.cs`:
  - `SuggestFlagWithoutValueDoesNotThrow` — `{"suggest","--myflag"}` yields exactly `["--myflag"]`,
    no exception.
  - `SuggestFlagWithSpaceSeparatedValueDoesNotThrow` — `{"suggest","--myflag","va"}` yields exactly
    `["cmd1"]`, no exception. **New baseline:** `va` is consumed as the flag's value, so `myflag`
    becomes `IsSet` and drops out of the flag suggestions. This input had no defined behaviour
    before; consuming the token is the accepted consequence recorded in the requirements.

- [x] New documentation tests present — 2:
  - `AiHelpYamlTests.ConventionsBlock_FlagForm_DocumentsSpaceSeparatedValues` pins the new
    `flag_form` wording and asserts the old `--flag=value preferred` string is gone.
  - `HelpGeneratorTests.FlagFormLegendIsOmittedWhenEveryFlagIsHidden` pins that the rendered-help
    legend is suppressed when no visible flags exist; `WriteGlobalFlag` now also asserts the
    legend is present when they do.

## Implementation shape

- [x] `src/KingpinNet/Parser.cs` is the only source file whose behaviour changed.
- [x] Value resolution is consolidated in one helper — `grep -n "SplitFirst('=')" src/KingpinNet/Parser.cs`
      returns exactly one hit (line 442, inside `TryResolveFlagValue`), proving no duplicated
      `=`-splitting is left.
- [x] Both forms run the same `IsValidItem` type validation, so `--port abc` fails exactly like
      `--port=abc`.
- [x] `--connstr=Server=localhost;Database=mydb` still parses (only the first `=` splits).

## End-to-end smoke (Curl / CurlAi)

```sh
dotnet run --project Examples/Curl/Curl.csproj -- --timeout=10s get url http://localhost > eq.txt
dotnet run --project Examples/Curl/Curl.csproj -- --timeout  10s get url http://localhost > sp.txt
diff eq.txt sp.txt
```

- [x] Byte-identical output for the global flag `--timeout` in both forms (`diff` clean), both
      logging `Found/updated global flag timeout with value 10s`.
- [x] Byte-identical output for the command-scoped flag `--data` (`--data=key:value` vs
      `--data key:value` on `post http://localhost`), both logging
      `Found/updated post:data with value key:value`.
- [x] `dotnet run --project Examples/CurlAi/CurlAi.csproj -- --help-ai` still emits the full YAML,
      and line 135 now reads:

      ```yaml
      conventions:
        flag_form: "--flag=value and --flag value are interchangeable for value-taking flags (same for -f=value and -f value); booleans are bare or --flag=false and never consume the next token; values starting with - require the = form"
      ```

- [x] `dotnet run --project Examples/Curl/Curl.csproj -- --help` renders the flag-form legend under
      the `Flags:` list:

      ```
      Flags:
        -h,  --help                           Show context-sensitive help
        -t,  --timeout=<5s>                   Set connection timeout.
        -H,  --headers=<value>                Add HTTP headers to the request.

        Value flags accept --flag=value or --flag value (short: -f=value or -f value). Booleans are bare or --flag=false.
      ```

- [x] `dotnet run --project Examples/Curl/Curl.csproj -- get url --help` renders the command
      template without the legend, because that command has no visible flags.

**Known, out of scope:** `Examples/Curl` still ends with `Unhandled exception: Didn't understand
commandline` for **both** flag forms. `KingpinCommandLineConfigurationProvider.Load` assigns
`ConfigurationProvider.Data = Kingpin.Parse(args).Result`, replacing the base class's
case-insensitive dictionary with the parser's ordinal one, so the example's
`configuration["command"]` lookup misses the `"Command"` key. Pre-existing and unrelated to flag
parsing, which the log lines confirm is correct.

## Documentation

- [x] `grep -rn "flag=value preferred" src/ README.md` returns no hits — the old wording is fully
      removed from shipping code and the README. `specs/2026-05-12-ai-help/plan.md` deliberately
      keeps its original wording as a historical record of that phase.
- [x] `README.md` `## Usage` has a new `### Flag syntax` subsection covering both forms, the
      boolean exemption, the dash-token rule, and the command-shadow consequence.
- [x] `README.md` `### Flags` reference subsection states the same rules in one paragraph.
- [x] `README.md` `## What's New` has a `### Space-separated flag values` section above
      `### Phase 3 — AI-friendly help`.
- [x] `README.md` `## Changelog` has a new top `1.2` bullet.
- [x] `README.md` AI-help sample YAML excerpt updated to the actual emitted `flag_form` line.
- [x] `src/KingpinNet/Help/ApplicationHelp.liquid` and `src/KingpinNet/Help/CommandHelp.liquid`
      both render the same legend, gated on a visible-flag count so all-hidden flag sets stay
      clean. Both files are embedded as resources into `test/UnitTests/Tests.csproj`.
- [x] `specs/roadmap.md` has a new `## Phase 6 — Space-Separated Flag Values` after Phase 5 (its
      heading ✓ is added at merge time, per the roadmap's own convention), and
      its Phase 3 Format reference `flag_form` line matches the new emitted string.

## Final gate

- [x] Build and test gates above are green.
- [x] No `TODO`, `FIXME`, or `XXX` left in new code on this branch.
- [ ] Commit history squashed/legible before the PR is raised.

Only when this entire document is green: open the PR against `master`.
