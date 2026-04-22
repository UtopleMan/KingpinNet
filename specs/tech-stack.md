# Tech Stack

## Current Stack

| Layer | Technology | Notes |
|---|---|---|
| Language | C# | |
| Target framework | `net10.0` | |
| Help templating | DotLiquid | Renders Liquid templates for help output |
| Configuration | `Microsoft.Extensions.Configuration` 8.0.0 | Config source integration |
| Test framework | xUnit 2.8.0 | |
| Mocking | Moq 4.20.70 | |
| Shell completion | Embedded Bash / ZSH / PowerShell scripts | Resource files |
| UI widgets | KingpinNet.UI (sub-project) | ProgressBar, Spinner, Window, DataTable |
| Packaging | NuGet | Published to nuget.org |

## Known Gaps

### AOT / Trimming Support
DotLiquid uses reflection-based template evaluation which is incompatible with .NET's Native AOT and IL trimming. Publishing as a self-contained, single-file binary currently requires disabling trimming. Resolving this may require replacing DotLiquid with a trimming-safe alternative (e.g., Scriban with source-generated models, or compile-time help rendering).

### Async Command Handlers
Parsing is synchronous. There is no built-in mechanism for `async`/`await` command execution. Applications that perform I/O-bound work in their command handlers must manage async plumbing themselves, which is inconsistent with modern .NET patterns.

### Source Generators / DI Integration
CLI structure must be defined imperatively via the fluent API. There is no attribute-based declaration (e.g., `[Command]`, `[Flag]`) and no integration with `Microsoft.Extensions.DependencyInjection`. Source generators could enable compile-time CLI wiring from annotated classes, reducing boilerplate and enabling DI-friendly handler registration.

### Technical Debt
Accumulated internal inconsistencies in the parser and base item types that need cleanup before the above features can be added cleanly. Tracked on `master` (see `BaseItem.cs`, `Parser.cs` current modifications).
