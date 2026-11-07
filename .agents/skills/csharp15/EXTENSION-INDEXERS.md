# Extension indexers

C# 14 extension blocks can now declare **indexers**. An indexer is an instance member, so the block needs a **named receiver**.

```csharp
public static class SequenceIndexer
{
    extension(IEnumerable<int> sequence)
    {
        public int this[int index] => sequence.ElementAt(index);
    }
}

IEnumerable<int> numbers = Enumerable.Range(1, 10);
int third = numbers[2];

public static class SettingsExtensions
{
    extension(IDictionary<string, string> settings)
    {
        public string this[string section, string key]
        {
            get => settings[$"{section}:{key}"];
            set => settings[$"{section}:{key}"] = value;
        }
    }
}
settings["Logging", "Level"] = "Debug";
```

## Rules

- Everything ordinary indexers support works: `get` and `set`, expression bodies, `ref`-returning accessors, `scoped` parameters, attributes, `params`.
- The usual extension-member restrictions apply: no `abstract`, `virtual`, `override`, `new`, `sealed`, `partial`, `protected`-family modifiers or `init` accessors.
- Every type parameter of a generic extension block appears in the receiver or the indexer's parameters.
- **Metadata names:** `Item` by default, so an `Item` property with `get_Item`/`set_Item` implementation methods. `[IndexerName("X")]` renames them. The implementation methods are static methods on the enclosing class with the receiver first, so you can disambiguate with `SettingsExtensions.get_Item(settings, "Logging", "Level")`.
- **Lookup order**, first applicable tier wins: (1) instance indexers, (2) implicit `Index`/`Range` support from instance members, (3) extension indexers, (4) implicit `Index`/`Range` support from extension members. An extension never hijacks an existing instance indexer. Within each scope, `Length` is tried before `Count`.
- **Type inference** uses only the receiver and the index arguments, never the assigned value: `i[42, null] = new object();` fails inference where `E.set_Item(...)` would succeed.
- Null-conditional access (`x?[i]`), index assignments in object initializers, list patterns and spread patterns all use the same resolution.

## Indexing from the end, ranges and list patterns

An extension `Count` or `Length`, an extension `this[int]` and a `Slice(int, int)` together make a type indexable with `^`, sliceable with ranges and matchable with list patterns. Classic `this`-parameter extension methods count too.

```csharp
public sealed class Readings(int[] values) { public int[] Values { get; } = values; }

public static class ReadingExtensions
{
    extension(Readings readings)
    {
        public int Count => readings.Values.Length;
        public int this[int index] => readings.Values[index];
        public Readings Slice(int start, int length) => new(readings.Values.AsSpan(start, length).ToArray());
    }
}

Readings r = new([10, 20, 30, 40]);
_ = r[^1];               // 40
_ = r[1..3][0];          // 20
_ = r is [10, .., 40];   // true
```

## Where extension indexers don't apply

- Arrays and `string`: their built-in indexing always wins, even over a declared extension.
- Expression trees: keep extension indexers out of `IQueryable` lambdas (EF Core and other LINQ providers), and index through the underlying member there.
- `base[...]` access, and receivers that are types rather than values.
- Collection-expression spread presizing: `[.. x]` doesn't presize from an extension `Length` or `Count`.

Sources: [Extension indexers](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/extension#extension-indexers), [spec](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-15.0/extension-indexers).
