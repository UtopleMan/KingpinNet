---
name: csharp
description: C# coding standards. Use before writing, reviewing or editing .cs or .razor code.
---
# C# coding standards

Many rules in this skill have a good/bad example under the same heading in [EXAMPLES.md](EXAMPLES.md). Read the matching heading when the rule alone doesn't tell you what the code should look like.

## Naming

- **Name the positive case.** Negated booleans compound badly: `isNotDisabled` becomes `!isNotDisabled` at the call site.
- **Booleans** take an `is` / `has` / `can` / `should` prefix: `isExpired`, not `flag`, `status` or `check`.
- **Constants** are PascalCase: `MaxRetryCount`, not `MAX_RETRY_COUNT`.
- **Collections** are plural: `users`, `orderIds`, not `userList`, `userArray`.
- Avoid abbreviations except genuinely universal ones (`id`, `url`, `http`, `db`).

## Functions

- **Small.** Aim for under 30 lines. Past that, a function is usually doing several things and wants splitting. This is a smell detector, not a hard limit — a 40-line function that is one flat switch statement is fine.
- **Two levels of nesting maximum. Return early.** Use guard clauses to put the exceptional cases where they're easy to scan and leave the main logic unindented, which is where a reader's eye goes first. A function with one exit point and six levels of nesting is worse than one with six exits and no nesting.

## Comments

- **A comment says why, when the code can't:** a constraint from outside the code, a platform quirk or bug being worked around, a consequence of calling it wrong, or why the obvious approach was rejected. If a reader can work out the why from the code, leave the comment out.
- The same test applies to `///` XML docs: a summary earns its place with the why, not by restating the signature.
- A comment sits on its own line, starts with `// `, and has one blank line before it.

## Error Handling

Handle errors where you can do something useful about them. Passing an error up
to a caller that has more context is correct; swallowing it is not.

- Never catch-and-ignore. An empty catch block hides the failure that would have
  told you what went wrong.
- Catch specific exception types rather than `System.Exception`, so genuinely
  unexpected failures still surface.
- A business outcome the caller is expected to handle is a result record such as
  `ValidationResult(bool Accepted, IReadOnlyList<ValidationError> Errors)`.
- Throw for the exceptional: fail fast on programmer error (bad arguments,
  impossible states), and recover from environmental error (network, disk,
  third-party).
- Use `using` declarations instead of try-finally for disposal.

## Structure

Keep files focused. A file owns one concern: one primary thing plus its close
helpers is easy to navigate; a 900-line grab-bag is not.

Order within a file: public API first, private helpers below. The reader
looking for the entry point finds it immediately.

## DRY, but honestly

Extract logic that is duplicated *and* will change together. Two blocks that
happen to look alike right now but serve different callers are not
duplication — merging them creates a coupling you'll have to tear apart later.

Rule of thumb: duplicate once, extract on the third occurrence. By then you can
see what actually varies.

## Patterns We Use
- Records for DTOs and commands
- Always pass CancellationToken to async methods

## Patterns We DON'T Use (Never Suggest)
- AutoMapper (write explicit mappings)
- Stored procedures

## Language Features

Use the latest language features (C# 12/13/14, .NET 8/9/10) unless backward compatibility is required, and follow Microsoft's official coding conventions.

- **Primary Constructors**: declare parameters on the class or struct and use them throughout, including for DI, instead of explicit fields.
- **Collection Expressions**: `int[] all = [..a, ..b];` bracket syntax and spread over `new` and concatenation.
- **Default Lambda Parameters**: give lambda parameters defaults where callers usually pass the same value.
- **Alias Any Type**: `using Point = (int x, int y);` for complex types.
- **Params Collections**: `params` takes `List<T>`, `Span<T>`, `IEnumerable<T>`; use `params ReadOnlySpan<T>` when memory matters.
- **Locking**: lock a `private readonly Lock` field kept typed as `Lock` everywhere (as `object` or via `Monitor` it falls back to `Monitor`); when the critical section awaits, use `SemaphoreSlim(1, 1)` with `WaitAsync` and release in `finally`.
- **Partial Properties and Indexers**: split a property's declaration from its implementation.
- **Implicit Index Access**: `^` works in object initializers.
- **Ref Struct Enhancements**: `ref struct` types can implement interfaces and be generic arguments under `allows ref struct`.
- **Extension Members**: `extension(...)` blocks add members without touching the original type.
- **Field-Backed Properties**: `field` replaces a hand-written backing field; an unavoidable explicit one is a camelCase private field.
- **Null-Conditional Assignment**: `customer?.Order = order;` over an `if (customer != null)` block.
- **Implicit Span Conversions**: `Span<T>` and `ReadOnlySpan<T>` in performance-critical code, relying on implicit array-to-span conversion.

## Naming Conventions

- **Pascal Casing**: types, public members and namespaces.
- **Camel Casing**: locals, parameters and private fields. Underscore prefixes (`_field`) are strictly prohibited, on backing fields too.
- **Interface Naming**: `I` prefix.
- **Type Parameter Naming**: `T` prefix and a meaningful name (`TEntity`).
- **Async Methods**: named for what they do, with no `Async` suffix: `Refresh`, not `RefreshAsync`. The `Task` return type and the `await` at the call site already say it is asynchronous. Framework and library methods keep their own names (`WaitAsync`).

## Code Layout

- **Indentation**: 4 spaces, no tabs.
- **Curly Braces**: Allman style, and braces on every block, including single-statement bodies.
- **Line Statements**: one statement and one declaration per line; one blank line between method and property definitions.
- **Namespaces**: file-scoped.
- **Using Directives**: outside the namespace, sorted alphabetically.

## Types and Variables

- **Type Specification**: language keywords (`string`, `int`), not runtime types (`System.String`).
- **Type Inference (var)**: `var` only when the type is obvious from the assigned value, and in LINQ query declarations; built-in types spelled out.

## Strings

- **String Interpolation**: for short concatenation.
- **StringBuilder**: when appending lots of text in a loop.
- **Raw String Literals**: over escape sequences.

## Other

- **Object Initializers**: to set properties at creation.
- **LINQ**: filter with `Where` early.
- **Event Handlers**: lambdas for handlers that are never removed.
- **Parameter Modifiers**: C# 14 lambdas take `out`/`ref` modifiers without explicit types.
- **Call Via Class Name**: call static members through the class name.
