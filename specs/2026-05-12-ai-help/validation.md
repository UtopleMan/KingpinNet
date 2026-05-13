# Validation — AI-Friendly Help (`--help-ai`)

Branch: `feature/ai-help`
Spec dir: `specs/2026-05-12-ai-help/`

This branch is mergeable only when **every** box below is ticked locally and the same checks pass in CI. Do not raise the PR until the entire checklist is green.

## Pre-flight

- [ ] On `feature/ai-help`, rebased onto current `master` with no conflicts.
- [ ] `git status` is clean (no stray scratch files, no unrelated edits).
- [ ] Every Phase 3 checkbox in `specs/roadmap.md` is ticked for items now actually implemented (and untouched for any explicitly deferred).

## Build

Solution must build clean on `net10.0`.

```sh
dotnet restore
dotnet build -c Release --nologo
```

- [ ] Zero errors.
- [ ] Zero new warnings attributable to the changes on this branch (compare against `master`).

## Tests

Full xUnit suite, existing + new.

```sh
dotnet test -c Release --nologo
```

- [ ] All tests pass.
- [ ] New tests are present and meaningful for: scalar quoting (each of `:`, `#`, leading `-`, embedded newline), block-scalar selection, nested `commands:` indentation, optional-field suppression, global-section suppression when empty, scope parameter, YAML round-trip validity (via `YamlDotNet` test-only dependency), golden snapshot of a representative app, and the `examples[].command` parse-check.
- [ ] `YamlDotNet` (or equivalent) appears **only** in the test project. Verify:
  ```sh
  dotnet list src/KingpinNet/KingpinNet.csproj package | grep -i yaml || echo "OK: no YAML package in runtime project"
  dotnet list test/UnitTests package | grep -i yaml
  ```

## AOT publish gate

The Phase 2 gate must remain green — the emitter must not reintroduce reflection-based serialisation.

```sh
dotnet publish Examples/CurlAi/CurlAi.csproj -c Release -r osx-arm64 -p:PublishAot=true --nologo
```

- [ ] Publish succeeds.
- [ ] No new AOT / trimming warnings emitted (`IL2xxx`, `IL3xxx`) attributable to the new emitter code.
- [ ] Resulting binary runs `--help-ai` successfully (see smoke below).

## End-to-end smoke (CurlAi)

Run the example binary that exercises every Phase 3 surface.

```sh
dotnet build Examples/CurlAi/CurlAi.csproj -c Release
EXE="$(find Examples/CurlAi/bin/Release -name 'CurlAi' -o -name 'CurlAi.exe' | head -n1)"
"$EXE" --help-ai | head -80
"$EXE" --help-ai-file
ls -1 "$(dirname "$EXE")"/curl-ai-ai-help.md
```

- [ ] `--help-ai` prints YAML beginning with `command:` and ending with the `conventions:` block. Output is non-trivial (more than a dozen lines).
- [ ] Output includes at least one populated `exit_codes`, `examples`, `notes`, `prefer`, and `avoid` section.
- [ ] At least one flag/argument shows a populated `unit:` value, and at least one shows `caution:`.
- [ ] At least one positional argument shows `required: true`.
- [ ] `--help-ai-file` writes `curl-ai-ai-help.md` next to the binary; file begins with `# curl-ai help` followed by a blank line and a ` ```yaml ` fence.
- [ ] Running `--help-ai-file=/tmp/custom-name.md` writes to the explicit path instead.
- [ ] `--help` output now contains the footer line: `For agents: run with --help-ai for a machine-readable description.`

## Documentation

- [ ] `README.md` has a new section titled "AI-friendly help (`--help-ai`)" that introduces both flags, documents the fluent API for `ExitCodes`/`Examples`/`Notes`/`Prefer`/`Avoid`, states the default file location, and embeds a short YAML excerpt taken from the actual CurlAi output (not a hand-written sample).
- [ ] `specs/roadmap.md` Phase 3 checkboxes are updated to reflect the actual delivered scope.

## Final gate

- [ ] All checkboxes above are ticked.
- [ ] No `TODO`, `FIXME`, or `XXX` left in new code on this branch.
- [ ] Commit history is reasonable (squashed or otherwise legible; no `wip` / `fix typo` noise).

Only when this entire document is green: open the PR against `master`.
