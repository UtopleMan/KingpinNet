---
name: pa-where-code-lives
description: Where new server code goes: one project per aggregate root, plus Shared and Contracts. Use before adding server-side code or a type that crosses the client–server wire.
---

# Where code lives

On the server, each domain feature is its own project, built around one aggregate root. New server code goes into its feature's project; a feature without a project gets one first. Client code (apps, front ends, platform heads, shared client libraries) keeps its existing layout.

Throughout, a type's namespace follows its project and folder (`<Product>.Orders.Models`).

## Find the feature

Name the aggregate root the new code serves, in the domain's own words: the glossary when there is one (`GLOSSARY.md`, `CONTEXT.md`), otherwise the names already in the code. A new root gets its term into the glossary first (the `domain-modeling` skill), then its project.

## The feature project

- **Name**: the root's term in the plural (`Orders`), in the solution's existing project naming (`<Product>.Orders` beside `<Product>.<Name>`), with a matching folder.
- **Wiring**: added to the solution and referenced by the server host that runs it.
- **Endpoints**: public API endpoints live in the host that serves them; each maps its route and calls into the feature's `Services/`.
- **Folders**: each type goes in the one folder that says what it is:
  - `Models/`: models, data carriers and DTOs that stay on the server.
  - `Services/`: the feature's work.
  - `Repositories/`: repositories and their interfaces.

## Shared code

Server code used by more than one feature project goes in `<Product>.Shared`, with the same three folders. It references no feature project, so features depend on it without a loop. Code used by one feature stays there until a second feature needs it.

## Contracts

Data carriers that cross the wire between clients and server go in `<Product>.Contracts`, referenced by both: request and response bodies, the enums they carry, and pushed-event payloads. One folder per feature, named like its project (`Orders/`). It references no server or client project, so both sides can depend on it. Storage rows and server-side models stay in the feature's `Models/`; client-only models stay in the client.

## What stays where it is

- Code an ADR or documented convention gives its own home, such as a pure domain-rules library.
- Code already in another project, until a change moves it on purpose.

## Done when

Every file you added sits in its home: endpoints in their host, wire types in Contracts, other server code in its feature project (or Shared, when several features use it) under `Models/`, `Services/` or `Repositories/`. The solution builds.
