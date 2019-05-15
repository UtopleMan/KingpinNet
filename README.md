[![CircleCI](https://circleci.com/gh/UtopleMan/KingpinNet/tree/master.svg?style=svg)](https://circleci.com/gh/UtopleMan/KingpinNet/tree/master)
[![Latest version](https://img.shields.io/badge/nuget-v0.2-blue.svg)](https://www.nuget.org/packages/KingpinNet)
[![License GPLv3](https://img.shields.io/badge/license-GPLv3-green.svg)](http://www.gnu.org/licenses/gpl-3.0.html)
# Kingpin.Net style command line arguments parser for .NET

<!-- MarkdownTOC -->
- [Overview](#overview)
- [Features](#features)
- [Usage](#usage)
  - [Bare-bone example](#bare-bone-example)
  - [Example integrating into Microsoft.Extensions.Settings](#example-integrating-into-microsoft.extensions.settings)
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
- Deep integration into Microsoft.Extension.Settings
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
KingpinNet.Application = "hej med dig";
```

### Example integrating into Microsoft.Extensions.Settings
## Reference documentation
### General configuration
### Commands
### Flags
### Arguments
### Custom help
## Changelog
