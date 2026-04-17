# Requirements — Phase 1: Stabilization

## Scope

All three roadmap items are in scope for this branch:

1. **BaseItem / Parser technical debt** — resolve the in-progress inconsistencies already visible in `BaseItem.cs` and `Parser.cs`
2. **Test coverage** — audit and tighten xUnit coverage of parser edge cases
3. **Framework retargeting** — replace `netstandard2.1` with dual `net8.0` / `net9.0` targets

## Context

This is the foundation phase. Nothing in Phase 2 (AOT), Phase 3 (async), or Phase 4 (source generators) can be built cleanly on top of an inconsistent parser core. The work here is not cosmetic — it shapes the internal model that all future phases will extend.

See [mission.md](../mission.md): KingpinNet is a faithful, idiomatic .NET port. Internally, that means the codebase should be as clean and conventional as idiomatic .NET expects.

See [tech-stack.md](../tech-stack.md): `netstandard2.1` is a legacy target. The library should track the current .NET LTS (net8.0) and current release (net9.0). Dropping `netstandard2.1` also removes constraints that block future AOT work.

## Decisions

### Refactor depth: Broader — align with future phases

Internal logic may be restructured beyond just fixing what is broken today. The goal is to leave the parser and base item types in a shape that is ready for:
- Async execution hooks (Phase 3)
- Source-generator-driven registration (Phase 4)

**Constraint:** The public API surface must not have breaking changes. Any type or member visible to NuGet consumers must remain stable.

### Framework targets

Remove `netstandard2.1`. The library `.csproj` will multi-target `net8.0;net9.0`. Tests and examples already target `net8.0` and should be extended to cover both.

## Out of Scope

- AOT / trimming changes (Phase 2)
- Async command handler API (Phase 3)
- Source generators or DI integration (Phase 4)
- UI widget additions
