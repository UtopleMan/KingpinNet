# Validation — Phase 1: Stabilization

This branch is ready to merge when all of the following are true.

## 1. All existing tests pass

- `dotnet test` exits 0 with no failures and no skipped tests on both `net8.0` and `net9.0` targets
- Test run must include the full `UnitTests` project — no selective filtering

## 2. Both TFMs build clean

- `dotnet build -c Release` produces zero warnings for `KingpinNet` and `KingpinNet.UI` on both `net8.0` and `net9.0`
- Zero warnings is a hard gate — treat warnings as errors (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`) to confirm

## 3. No public API breaking changes

- Diff `KingpinApplication`, `Kingpin`, `FlagItem<T>`, `ArgumentItem<T>`, `CommandItem`, and `Parser` public members before and after
- Any removal or signature change on a `public` or `protected` member is a blocker unless explicitly approved

## 4. Examples build and run

- All projects under `Examples/` compile against both TFMs
- At least one example (`Ping` or `Curl`) runs end-to-end and produces correct help output and parses a sample invocation correctly

## How to run the checks locally

```bash
# Build both TFMs
dotnet build src/KingpinNet/KingpinNet.csproj -c Release

# Run tests on both TFMs
dotnet test test/UnitTests/UnitTests.csproj -f net8.0
dotnet test test/UnitTests/UnitTests.csproj -f net9.0

# Smoke-test an example
dotnet run --project Examples/Ping -- --help
```

## Merge checklist

- [ ] `dotnet test` green on `net8.0`
- [ ] `dotnet test` green on `net9.0`
- [ ] Zero build warnings on both TFMs
- [ ] No public API removals or signature changes
- [ ] At least one example exercised manually
- [ ] Roadmap Phase 1 checkboxes updated in `specs/roadmap.md`
