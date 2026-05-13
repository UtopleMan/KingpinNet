[![Latest version](https://img.shields.io/badge/nuget-v0.2-blue.svg)](https://www.nuget.org/packages/KingpinNet)
# Kingpin.Net style command line arguments parser and command line UI goodies for .NET 

<!-- MarkdownTOC -->
- [Overview](#overview)
- [Features](#features)
- [Usage](#usage)
  - [Bare-bone example](#bare-bone-example)
  - [Example integrating into Microsoft.Extensions.Configuration](#example-integrating-into-microsoft.extensions.configuration)
- [Reference documentation](#reference-documentation)
  - [General configuration](#general-configuration)
  - [Commands](#commands)
  - [Flags](#flags)
  - [Arguments](#arguments)
  - [Custom help](#custom-help)
- [Changelog](#changelog)
<!-- /MarkdownTOC -->

## Overview

Kingpin.Net is a free interpretation of the glorious Kingpin golang project, found [here](https://github.com/alecthomas/kingpin).

Using coding fluent style, you can easily build a consistent commandline interface for your tool. Kingpin.Net supports type safety on arguments and flags, and supports nested commands.

Install the Kingpin.Net nuget package using the following command in the package manager console window

```
PM> Install-Package KingpinNet
```

The Nuget package can be found [here](https://www.nuget.org/packages/Newtonsoft.Json)

 
## Features

- Fluent style API
- Rich support for commmands, sub-commands, arguments and flags
- Deep integration into Microsoft.Extensions.Configuration
- Type safe arguments and flags
- Beautiful console help
- POSIX Style short flags
- Customizable console help using [Liquid syntax](https://shopify.github.io/liquid/basics/introduction/) (rendered with [Scriban](https://github.com/scriban/scriban))
- Context sensitive help output
- TAB Auto-completion on ZSH, Bash and Powershell
- Arguments containing lists of values

## Usage

Here is the two major ways to add rich support for command line aruments into your application

### Bare-bone example

In order just to get the simplest command line parsing up and running 

```csharp
class Program
{
    static void Main(string[] args)
    {
        Kingpin.Version("0.0.1");
        Kingpin.Author("Joe Malone");
        Kingpin.ExitOnHelp();
        Kingpin.ShowHelpOnParsingErrors();

        FlagItem debug = Kingpin.Flag("debug", "Enable debug mode.").IsBool();
        FlagItem timeout = Kingpin.Flag("timeout", "Timeout waiting for ping.")
            .IsRequired().Short('t').IsDuration();
        ArgumentItem ip = Kingpin.Argument("ip", "IP address to ping.").IsRequired().IsIp();
        ArgumentItem count = Kingpin.Argument("count", "Number of packets to send").IsInt();

        var result = Kingpin.Parse(args);
        Console.WriteLine($"Would ping: {ip} with timeout {timeout} and count {count} with debug = {debug}");
        Console.ReadLine();
    }
}
```

### Example integrating into Microsoft.Extensions.Configuration

Integrating with the configuration system build into .NET Core is equally easy. Just add .AddKingpinNetCommandLine(args) to your configuration builder

```csharp
class Program
{
    static void Main(string[] args)
    {
        Kingpin.Version("1.0").Author("Peter Andersen").ApplicationName("curl")
            .ApplicationHelp("An example implementation of curl.");
        Kingpin.ShowHelpOnParsingErrors();
        var get = Kingpin.Command("get", "GET a resource.").IsDefault();
        get.Argument("url", "Retrieve a URL.").IsDefault();
        var post = Kingpin.Command("post", "POST a resource.");
        post.Argument("url", "URL to POST to.").IsRequired().IsUrl();


        var configuration = new ConfigurationBuilder().AddEnvironmentVariables()
            .AddKingpinNetCommandLine(args).Build();

        switch (configuration["command"])
        {
            case "get:url":
                Console.WriteLine($"Getting URL {configuration["get:url"]}");
                break;

            case "post":
                Console.WriteLine($"Posting to URL {configuration["post:url"]}");
                break;
        }

        Console.ReadLine();
    }
}
```
### Auto-completion in Powershell, Bash and ZSH

KingpinNet supports auto completion on all the three major terminals. Just run the following commands with your tool:

For ZSH:
```
eval "$({Your-tool-executable} --suggestion-script-zsh)"
```
For Bash:
```
eval "$({Your-tool-executable} --suggestion-script-bash)"
```
For Powershell:
```
iex "$({Your-tool-executable} --suggestion-script-pwsh)"
```

After you run the script, you are able to have TAB auto complete on your tool.

## AI-friendly help (`--help-ai`)

KingpinNet apps emit a YAML description of their command tree designed for LLM agents to consume directly, alongside the human-rendered `--help`. Two global flags are registered automatically (parallel to `--help`) once `Initialize()` runs:

- `--help-ai` — writes the YAML to stdout.
- `--help-ai-file` — writes a Markdown-wrapped copy to `<exename>-ai-help.md` next to the executable. Pass an explicit path with `--help-ai-file=/path/to/file.md` to override.

Both flags are available at the root and on every subcommand; invoking on a subcommand scopes the output to that subtree. The human `--help` output gains a single footer line — `For agents: run with --help-ai for a machine-readable description.` — so an LLM that reads `--help` first can discover the AI variant without external docs.

### Authoring the global sections

Five fluent methods on `KingpinApplication` (and the static `Kingpin` facade) populate the root-level sections of the YAML — each is emitted only when non-empty:

```csharp
Kingpin
    .ExitCode(0, "Success")
    .ExitCode(1, "Network or protocol error")

    .Example("Fetch a URL", "curl-ai get url https://example.com")

    .Note("All commands accept --timeout to bound network operations.")

    .Prefer(
        rule:  "Always pass --timeout explicitly",
        when:  "Non-interactive or scripted invocations",
        why:   "Without --timeout, curl-ai blocks indefinitely on dead peers.")

    .Avoid(
        rule:    "Don't use --insecure",
        unless:  "Hitting a trusted endpoint with a known-bad certificate during dev",
        why:     "Disables TLS verification globally; opens the request to MITM.");
```

Per-flag/argument, `.Unit("seconds")` declares what a value means and `.Caution("...")` surfaces destructive-flag warnings inline:

```csharp
Kingpin.Flag("timeout", "Set connection timeout.")
    .Short('t').IsInt().Unit("seconds").Default("5");

Kingpin.Flag("insecure", "Skip TLS certificate verification.")
    .Short('k').IsBool()
    .Caution("Disables TLS verification; only use against trusted hosts you control.");
```

### Sample output

Running the bundled `Examples/CurlAi` with `--help-ai` produces, in part:

```yaml
command: curl-ai
summary: An example implementation of curl exercising the AI-friendly help surface.
synopsis: "curl-ai [commands] [flags]"

global_flags:
  - long: --timeout
    short: "-t"
    type: int
    takes_value: true
    unit: seconds
    default: "5"
    required: false
    help: Set connection timeout.
  - long: --insecure
    short: "-k"
    type: bool
    takes_value: false
    required: false
    help: Skip TLS certificate verification.
    caution: Disables TLS verification; only use against trusted hosts you control.

commands:
  - path: [get, url]
    summary: Retrieve a URL.
    synopsis: "curl-ai get url <arguments>"
    argument:
      - type: url
        takes_value: true
        required: true
        help: URL to GET.

exit_codes:
  0: Success
  1: Network or protocol error

prefer:
  - rule: Always pass --timeout explicitly
    when: Non-interactive or scripted invocations
    why: Without --timeout, curl-ai blocks indefinitely on dead peers.

conventions:
  flag_form: "--flag=value preferred; bare for booleans"
  inheritance: Commands inherit flags from each ancestor and from global_flags
```

The full set of fields, the inheritance rule, and the format of every section are documented in `specs/roadmap.md` under "Phase 3 — AI-Friendly Help".

## What's New

### Phase 3 — AI-friendly help
 - New `--help-ai` and `--help-ai-file` global flags emit a YAML description of the command tree
 - Fluent API for declaring `ExitCode`, `Example`, `Note`, `Prefer`, and `Avoid` sections
 - Per-flag/argument `.Unit(...)` and `.Caution(...)` for machine-readable hints
 - Hand-rolled, trim/AOT-safe YAML emitter (no `YamlDotNet` runtime dependency)
 - `Examples/CurlAi/` exercises the full surface end-to-end

### Phase 2 — Modern .NET compatibility
 - Replaced DotLiquid with Scriban for trimming-safe help rendering
 - Library is now marked `IsAotCompatible` with trim, AOT, and single-file analyzers enabled
 - Help rendering no longer uses reflection — models are built as explicit `ScriptObject`/`ScriptArray` values
 - `Environment.ProcessPath` replaces `Assembly.Location` to support single-file publish
 - CI now gates on AOT and single-file publish of an example app

### Phase 1 — Stabilization
 - Resolved inconsistencies in `BaseItem.cs` and `Parser.cs`
 - Tightened parser edge-case test coverage
 - Target framework aligned to `net10.0`

## Changelog
 - 1.1.15
   - Added argument containing list of values
 - 1.1
   - Various clean ups
 - 1.0
   - Removed TT templates help generation
   - Added default DotLiquid help template
   - Cleaned up IConsole usage
 - 0.9
   - Added auto completion scripts for ZSH, Bash and Powershell
 - 0.8
   - Added first attempt on auto completions for PowerShell, BASH and ZSH
 - 0.7
   - Added KingpinNet.UI
   - Added ProgressBar and Spinner widgets
   - Removed all references to Microsofts static Console object and used the mockable IConsole interface instead
   - Updated tests
 - 0.6
   - Bug fixes    
 - 0.5
   - Bug fixes
 - 0.4
   - Added parse method to the KingpinApplication class
 - 0.2
   - Added support for Linux newlines
   - Added documentation
   - Refactored the help flag code
 - 0.1
   - Initial project structure setup
   - Help on nested commands
   - Added example applications
   - Added template help using T4 templates

## Reference documentation (WIP)
### General configuration
### Commands
### Flags
### Arguments
### Custom help
## Integration into .NET Core Options and Configuration

## Mentions
 * Check out this fantastic ASCII font library https://github.com/drewnoakes/figgle
