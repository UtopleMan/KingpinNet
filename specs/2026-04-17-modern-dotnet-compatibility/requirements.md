# Requirements — Phase 2: Modern .NET Compatibility

## Context

KingpinNet (see `specs/mission.md`) aims to be a first-class .NET CLI library. As of Phase 1 the internals are clean and the target framework is `net10.0`. Phase 2 makes KingpinNet usable in modern .NET publish scenarios: Native AOT, single-file, and trimming-safe builds.

The core blocker (from `specs/tech-stack.md`) is **DotLiquid**, which uses reflection-based template evaluation incompatible with .NET's Native AOT and IL trimming.

## Scope

All three Phase 2 roadmap items are in scope:

1. **AOT / trimming support** — Replace DotLiquid with **Scriban** as the help-rendering engine. Scriban's model supports source-generated type descriptors, making it trim-safe. Validate with `dotnet publish -p:PublishAot=true`.
2. **Single-file publish** — Ensure `dotnet publish -p:PublishSingleFile=true` produces no trim or AOT warnings.
3. **CI AOT gate** — Add an AOT publish step to the CI pipeline so regressions are caught automatically on every PR.
4. **README update** — Add high-level bullet summaries of what changed in Phase 1 and Phase 2.

## Decisions

### Templating: Scriban replaces DotLiquid

- Scriban is Liquid-compatible so existing `.liquid` help templates can be migrated with minimal syntax changes.
- Scriban supports AOT/trim-safe rendering when model objects are exposed via source-generated `IScriptObject` descriptors rather than reflection.
- DotLiquid is removed entirely; it must not remain as a transitive dependency.

## Out of Scope

- Async command handlers (Phase 3).
- Source generators / DI integration (Phase 4).
- Any new CLI features or help-formatting enhancements beyond what is needed to make Scriban parity with the current DotLiquid output.
- Changes to `KingpinNet.UI` sub-project unless it also pulls in DotLiquid.
