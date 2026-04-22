# Validation — Phase 2: Modern .NET Compatibility

## Merge Checklist

All gates below must be green before raising a PR against `master`.

### 1. All existing tests pass

```sh
dotnet test
```

- [ ] Zero test failures
- [ ] Zero skipped tests introduced by this branch

### 2. AOT publish succeeds with zero warnings

```sh
dotnet publish src/KingpinNet/KingpinNet.csproj \
  -c Release \
  -p:PublishAot=true \
  -p:RuntimeIdentifier=linux-x64
```

- [ ] Exit code 0
- [ ] Output contains no `ILC` trim warnings
- [ ] Output contains no `IL2` / `IL3` analyzer warnings

### 3. Single-file publish is clean

```sh
dotnet publish src/KingpinNet/KingpinNet.csproj \
  -c Release \
  -p:PublishSingleFile=true \
  -p:RuntimeIdentifier=linux-x64
```

- [ ] Exit code 0
- [ ] No trim warnings in output

### 4. Help output unchanged (golden-file parity)

Run the snapshot/golden-file tests that capture rendered help text:

```sh
dotnet test --filter "Category=GoldenFile"
```

- [ ] All golden-file tests pass (help output matches pre-Scriban baseline exactly)

### 5. DotLiquid fully removed

```sh
grep -r "DotLiquid" --include="*.cs" --include="*.csproj" .
```

- [ ] Zero matches — DotLiquid is not referenced anywhere in the solution

### 6. README updated

- [ ] `README.md` contains a "What's New" section with Phase 1 and Phase 2 bullets

### 7. CI gate present

- [ ] CI pipeline includes an AOT publish step that fails on any warning
- [ ] CI pipeline includes a single-file publish step
