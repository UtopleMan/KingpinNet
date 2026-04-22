# Plan — Phase 2: Modern .NET Compatibility

## Group 1 — Audit & Snapshot

Establish a baseline before touching any templating code.

- **1.1** Locate all DotLiquid usages across the solution (`grep -r "DotLiquid" --include="*.cs" --include="*.csproj"`).
- **1.2** Identify every Liquid template file (`.liquid` or embedded string) used for help rendering.
- **1.3** Capture golden-file snapshots of current help output for all existing command/flag/argument combinations (used in step 3.3 to verify parity).
- **1.4** Run `dotnet publish -p:PublishAot=true` on the main library and record all warnings/errors as the baseline to fix.

## Group 2 — Scriban Integration

Replace DotLiquid with Scriban.

- **2.1** Add `Scriban` NuGet package to `KingpinNet.csproj`; remove `DotLiquid`.
- **2.2** Create Scriban-compatible model types (POCOs or `IScriptObject` wrappers) that expose the same data previously passed to DotLiquid templates.
- **2.3** Migrate each Liquid template to Scriban syntax (primarily `{{ }}` interpolation; adjust any DotLiquid-specific filters).
- **2.4** Replace the DotLiquid rendering call-sites with `Scriban.Template.Parse(...).Render(model)`.
- **2.5** Annotate model types with `[ScriptObject]` or equivalent source-generator attributes to enable trim-safe rendering.

## Group 3 — AOT & Single-File Validation

Confirm the library publishes cleanly.

- **3.1** Add `<IsAotCompatible>true</IsAotCompatible>` and `<EnableTrimAnalyzer>true</EnableTrimAnalyzer>` to `KingpinNet.csproj`.
- **3.2** Run `dotnet publish -p:PublishAot=true`; resolve all remaining trim/AOT warnings to zero.
- **3.3** Run `dotnet publish -p:PublishSingleFile=true`; confirm no warnings.
- **3.4** Re-run the golden-file snapshot tests from step 1.3 to confirm help output is byte-for-byte identical.

## Group 4 — CI Gate

Prevent regressions in CI.

- **4.1** Add an AOT publish job/step to the CI pipeline (`.github/workflows/` or equivalent) that runs `dotnet publish -p:PublishAot=true` and fails the build on any warning or error.
- **4.2** Add a single-file publish step alongside it.

## Group 5 — Documentation

- **5.1** Update `README.md`: add a "What's New" section with high-level bullet summaries for Phase 1 (stabilization) and Phase 2 (AOT/trimming/single-file support, Scriban).
