# Compiler breaking changes in the .NET 11 SDK

Check existing code for each of these when moving it to C# 15. Preview-only breaks are in [MEMORY-SAFETY.md](MEMORY-SAFETY.md#breaking-changes-when-moving-to-the-preview).

## New contextual keywords

Escape an identifier that collides with one of these with `@`.

| Spelling | Now parses as | Fix |
| --- | --- | --- |
| `[with(x, y), z]` | Collection expression arguments (LangVersion 15+) | `[@with(x, y), z]` to call a method named `with` |
| `union Foo;` inside a type | A union declaration (CS9370) | `@union` |
| A type named `closed` | The `closed` modifier (CS9380; fields of that type may give CS1519) | `@closed` |

## Other changes

- A `Span<T>`/`ReadOnlySpan<T>` collection expression now has *declaration-block* safe-context. Assigning a loop-local `Span<int> items = [x];` to an outer `scoped` span is now an error. Use an array, or hoist the span out of the loop.
- `nameof(this.P)` inside an attribute is an error. Write `nameof(P)`.
- `(X.Y) when` in a switch-expression arm parses as the constant pattern `(X.Y)` followed by a `when` clause, not as a cast.
- `x && y`, where `x` is statically an interface type and `y` is `dynamic`, is error CS7083. Cast `x` to a concrete type or to `dynamic`.
- Synthesized `ref readonly`-returning delegates and `ref readonly` local functions need `System.Runtime.InteropServices.InAttribute` (CS0518 when it's missing).

Source: [Compiler breaking changes in .NET 11](https://learn.microsoft.com/dotnet/csharp/whats-new/breaking-changes/compiler%20breaking%20changes%20-%20dotnet%2011).
