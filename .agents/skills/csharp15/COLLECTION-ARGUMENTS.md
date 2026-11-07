# Collection expression arguments: `with(...)`

`with(...)` passes arguments to the target collection's constructor or builder method from inside a collection expression. It must be the **first** element.

```csharp
string[] values = ["one", "two", "three"];

List<string> names = [with(capacity: values.Length * 2), .. values];                       // List<T>(int capacity)
HashSet<string> set = [with(StringComparer.OrdinalIgnoreCase), "Hello", "HELLO", "hello"]; // 1 element
IList<int> numbers = [with(capacity: 100), 1, 2, 3];                                        // creates List<int>(100)
List<int> copy = [with([1, 2]), 3];                                                         // List<T>(IEnumerable<T>): 1, 2, 3
```

Refactor to it: `new List<T>(n)` followed by `AddRange(src)` becomes `[with(n), .. src]`, and `new HashSet<T>(comparer) { a, b }` becomes `[with(comparer), a, b]`. Classic initializers (`new List<T>(n) { ... }`) are unchanged.

## How `with(...)` binds, by target type

| Target type | What `with(args)` does |
| --- | --- |
| Class or struct implementing `IEnumerable` (no `[CollectionBuilder]`, not a type parameter) | Overload resolution over accessible instance constructors. Named arguments and expanded `params` work. |
| Type with `[CollectionBuilder(typeof(B), "Create")]` | Overload resolution over the `Create` overloads, each treated as if its last parameter were removed. `args` bind to the parameters **before** the final `ReadOnlySpan<T>`. |
| `IEnumerable<T>`, `IReadOnlyCollection<T>`, `IReadOnlyList<T>` | Only an empty `with()`, which means the same as no `with`. |
| `ICollection<T>`, `IList<T>` | `with()` or `with(int capacity)`. The compiler constructs a `List<T>`. |
| Arrays, `Span<T>`, `ReadOnlySpan<T>`, anything else | Error, even for an empty `with()`. Target `List<T>`, `HashSet<T>` or a builder type instead. |

So to set a capacity, target `List<T>` or `IList<T>`: `IReadOnlyList<T> x = [with(capacity: 10)]` is an error.

## Builder types with configuration parameters

`Create` methods can take configuration parameters before the span, and a type can have several `Create` overloads. When you author a builder, put configuration parameters first and the element `ReadOnlySpan<T>` **last**: `with(...)` can't bind to a span-first shape with optional parameters after it.

```csharp
[CollectionBuilder(typeof(MySetBuilder), nameof(MySetBuilder.Create))]
public sealed class MySet<T> : IEnumerable<T>
{
    private readonly HashSet<T> _items;

    internal MySet(ReadOnlySpan<T> items, IEqualityComparer<T>? comparer = null)
    {
        _items = new HashSet<T>(comparer);
        foreach (var item in items) _items.Add(item);
    }

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class MySetBuilder
{
    public static MySet<T> Create<T>(ReadOnlySpan<T> items) => new(items);
    public static MySet<T> Create<T>(IEqualityComparer<T> comparer, ReadOnlySpan<T> items) => new(items, comparer);
}

MySet<string> s = [with(StringComparer.OrdinalIgnoreCase), "A", "a", "B"]; // two elements
MySet<string> t = [with(), "x"];                                          // calls Create(ReadOnlySpan<T>)
```

## Targets whose constructors all take arguments

A type with no parameterless constructor is now a valid target, as long as the expression has a `with(...)`:

```csharp
public class NamedValues(string name) : List<int>
{
    public string Name { get; } = name;
}

NamedValues readings = [with("temperature"), 18, 21, 24];
// NamedValues bad = [18, 21, 24];   // error: no parameterless constructor and no with(...)
```

## Overload resolution and inference ignore the arguments

Only the *presence* of `with(...)` decides whether a collection expression converts to a candidate type. The arguments bind after the target type is fixed, so give the target explicitly:

```csharp
static void Print(List<string> values) { }
static void Print(HashSet<string> values) { }

Print([with(StringComparer.OrdinalIgnoreCase), "one", "ONE"]); // CS0121: ambiguous
HashSet<string> h = [with(StringComparer.OrdinalIgnoreCase), "one", "ONE"];
Print(h);                                                        // OK

static void Use<T>(List<T> values) { }
Use([with(10)]);        // CS0411: T can't be inferred from a capacity argument
Use<int>([with(10)]);   // OK
```

## Rules

- **Evaluation order:** `with(...)` arguments first, then the elements left to right, each exactly once. In `.. GetItems()`, `GetItems()` is *called* in its position, but its result may be *enumerated* after later elements are evaluated, so keep enumeration free of side effects that later elements depend on.
- Use named arguments when an argument could match several constructors: `[with(default)]` on `List<int>` is ambiguous, `[with(capacity: n)]` is not.
- Arguments are statically typed: `dynamic` arguments and `__arglist` are errors.
- C# 15 has no dictionary expressions. The spec mentions `IDictionary<K,V>` targets with comparer arguments and `key: value` elements, but they belong to a separate proposal that is still in progress.

Sources: [Collection expression arguments](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/collection-expressions#collection-expression-arguments), [spec](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-15.0/collection-expression-arguments).
