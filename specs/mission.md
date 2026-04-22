# Mission

KingpinNet is a faithful, idiomatic .NET port of the Go [Kingpin](https://github.com/alecthomas/kingpin) CLI library.

## Problem

Building consistent, well-structured command-line interfaces in .NET is tedious. Existing libraries either require verbose configuration, lack opinionated conventions, or don't compose well with the .NET ecosystem. Developers familiar with Go's Kingpin face an unnecessary context switch when moving to .NET.

## Purpose

KingpinNet brings Kingpin's fluent, convention-driven model to .NET — giving any developer building CLI tools a productive, expressive foundation with sensible defaults.

## Target Audience

Any .NET developer building a CLI tool — whether writing open-source utilities, internal DevOps tooling, automation scripts, or admin commands. No prior Kingpin/Go knowledge required.

## Core Values

- **Fluent API** — define commands, flags, and arguments in a readable, chainable style
- **Convention over configuration** — consistent help formatting, usage strings, and shell completion out of the box
- **.NET ecosystem fit** — first-class integration with `Microsoft.Extensions.Configuration` and the broader hosting model
- **Testability** — console abstraction and parser separation make CLI logic unit-testable without process I/O
