---
name: csharp15
description: Use when code targets net11.0 or LangVersion 15/preview, C# 15 language features - collection expression arguments `[with(...)]`, `union` types, `closed` hierarchies, extension indexers, labeled `break`/`continue`, down-level static interface members, and the preview memory-safety model (`safe`, `unsafe(expr)`). Use when replacing marker interfaces, OneOf-style results, goto/flag loop exits or capacity/comparer boilerplate, or on build errors CS8121, CS8509, CS8655, CS9370, CS9380 or CS9363.
---
## Before writing C# 15

1. **Confirm the version.** `net11.0` without a `LangVersion` gives C# 15. To see the active version, add `#error version` temporarily: CS8304 reports the compiler and language versions.
2. **Raise the TFM, not the `LangVersion`.** A language version newer than the TFM's default is unsupported, and `latest` resolves differently per machine. Pin `15` or `preview` explicitly when you must set it.
3. **Unions on older TFMs** need `System.Runtime.CompilerServices.UnionAttribute` and `IUnion` referenced or defined in the project. They ship in the .NET 11 runtime, and the compiler does not synthesize them.
4. **Upgrading existing code:** read [BREAKING-CHANGES.md](BREAKING-CHANGES.md). `with(`, `union`, `closed` and (preview) `safe` became contextual keywords, so identifiers spelled like them need `@`.
5. When a feature spec and the Microsoft Learn language reference disagree, follow the Learn reference: it describes the shipped compiler.

## Features

Read a feature's file before you write or review code that uses it.

| Feature | Syntax | Reach for it when | Reference |
| --- | --- | --- | --- |
| Collection expression arguments | `[with(args), e1, e2]` | A capacity, comparer or constructor argument matters | [COLLECTION-ARGUMENTS.md](COLLECTION-ARGUMENTS.md) |
| Union types | `union U(A, B);`, `[Union]` | A value is one of a fixed set of types | [UNIONS.md](UNIONS.md) |
| Closed hierarchies | `closed class`, `closed record class` | A value is one of your own subclasses | [CLOSED-HIERARCHIES.md](CLOSED-HIERARCHIES.md) |
| Extension indexers | `this[...]` in `extension(T x) { }` | Indexing, `^`, ranges or list patterns on a type you don't own | [EXTENSION-INDEXERS.md](EXTENSION-INDEXERS.md) |
| Labeled `break` / `continue` | `rows: for (...) { ... break rows; }` | Leaving or continuing an outer loop: replaces `goto` and found-flags | [LABELED-JUMPS.md](LABELED-JUMPS.md) |
| Static interface members down-level | `static` helpers in interfaces | Multi-targeting .NET Framework or `netstandard2.0` | [below](#static-interface-members-on-older-runtimes) |
| Memory safety model (**preview**) | `unsafe(expr)`, `safe`, requires-unsafe members | **Only when the user opts in** to the preview | [MEMORY-SAFETY.md](MEMORY-SAFETY.md) |

## Union or closed hierarchy?

| Choose a `union` when | Choose a `closed` hierarchy when |
| --- | --- |
| The cases are existing or unrelated types (`int`, `string`, third-party types) | You own every case type and they form one family |
| The cases share nothing: no base, no shared members | The cases share base members, state or virtual behavior |
| A lightweight struct wrapper suits you (value-type cases box) | You want reference semantics and polymorphism |
| One type may belong to several unions | A type belongs to exactly one family |

Typical union uses: result-or-error returns (`union Result<T>(Ok<T>, Error)`), dispatch over a fixed message set, and replacing marker interfaces or OneOf-style wrappers that exist only to group types for pattern matching.

## Exhaustive switches

Both features exist to make a `switch` *expression* exhaustive with no default arm. The same rules hold for both:

- Switch on the union value or the closed base itself (`u switch`, `this switch`), never on `.Value` or `object`.
- Close the switch with a `null` arm instead of `_`. A `null` arm keeps CS8509 firing when someone adds a case. A `_` arm swallows the new case silently.
- Handle `null` whenever the input is nullable or can be a `default` union. A `default` union holds null contents that the compiler doesn't warn about (see [UNIONS.md](UNIONS.md#null-and-default)).

## Static interface members on older runtimes

C# 15 lets you declare and call **non-virtual static** interface members even on runtimes without default interface methods (.NET Framework, `netstandard2.0`). Call them through the interface: `IMeasurement.ToMeters(250)`.

```csharp
public interface IMeasurement
{
    public static double ToMeters(double centimeters) => centimeters / 100;
}
```

Instance default implementations, `static abstract`/`static virtual` members, and `protected`/`protected internal`/`private protected` interface members still need runtime support. For down-level targets, keep interface helpers static and non-virtual, with none of the `protected` access modifiers.

## Diagnostics

| Diagnostic | Meaning | Reference |
| --- | --- | --- |
| CS8121 | A type pattern matches no union case type (e.g. `pet is Pet`) | [UNIONS.md](UNIONS.md) |
| CS8509 | Switch not exhaustive: a missing union case or an inaccessible closed subtype | [UNIONS.md](UNIONS.md), [CLOSED-HIERARCHIES.md](CLOSED-HIERARCHIES.md) |
| CS8655 | Union contents are maybe-null and there is no `null` arm | [UNIONS.md](UNIONS.md) |
| CS8510 | Unreachable arm: a base arm before its subtype, or after all subtypes | [CLOSED-HIERARCHIES.md](CLOSED-HIERARCHIES.md) |
| CS0121 / CS0411 on `with(...)` | `with` arguments don't pick overloads or infer type arguments | [COLLECTION-ARGUMENTS.md](COLLECTION-ARGUMENTS.md) |
| CS9370 / CS9380 / CS1519 | `union` / `closed` used as an identifier | [BREAKING-CHANGES.md](BREAKING-CHANGES.md) |
| CS9363, CS9376, CS9392, CS9401, CS0227 | Preview memory-safety rules | [MEMORY-SAFETY.md](MEMORY-SAFETY.md) |
| `SwitchExpressionException` at run time | A `default` union reached a switch with no `null` arm | [UNIONS.md](UNIONS.md#null-and-default) |

The work is done when every C# 15 construct you wrote or reviewed has been checked against its feature file's rules, and the build reports none of these diagnostics.

Source: [What's new in C# 15](https://learn.microsoft.com/dotnet/csharp/whats-new/csharp-15), [Roslyn language feature status](https://github.com/dotnet/roslyn/blob/main/docs/Language%20Feature%20Status.md).
