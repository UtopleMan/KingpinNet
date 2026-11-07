# Closed hierarchies: `closed`

`closed` on a class means only its declaring **assembly and module** may declare **direct** subclasses. Because the set of direct descendants is fixed, a `switch` expression that handles them all is exhaustive without a default arm, in consuming assemblies too.

```csharp
// Assembly 1
public closed record class JobStatus;
public sealed record class Queued : JobStatus;
public sealed record class Running(int PercentComplete) : JobStatus;
public sealed record class Completed(TimeSpan Elapsed) : JobStatus;
public sealed record class Failed(string Error) : JobStatus;

// Any assembly
public static string Describe(JobStatus status) => status switch
{
    Queued => "waiting to start",
    Running(var percent) => $"{percent}% complete",
    Completed(var elapsed) => $"finished in {elapsed.TotalSeconds:F1}s",
    Failed(var error) => $"failed: {error}",
};  // no CS8509

// Assembly 2
public record class Paused : JobStatus;   // error: 'JobStatus' is a closed class
```

## Declaration rules

- `closed` applies to classes and record classes only.
- A closed class is **implicitly abstract** and can't be instantiated. `closed` combined with `sealed`, `static` or an explicit `abstract` is an error.
- Direct subtypes live in the same assembly **and module** as the closed class.
- **Not transitive.** A plain subclass of a closed class is open to other assemblies. Lock the hierarchy down by making each leaf `sealed` and each intermediate level `closed`.
- **Generic rule:** a generic class that directly derives from a closed class uses **every** one of its type parameters in the base class specification.

```csharp
public closed record class Tree<T>;
public record class Leaf<T>(T Value) : Tree<T>;                        // OK
public record class Branch<T>(Tree<T> Left, Tree<T> Right) : Tree<T>;  // OK
public record class Constant<U>(U Value) : Tree<int>;                  // error: U not used in the base
```

## Exhaustiveness rules

- **Only direct descendants count.** An indirect descendant is covered by its parent's arm. Ordinary subsumption still applies: put a more-derived arm (`Sedan`) **before** its base (`Car`), since the reverse is CS8510 (unreachable).
- An arm for the closed base itself, after all direct descendants, is an unreachable-case error.
- **Accessibility:** a direct descendant that isn't accessible at the switch site (an `internal` subtype seen from another assembly, a `protected` nested subtype) leaves the switch non-exhaustive (CS8509). Make every direct subtype at least as accessible as the base, or add a base-type arm.
- **Nullable input:** a switch over `JobStatus?` also needs a `null` arm.
- **Type parameters:** a switch over `X` where `X : JobStatus` is exhaustive on the same terms as one over `JobStatus`.
- **Generic closed bases:** only subtypes that can produce the given construction are required. For `C<string>`, a `D2<V> : C<V[]>` case isn't needed. Constraints on subtypes are *not* used to rule cases out, and a non-speakable generic subtype makes the switch non-exhaustive. Add a base-type arm in those situations.
- A closed class with no subtypes is not exhausted by an empty switch.
- Exhaustiveness checking applies to `switch` expressions, which is where CS8509 comes from.

## Other effects

- **Interface casts:** when every class in a closed hierarchy is `sealed` or `closed`, an explicit cast to an interface that no class in it implements is a compile error, the same rule as for a sealed class.
- **Metadata:** the class gets `[IsClosedType]` and every constructor gets `[CompilerFeatureRequired("ClosedClasses")]`, so older compilers and VB can't derive from it.
- **Versioning:** the list of direct subtypes is public API. Adding `closed` to a published class, or adding a direct subtype to a closed class, breaks consumers whose exhaustive switches stop being exhaustive.
- `closed` is a contextual keyword: identifiers named `closed` need `@closed` (see [BREAKING-CHANGES.md](BREAKING-CHANGES.md)).

Sources: [`closed` modifier](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/closed), [spec](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-15.0/closed-hierarchies), [patterns](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns).
