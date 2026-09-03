# Plan — Space-Separated Flag Values (`--flag value`)

Branch: `feature/space-separated-flags`
Spec dir: `specs/2026-09-03-space-separated-flags/`
Working plan (with per-phase summaries and verification results): `plans/space-separated-flag-values.md`

Task groups are ordered by dependency. Do not start a group until every step in the preceding group is complete.

## Group 1 — Branch, spec folder, and failing tests

Pin the intended behaviour in tests before touching the parser.

1.1 Create branch `feature/space-separated-flags` off `master`.
1.2 Create `specs/2026-09-03-space-separated-flags/requirements.md` mirroring `specs/2026-05-12-ai-help/requirements.md` (Goal, Context, Scope in, Scope out, Decisions table).
1.3 Add happy-path tests to `test/UnitTests/ParserTests.cs`, one per case, each asserting the parsed value in `ParseResult.Result`: global string flag, command-scoped string flag, short flag, and one per value type (Int, Float, Duration, Date, Url, Ip, Tcp, Enum), plus `ListOfString`, mixed forms in one command line (`run --a=1 --b 2`), and repeated-flag last-wins.
1.4 Add negative tests: `--flag` alone at end of args, `--host --port=1` (dash token is not a value), `--flag x1x` for an Int flag, and `--items` alone for a `ListOfString` flag. All throw `ParseException`.
1.5 Add bool-exemption tests: `{"--verbose", "run"}` still runs command `run`; `{"--bool", "true"}` sets the bool and leaves `true` to be rejected as an unexpected argument.
1.6 Add the documented command-shadow test: `--host deploy` sets `host="deploy"` and does not run the declared `deploy` command.

## Group 2 — Parser implementation

`src/KingpinNet/Parser.cs` is the only source file that changes behaviour.

2.1 Add `ExtraTokensConsumed` to `Parser.ParseContext`.
2.2 Add `TryResolveFlagValue(IItem flag, string arg, string nextToken, out string rawValue, out bool consumesNext, out string error)` as the single source of truth for value resolution, resolving in order: the `=` form (via the one surviving `SplitFirst('=')`, so `--connstr=Server=localhost;Database=mydb` still works), then the bool short-circuit to `"true"`, then rejection when `nextToken` is null or starts with `-` (error `--<name> need a value`), otherwise the next token becomes the value. Finish by running the existing `IsValidItem` so every type check applies identically to both forms.
2.3 Rewrite `IsValidFlag` as a thin wrapper over `TryResolveFlagValue`, taking the new `nextToken` parameter.
2.4 Change `IsFlag` to compute `nextToken`, reset `ExtraTokensConsumed` on entry, and pass `nextToken` down both the `--` and `-` branches.
2.5 Change `EvaluateItem` to call `TryResolveFlagValue`, set `StringValues` (comma-split + trim) for `ListOfString` or `StringValue` otherwise, and record `ExtraTokensConsumed`.
2.6 Delete `GetValue`/`GetValues`; leave no duplicated `=`-splitting in the file.
2.7 Advance both call sites (`MainParse`, `CommandFound`) with `ctx.CurrentItem += 1 + ctx.ExtraTokensConsumed;`.
2.8 Preserve existing failure behaviour: `ParseException("Illegal flag " + arg, errors)` with detail strings in `Errors`, and `return false` instead of throwing when `ctx.Result.IsSuggestion`.

## Group 3 — Suggestion path and example smoke test

3.1 Add tests to `test/UnitTests/SuggestionTests.cs` proving the suggest path never throws with the space form, both for a flag with no value and for a flag followed by a partial token. Record whatever list the latter produces as the new baseline.
3.2 Run one real value-taking flag through `Examples/Curl` in both forms and confirm identical output.
3.3 Run `Examples/CurlAi --help-ai` and confirm the YAML still emits.

## Group 4 — Documentation

4.1 `src/KingpinNet/AiHelp/AiHelpYamlWriter.cs` — rewrite the `conventions.flag_form` scalar to state that both forms are interchangeable for value-taking flags, that booleans are bare or `--flag=false`, and that dash-leading values require the `=` form. Pin the new wording in `test/UnitTests/AiHelpYamlTests.cs`.
4.2 `README.md` — new `### Flag syntax` subsection under `## Usage` covering both forms, the boolean exemption, the dash-token rule, and the command-shadow consequence.
4.3 `README.md` `### Flags` reference subsection — the same rules in one paragraph.
4.4 `README.md` `## What's New` — new section above `### Phase 3 — AI-friendly help`, matching that entry's style.
4.5 `README.md` `## Changelog` — new top bullet.
4.6 `src/KingpinNet/Help/ApplicationHelp.liquid` and `src/KingpinNet/Help/CommandHelp.liquid` — render a flag-form legend under the `Flags:` list in both templates. The existing `--flag=<value>` column rendering stays as-is so the aligned-column assertions in `HelpGeneratorTests` keep passing. Both files are also embedded into `test/UnitTests/Tests.csproj` as resources.
4.7 `specs/roadmap.md` — add Phase 6 after Phase 5 with a checklist mirroring these groups, and update the `flag_form` line in the Phase 3 Format reference to the new wording.
4.8 `specs/2026-09-03-space-separated-flags/plan.md` and `validation.md` — mirror the `specs/2026-05-12-ai-help/` structure; `validation.md` records the actual verification command output.

## Group 5 — Final validation pass

Runs the merge checklist before the PR is raised. See `validation.md` for the exact commands.

5.1 `dotnet build KingpinNet.slnx` clean.
5.2 `dotnet test test/UnitTests/Tests.csproj` clean (all pre-existing + new tests pass).
5.3 Old `flag=value preferred` wording fully removed from `src/` and `README.md` (the historical `specs/2026-05-12-ai-help/plan.md` keeps its original wording as a record of that phase).
5.4 Tick the Phase 6 checkboxes in `specs/roadmap.md` only once the gates above pass.
