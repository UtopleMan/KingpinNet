# Plan — Phase 1: Stabilization

## Group 1 — BaseItem & Parser Cleanup

1.1 Read and understand the current state of `src/KingpinNet/BaseItem.cs` and `src/KingpinNet/Parser.cs` — document every inconsistency found (naming, responsibility leakage, duplicated logic).

1.2 Identify which inconsistencies are self-contained fixes vs. which require touching the surrounding call graph. Produce a list before touching any code.

1.3 Refactor `BaseItem.cs` — align with the internal model that async execution and source-generator registration will need (clean separation of definition vs. runtime state).

1.4 Refactor `Parser.cs` — ensure tokenization and validation are clearly separated; eliminate any stateful side-effects that would block async or parallel parse paths later.

1.5 Verify all existing tests still pass after each step. Do not batch steps 1.3 and 1.4 into a single commit.

## Group 2 — Test Coverage Audit

2.1 Run the existing xUnit suite and record baseline pass/fail counts and any skipped tests.

2.2 Review `Parser.cs` paths against existing tests — identify untested branches (edge cases: special-char flag values, nested sub-commands, duplicate flags, missing required args, empty input).

2.3 Write new xUnit tests for each uncovered edge case identified in 2.2.

2.4 Confirm no test relies on implementation details exposed by the debt cleaned up in Group 1 — update any brittle tests.

## Group 3 — Framework Retargeting

3.1 Update `src/KingpinNet/KingpinNet.csproj`: set `TargetFrameworks` to `net10.0`.

3.2 Update `src/KingpinNet.UI/KingpinNet.UI.csproj` to the same target.

3.3 Update test and example projects to target `net10.0`.

3.4 Build the full solution in Release configuration — zero warnings required before this group is closed.

## Group 4 — Solution File Migration

4.1 Convert `KingpinNet.sln` to `KingpinNet.slnx` format using `dotnet sln` or the Visual Studio migration tool.

4.2 Verify all projects are correctly referenced in the new `.slnx` file.

4.3 Confirm `dotnet build KingpinNet.slnx` and `dotnet test` resolve correctly from the new solution file.

4.4 Delete the old `KingpinNet.sln` once the `.slnx` file is validated.

## Group 5 — Package Updates

5.1 Audit all NuGet package references across the solution for outdated versions.

5.2 Update all packages to their latest stable versions compatible with `net10.0`.

5.3 Build and run the full test suite to confirm no regressions from updated packages.

## Group 6 — Integration Check

6.1 Run the full xUnit suite on `net10.0`.

6.2 Build all example projects (`Ping`, `Curl`, `Motif`, `Integration`, `Widgets`, `Checque`) against `net10.0`.

6.3 Manually exercise at least one example end-to-end to confirm help output and flag parsing are unaffected.

6.4 Review diff for any accidental public API surface changes — confirm none before raising PR.
