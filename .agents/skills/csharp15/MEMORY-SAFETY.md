# Memory safety model (preview)

Use this only when the user has opted in to the preview.

This is the first step of a multi-release redesign. `unsafe` moves from marking code that *mentions pointers* to marking code that *accesses unmanaged memory*, and an `unsafe` member becomes a contract that **its callers** uphold. Some docs and diagnostics call this language version "C# 16" (`langversion:16`), meaning the preview after C# 15.

## Enabling it

```xml
<PropertyGroup>
  <LangVersion>preview</LangVersion>                                <!-- syntax + pointer relaxations -->
  <Features>$(Features);updated-memory-safety-rules</Features>       <!-- the updated rules (requires-unsafe) -->
  <AllowUnsafeBlocks>true</AllowUnsafeBlocks>                       <!-- to write unsafe / safe / unsafe(...) -->
</PropertyGroup>
```

In a file-based app:

```csharp
#:property LangVersion=preview
#:property Features=$(Features);updated-memory-safety-rules
```

- The three switches are independent. A project with the updated rules but without `AllowUnsafeBlocks` gets errors when it calls requires-unsafe APIs.
- Enable it with the `Features` flag above. A stable `<MemorySafetyRules>` property is planned but not implemented yet.

## What needs an unsafe context

| Operation | Original model | Preview model |
| --- | --- | --- |
| Declare a pointer type, take `&x` | unsafe | **safe** |
| `fixed` statement | unsafe | **safe** |
| `stackalloc` converted to a pointer | unsafe | **safe** |
| `sizeof(T)` for any unmanaged `T` | unsafe | **safe** |
| `*p`, `p->m`, `p[i]` | unsafe | unsafe |
| Function pointer invocation | unsafe | unsafe |
| Fixed-size buffer element access | unsafe | unsafe |
| Calling a member marked `unsafe` | no caller requirement | **caller needs unsafe** (updated rules) |
| `stackalloc` to `Span<T>` without an initializer in a `[SkipLocalsInit]` member | safe | **unsafe** (updated rules) |

The first four rows apply with `LangVersion=preview` alone, even without the updated rules.

## Requires-unsafe members

Under the updated rules, `unsafe` on a member's signature makes it **requires-unsafe**: every caller must be in an unsafe context. It no longer makes the body an unsafe context, so wrap each dereference in an inner `unsafe { }` block.

```csharp
/// <safety>source must point to at least 4 readable bytes.</safety>
public static unsafe int ReadInt32(byte* source)   // the obligation flows to callers
{
    unsafe { return *(int*)source; }               // the unsafe operation is scoped inside
}

unsafe { int v = ReadInt32(buffer); }              // the caller takes on the audit
```

**Propagate or discharge.** A member either marks itself `unsafe` (propagate), or validates its inputs, does the work in an inner `unsafe` block and keeps a safe signature (discharge). A discharging member is an **unsafe boundary**. Discharge wherever the member can check the invariant itself.

```csharp
public static int SumBytes(byte[] source)          // safe signature: an unsafe boundary
{
    ArgumentNullException.ThrowIfNull(source);
    fixed (byte* first = source)                   // pinning needs no unsafe
    {
        unsafe
        {
            // SAFETY: the null check and source.Length bound every read to the pinned array.
            int total = 0;
            for (int i = 0; i < source.Length; i++) total += first[i];
            return total;
        }
    }
}
```

**Document every contract.** Each requires-unsafe member gets a `/// <safety>...</safety>` block stating what callers must guarantee (an analyzer can require it). Each `unsafe` block gets a `// SAFETY:` comment saying why that spot is sound. Keep each unsafe region as small as the operation it covers.

Rules under the updated rules:

- `unsafe` is a member-level marker. On a type declaration, static constructor, finalizer, `delegate` or lambda it's an error.
- An override or implementation can be `unsafe` only if the member it overrides or implements is requires-unsafe.
- Converting a requires-unsafe method to a delegate needs an unsafe context.
- An `unsafe` constructor's `: base(...)` / `: this(...)` initializer is an unsafe context.
- A type whose parameterless constructor is `unsafe` doesn't satisfy `new()` or `struct` constraints in declarations such as base types, field types and aliases (CS9376). In expressions it satisfies them only inside an unsafe context.
- Local functions and lambdas don't inherit `unsafe` or `safe` from the containing member. Mark a local function `unsafe` explicitly. Lambdas can't be `unsafe`.
- Both parts of a `partial` member carry the same `unsafe`/`safe` modifier.
- **Properties:** put `unsafe`/`safe` either on the property or on individual accessors, never both, and never the same modifier on every accessor. Event accessors can't be marked individually.
- **Fields:** an `unsafe` field makes every write need an unsafe context. Use it when the field's type can't express an invariant, such as a `byte*` paired with a length. `readonly unsafe` adds write-once. Marking a property or event `unsafe` doesn't affect its backing field.

## The `safe` keyword

`safe` attests that a declaration is sound. Ordinary declarations are safe by default, so write `safe` only where the compiler requires a choice:

- every `extern` method, including `LibraryImport` partials,
- every instance field of a type with `LayoutKind.Explicit`, `[ExtendedLayout]`, or a nonzero `Pack`/`Size` (CS9392). For a backing field, annotate the auto-property or field-like event. Static and const fields are exempt.

```csharp
[LibraryImport("libc")]
internal static safe partial int getpid();                 // no pointers: attest safe

[LibraryImport("libc", StringMarshalling = StringMarshalling.Utf8)]
internal static unsafe partial nint strlen(byte* str);      // dereferences a pointer: propagate
```

A declaration carries at most one of `safe` and `unsafe`. There is no `safe { }` block. Compilers after .NET 11.0.100 require `AllowUnsafeBlocks` for both modifiers (CS9401 otherwise).

## `unsafe(expression)`

`unsafe(expression)` is an unsafe context for exactly one expression, ending at its closing parenthesis. Use it where a block can't go, or to keep the unsafe scope minimal:

```csharp
static readonly int Signature = unsafe(ReadSignature());       // field initializer
return await unsafe(ReadSignatureCoreAsync());                 // only the call is unsafe, not the await
int b = unsafe(c[null]);                                       // indexer with a pointer parameter (CS9363 otherwise)
```

It also works in constructor initializers and `catch (...) when (...)` filters. It needs `AllowUnsafeBlocks` and the preview language version.

## Async

`await` is allowed inside `unsafe` blocks, but not inside a `fixed` statement. Pointers kept alive across an `await` are your responsibility.

## Cross-assembly behavior

An opted-in assembly gets `[module: MemorySafetyRules(...)]`, and its requires-unsafe members get `[RequiresUnsafe]`. The compiler emits both; you can't apply them yourself.

| Caller | Callee | Result |
| --- | --- | --- |
| Updated rules | Updated rules | `unsafe` markers flow through metadata. Calls need an unsafe context. |
| Updated rules | Original rules | Compat mode: any callee member with a pointer or function pointer in its signature is treated as requires-unsafe. |
| Original rules | Updated rules | The original pointer rules apply. A requires-unsafe member with no pointer in its signature is callable from safe code. |

## Breaking changes when moving to the preview

- Lambdas can bind to delegates with pointer parameters, so overloads can become ambiguous (CS0121). Give the lambda explicit parameter types: `M((int x) => { })`.
- `safe` becomes a modifier: a type named `safe` needs `@safe`.
- Indexers and other members with pointer parameters need an unsafe context at the call site (CS9363).
- `safe`/`unsafe` modifiers or blocks without `AllowUnsafeBlocks` give CS9401 / CS0227.

Sources: [Unsafe code](https://learn.microsoft.com/dotnet/csharp/language-reference/unsafe-code), [spec](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/unsafe-evolution), [enabling the updated rules](https://learn.microsoft.com/dotnet/csharp/language-reference/compiler-options/language#enable-the-updated-memory-safety-rules).
