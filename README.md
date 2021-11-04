[![CircleCI](https://circleci.com/gh/UtopleMan/KingpinNet/tree/master.svg?style=svg)](https://circleci.com/gh/UtopleMan/KingpinNet/tree/master)
[![Latest version](https://img.shields.io/badge/nuget-v0.2-blue.svg)](https://www.nuget.org/packages/KingpinNet)
[![License GPLv3](https://img.shields.io/badge/license-GPLv3-green.svg)](http://www.gnu.org/licenses/gpl-3.0.html)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=UtopleMan_KingpinNet&metric=alert_status)](https://sonarcloud.io/dashboard?id=UtopleMan_KingpinNet)
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
- Customizable console help using the awesome [Liquid syntax](https://shopify.github.io/liquid/basics/introduction/)
- Context sensitive help output
- TAB Auto-completion on ZSH, Bash and Powershell

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

## Changelog
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
