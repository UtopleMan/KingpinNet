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

3.1 Update `src/KingpinNet/KingpinNet.csproj`: change `TargetFramework` to `TargetFrameworks` with value `net8.0;net9.0`.

3.2 Update `src/KingpinNet.UI/KingpinNet.UI.csproj` to the same dual targets.

3.3 Update test and example projects to run against both TFMs where practical.

3.4 Resolve any API differences between `net8.0` and `net9.0` (conditional compilation with `#if NET9_0_OR_GREATER` only if necessary).

3.5 Build the full solution in Release configuration for both TFMs — zero warnings required before this group is closed.

## Group 4 — Integration Check

4.1 Run the full xUnit suite on both `net8.0` and `net9.0` targets.

4.2 Build all example projects (`Ping`, `Curl`, `Motif`, `Integration`, `Widgets`, `Checque`) against both TFMs.

4.3 Manually exercise at least one example end-to-end to confirm help output and flag parsing are unaffected.

4.4 Review diff for any accidental public API surface changes — confirm none before raising PR.
