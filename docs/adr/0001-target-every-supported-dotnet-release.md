# Target every in-support .NET release, adding new ones at RC

KingpinNet multi-targets every .NET release that Microsoft still supports, so consumers on the LTS are not forced onto the newest STS. A new release is added as soon as it reaches RC (Microsoft's go-live support) rather than waiting for GA, and its target references the matching RC `Microsoft.Extensions.*` packages. Because a stable package may not depend on prerelease packages (NU5104), the package ships as a prerelease (`x.y.z-rc`) until GA.

## Consequences

- At GA: move the new target's `Microsoft.Extensions.*` references to the GA version, drop the `-rc` suffix and publish a stable package.
- When a release goes out of support, drop its target (`net10.0` in Nov 2028).
- Code must compile under the oldest targeted C# version, so newer language features wait until the older target is dropped.
- CI installs every targeted SDK with `dotnet-quality: preview`, and the AOT and single-file gate publishes for every target.
