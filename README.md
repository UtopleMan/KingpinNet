[![CircleCI](https://circleci.com/gh/UtopleMan/KingpinNet/tree/master.svg?style=svg)](https://circleci.com/gh/UtopleMan/KingpinNet/tree/master)
[![Latest version](https://img.shields.io/badge/nuget-v0.2-blue.svg)](https://www.nuget.org/packages/KingpinNet)
[![License GPLv3](https://img.shields.io/badge/license-GPLv3-green.svg)](http://www.gnu.org/licenses/gpl-3.0.html)
# Kingpin.Net style command line arguments parser for .NET

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
- Customizable console help using T4 templates
- context sensitive help output

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
        get.Command("url", "Retrieve a URL.").IsDefault();
        var post = Kingpin.Command("post", "POST a resource.");
        post.Argument("url", "URL to POST to.").IsRequired().IsUrl();


        var configuration = new ConfigurationBuilder().AddEnvironmentVariables()
            .AddKingpinNetCommandLine(args).Build();

        switch (configuration["command"])
        {
            case "get:url":
                Console.WriteLine($"Getting URL {configuration["get:url:url"]}");
                break;

            case "post":
                Console.WriteLine($"Posting to URL {configuration["post:url"]}");
                break;
        }

        Console.ReadLine();
    }
}
```

## Changelog
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
