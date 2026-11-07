# Union types

A union value holds exactly one value from a fixed set of **case types**. That value is the union's **contents**. The compiler provides implicit conversions from each case type, exhaustive `switch` checking and null-state tracking of the contents. C# unions are unions of *types*, not tagged unions. For a discriminated union, declare a fresh record type for each case.

```csharp
public record class Cat(string Name);
public record class Dog(string Name, string Breed);
public record class Bird(string Name);

public union Pet(Cat, Dog, Bird);

Pet pet = new Dog("Rex", "Labrador");   // implicit union conversion

string name = pet switch
{
    Dog d => d.Name,
    Cat c => c.Name,
    Bird b => b.Name,
};                                      // exhaustive: no default arm needed
```

## Declaration

```
[attributes] [modifiers] [partial] union Name[<T...>](CaseType1, CaseType2, ...) [: interfaces] [where ...]
    { members }   or   ;
```

- **Case types** are any types convertible to `object`: classes, structs, interfaces, type parameters, nullable types and other unions. Cases may overlap.
- **Lowering:** a plain `struct` (not a `record struct`) with `[Union]`, implementing `IUnion`, with `public object? Value { get; }` and one public constructor per case type. Value-type contents are **boxed** into the single `object?` field.
- **No record behavior:** no generated equality, cloning or deconstruction. A union answers "which case is it?". Its data lives in the case types.
- **The body** may declare methods, computed properties, operators and interfaces. State goes in the case types, so the body takes no instance fields, auto-properties or field-like events. It also can't declare a public single-parameter constructor, because those are the generated case constructors. Every constructor you write chains through `this(...)` to a generated one. A member that conflicts with a generated member is an error.

```csharp
public record class Meters(double Value);
public record class Feet(double Value);

public union Length(Meters, Feet)
{
    // Inside the body, `this switch` matches on the contents.
    public double TotalMeters => this switch
    {
        Meters m => m.Value,
        Feet f => f.Value * 0.3048,
        null => throw new InvalidOperationException("The Length has no value."),
    };

    public Length Add(Length other) => new Meters(TotalMeters + other.TotalMeters);
}

// Generic, discriminated style with fresh case types:
public sealed record class None;
public sealed record class Some<T>(T Value);
public union Option<T>(None, Some<T>);

public sealed record class Ok<T>(T Value);
public sealed record class Error(string Message);
public union Result<T>(Ok<T>, Error);

static Result<int> Parse(string text)
{
    if (int.TryParse(text, out var number))
        return new Ok<int>(number);   // union conversion Ok<int> -> Result<int>
    return new Error($"'{text}' is not a number");
}
```

Keep case lists flat. A union listed as its own case (`union Pet(Cat, Dog, Pet)`) makes `pet is Pet` test for a *contained* `Pet`, which reads as something else.

## Conversions

- An implicit union conversion from `E` to union `U` exists when `E` has a *standard* implicit conversion to some case type `C`. It runs the matching creation member, such as `new Pet(dog)`. When `U` is a struct, `E` also converts to `U?`.
- If several case types are equally applicable, the conversion is ambiguous (an error).
- A user-defined `implicit operator` beats the union conversion. A user-defined `explicit` operator wins only under a cast.
- A union conversion is not a standard conversion, so it doesn't chain with a user-defined conversion or another union conversion.
- Union conversions are implicit only and are never lifted. If `int` is a case type, `int?` doesn't convert to the union. Make `int?` the case type when you need that.

## Pattern matching: contents or instance

Type-based patterns look at the **contents**. Untyped recursive patterns look at the **union instance**.

| Pattern on a union expression | Tests |
| --- | --- |
| Type or declaration pattern: `Dog`, `Dog d`, `x is Dog` | The contents |
| Typed property/positional pattern: `Dog { Breed: "Lab" }` | The contents' type, then the nested patterns on that case value |
| Untyped property/positional pattern: `{ Name: "Rex" }` | The union instance's own members or `Deconstruct` |
| List pattern `[...]` | The union instance's count and indexer |
| `var x`, `_` | The union instance, without unwrapping |
| Constant or relational pattern, including `null` | The contents. `null` also matches a null class-union reference or an empty `Nullable<U>`. |

```csharp
public union Pet(Cat, Dog)
{
    public string Name => this switch { Cat c => c.Name, Dog d => d.Name, null => "" };
}

Pet pet = new Cat("Rex");
pet is Dog { Breed: "Labrador" };  // false: tests the contents
pet is { Name: "Rex" };            // true: uses the union's own Name property
pet is not null;                   // true: the "has contents" test

Pet empty = default;
empty is { };                      // TRUE: a non-nullable struct union always matches { }
empty is not null;                 // false: the contents are null
```

- **Test for contents with `is not null`** (or `.Value is not null`). `pet is Pet` is CS8121: a type pattern must match at least one case type. To capture the union itself, use `var`.
- Logical patterns keep the distinction: `var p and not null` captures the union and checks for contents. In `Dog and { Breed: "Lab" }`, the unwrapped `Dog` flows into the property pattern.
- A type parameter constrained to a union (`T u where T : MyUnion`) is never treated as a union. `u is int` there is ordinary type matching.

## Null and default

- A `switch` expression that handles every case type is exhaustive as long as nullability analysis considers the contents non-null.
- If any case type is nullable (`union NullablePet(Cat?, Dog, Bird)`), the contents are "maybe null", and a switch without a `null` arm gives CS8655.
- **The trap:** `default(Pet)` has null contents. That includes default-initialized fields, array elements and `default` itself. When every case type is non-nullable, the compiler still tracks the contents as "not null" and doesn't warn, so the switch throws `SwitchExpressionException` at run time. Add a `null` arm wherever a default union can reach the switch.
- Null-state rules: contents start "maybe null" if any case type is maybe-null, and "not null" otherwise. Creating a union copies the input's null state. A `true` result from `HasValue` or `TryGetValue` makes the contents "not null" on that branch.

```csharp
static string Describe(Pet? pet) => pet switch   // a nullable union struct works too
{
    Dog d => d.Name,
    Cat c => c.Name,
    Bird b => b.Name,
    null => "no pet",   // matches both an empty Pet? and a Pet with null contents
};
```

## Hand-written unions

Write a union by hand to adapt an existing type, to avoid boxing or use a tagged layout, to get reference semantics with a class, or to control the layout for interop. On hot paths with value-type cases, a hand-written non-boxing union is the default choice.

**The basic pattern** (all members public):

1. `[System.Runtime.CompilerServices.Union]` on the type.
2. One or more public constructors with exactly **one by-value or `in` parameter**. Each parameter type defines a case type.
3. A public `Value` property of type `object?` (or `object`) with a `get` accessor.

Also implement `IUnion` (`object? Value { get; }`), as the generated unions do. It enables run-time checks like `x is IUnion { Value: null }`.

```csharp
[System.Runtime.CompilerServices.Union]
public struct Shape : System.Runtime.CompilerServices.IUnion
{
    private readonly object? _value;
    public Shape(Circle value) { _value = value; }
    public Shape(Rectangle value) { _value = value; }
    public object? Value => _value;
}
```

**Non-boxing access.** Add `public bool HasValue { get; }` and one `public bool TryGetValue(out C value)` per case type. The compiler then calls `TryGetValue` for type patterns and `HasValue` for `null` patterns instead of boxing through `Value`. Each member is independently optional, and neither falls back to the other. For a nullable value case type such as `int?`, the `out` parameter is the underlying `int`. The compiler only picks a `TryGetValue` reachable through identity, implicit reference or boxing conversions.

```csharp
[System.Runtime.CompilerServices.Union]
public struct IntOrBool : System.Runtime.CompilerServices.IUnion
{
    private readonly int _int; private readonly bool _bool; private readonly byte _tag; // 0 none, 1 int, 2 bool
    public IntOrBool(int? value) { if (value.HasValue) { _int = value.Value; _tag = 1; } }
    public IntOrBool(bool? value) { if (value.HasValue) { _bool = value.Value; _tag = 2; } }
    public object? Value => _tag switch { 1 => _int, 2 => _bool, _ => null };
    public bool HasValue => _tag != 0;
    public bool TryGetValue(out int value) { value = _int; return _tag == 1; }
    public bool TryGetValue(out bool value) { value = _bool; return _tag == 2; }
}

string s = new IntOrBool((int?)42) switch { int i => $"int: {i}", bool b => $"bool: {b}" }; // no boxing
```

**Union member provider.** Use one when the constructors must be private, creation needs a factory, or the type is a `record class`. Nest a **public** interface named `IUnionMembers` in the union type and implement it. The compiler then looks for union members **only** on that interface (inherited members included), never on the type itself. The interface declares a static `Create(C value)` per case type (returning a type identity-convertible to the union), `Value`, and optionally `TryGetValue` and `HasValue`.

```csharp
[System.Runtime.CompilerServices.Union]
public struct Outcome<T> : Outcome<T>.IUnionMembers
{
    private readonly object? _value;
    private Outcome(object? value) => _value = value;

    public interface IUnionMembers
    {
        static Outcome<T> Create(T? value) => new(value);
        static Outcome<T> Create(Exception? value) => new(value);
        object? Value { get; }
        bool TryGetValue(out T value);
        bool TryGetValue(out Exception value);
    }

    object? IUnionMembers.Value => _value;
    public bool TryGetValue(out T value) { if (_value is T t) { value = t; return true; } value = default!; return false; }
    public bool TryGetValue(out Exception value) { if (_value is Exception e) { value = e; return true; } value = default!; return false; }
}
```

**Class-based unions:** put `[Union]` on a `sealed` class. `null` then matches both a null reference and null contents. Seal it because a derived class inherits `IUnion` but gets its own constructor set, which makes union matching confusing.

**Well-formedness contract.** The compiler trusts these without checking them, and a type that breaks them matches wrongly:

- **Soundness:** `Value` is always null or an instance of a case type, even for `default`.
- **Stability:** a union created from case `C` reports `C`, or null if the input was null.
- **Creation equivalence:** a value convertible to two case types produces the same union through either constructor.
- **Consistency:** `HasValue` and `TryGetValue` agree with `Value`.

**Torn reads:** a union struct member that reads `this` more than once can see a torn value when the variable is reassigned concurrently. Copy `this` into a local first when that matters.

Sources: [Union types](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/union), [spec](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-15.0/unions), [patterns](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns).
