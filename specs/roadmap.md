# Roadmap

Items are ordered by dependency — earlier items unblock later ones.

## Phase 1 — Stabilization ✓

**Goal:** Clean internal foundations before adding new capabilities.

- [x] **Technical debt** — Resolve inconsistencies in `BaseItem.cs` and `Parser.cs`
- [x] Audit and tighten test coverage of parser edge cases
- [x] Align library target framework with `net10.0`

## Phase 2 — Modern .NET Compatibility ✓

**Goal:** Make KingpinNet a first-class citizen in modern .NET publish scenarios.

- [x] **AOT / trimming support** — Evaluate replacing DotLiquid with a trimming-safe templating approach (Scriban or compile-time help generation); validate with `dotnet publish -p:PublishAot=true`
- [x] Ensure single-file publish works without warnings
- [x] Update CI to include AOT publish as a build gate

## Phase 3 — Async Execution

**Goal:** First-class `async`/`await` for command handlers.

- [ ] **Async command handlers** — Introduce `RunAsync(Func<ParseResult, CancellationToken, Task>)` or equivalent on `KingpinApplication`
- [ ] Propagate `CancellationToken` from top-level entry point through to handlers
- [ ] Preserve sync API for backwards compatibility

## Phase 4 — Source Generators & DI

**Goal:** Attribute-driven CLI definition with dependency injection support.

- [ ] **Source generators** — Design and implement `[Command]`, `[Flag]`, `[Argument]` attributes with a Roslyn source generator that emits the fluent-API registration at compile time
- [ ] **DI integration** — Register command handlers as `IHostedService`-compatible services; integrate with `Microsoft.Extensions.DependencyInjection`
- [ ] Provide a `UseKingpin()` extension on `IHostBuilder` / `IHostApplicationBuilder`
- [ ] Example project demonstrating full Generic Host + KingpinNet setup

## Unprioritized / Backlog

- Richer UI widgets (interactive prompts, selection menus, color themes)
- Structured output modes (JSON, plain) as a first-class flag convention
- .NET interactive / REPL-style command loop mode
