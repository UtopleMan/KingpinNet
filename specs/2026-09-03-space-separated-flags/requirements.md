# Requirements — Space-Separated Flag Values (`--flag value`)

Branch: `feature/space-separated-flags`
Roadmap phase: new phase after Phase 5 in `specs/roadmap.md`
Date opened: 2026-09-03

## Goal

Make KingpinNet accept `--flag value` and `-f value` as fully interchangeable with the
existing `--flag=value` / `-f=value` forms, for every value-taking flag type including
`ListOfString`. Booleans deliberately stay bare-or-`=` only, so `--verbose run` keeps
running the `run` command.

## Context

- **Mission alignment.** The mission (`specs/mission.md`) commits to "convention over
  configuration". Space-separated flag values are the dominant convention across GNU
  getopt, POSIX utilities, `git`, `docker`, `dotnet`, and the original Go `kingpin`
  that this library is modelled on. Requiring `=` surprises users and makes shell
  completion and copy-pasted command lines from other tools fail.
- **Tech-stack alignment.** `specs/tech-stack.md` pins `net10.0` and the AOT/trim-safe
  posture established in Phase 2. This change is confined to `src/KingpinNet/Parser.cs`
  and introduces no dependency, no reflection, and no new public API surface.
- **Existing behaviour.** `Parser.IsValidFlag`, `GetValue`, and `GetValues` each split
  the raw token on the first `=` independently. A token with no `=` is treated as a
  bool (or rejected with `--{name} need a value`). Nothing in the parser looks at the
  next token, so `--myflag danish` currently throws `Illegal flag --myflag`.
- **Target framework.** `net10.0`. Test framework xUnit 2.9.3.

## Scope (in)

1. **Long flags.** `--flag value` behaves exactly like `--flag=value` for every
   non-bool `ValueType`: String, Int, Float, Duration, Date, Url, Ip, Tcp, Enum, and
   file/directory-existence flags.
2. **Short flags.** `-f value` behaves exactly like `-f=value` under the same rules.
3. **`ListOfString` flags.** `--items alpha,beta,gamma` produces the same three-element
   list as `--items=alpha,beta,gamma`, including whitespace trimming.
4. **Mixed forms.** Both forms may appear on the same command line, at global scope and
   command scope, in any order.
5. **Consolidated value resolution.** A single helper becomes the one place that decides
   what a flag's raw value is, replacing the duplicated `SplitFirst('=')` logic in
   `IsValidFlag`, `GetValue`, and `GetValues`.
6. **Documentation.** README (Usage, Flags reference, What's New, Changelog), the
   `flag_form` conventions string in `--help-ai` output, and both Scriban help
   templates.

## Decisions

| Question | Decision |
|---|---|
| Bool flags | **Never** consume the next token. `--verbose` = true, `--verbose=false` works, `--verbose false` leaves `false` to the command/argument parser. Preserves `ParserTests.cs:777` (`{"--verbose", "run"}`). |
| Next token starts with `-` | Rejected as a value. `--host --port=1` and a trailing `--host` both fail with `--host need a value`. Dash-leading values require `--offset=-5`. |
| Scope | Long flags, short flags, and `ListOfString` flags all get the space form. |
| Suggestions (`suggest` path) | No intentional behaviour change; must not crash or throw. |
| `--` end-of-flags separator | Out of scope. |
| Implementation site | Inline lookahead in `IsFlag`/`EvaluateItem` (no token pre-pass rewriting). |
| Command shadowing | Accepted and documented: `--host deploy` sets `host="deploy"` even when `deploy` is a declared command (today it throws). Bools are exempt, so `--verbose deploy` still runs `deploy`. |
| Branch | `feature/space-separated-flags` off `master`, merged via PR. |
| Version | No manual bump — `PackageVersion` is pinned at `1.1.9999` in `src/KingpinNet/KingpinNet.csproj` and stamped by CI. |

### Rationale for the two contentious decisions

- **Bools never consume.** Consuming would silently break the existing
  `{"--verbose", "run"}` test and every real command line shaped like
  `app --verbose deploy`. Bool flags have a defined value without an operand, so
  lookahead buys nothing and costs a widely-relied-on behaviour.
- **Dash-leading values rejected.** Without this rule, a mistyped
  `--host --port=1` would silently set `host="--port=1"` instead of reporting a
  missing value. The `=` form remains available for genuinely dash-leading values.

## Out of scope

- The `--` end-of-flags separator.
- Repeated-flag accumulation (`--item a --item b` building a list); last-wins is
  preserved.
- Clustered short flags (`-abc`).
- Changing how positional arguments are parsed.
- Any change to the `suggest` path's produced suggestion list beyond not throwing.

## References

- `plans/space-separated-flag-values.md` — the execution plan this spec accompanies.
- `src/KingpinNet/Parser.cs` — `IsFlag`, `EvaluateItem`, `IsValidFlag`, `GetValue`,
  `GetValues`, `GetFlagName`, `ParseContext`.
- `test/UnitTests/ParserTests.cs` — 53 pre-existing tests; they assert exception *type*
  only, never message text.
- `src/KingpinNet/AiHelp/AiHelpYamlWriter.cs:258` — the `flag_form` conventions string.
- `specs/2026-05-12-ai-help/requirements.md` — folder and document convention mirrored
  here.
