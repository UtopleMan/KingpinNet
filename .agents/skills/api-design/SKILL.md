---
name: api-design
description: Designing an API contract between systems. Use when the user wants to design or review endpoints, a GraphQL schema or gRPC service, choose between REST, GraphQL, gRPC, WebSockets or messaging, or plan versioning and deprecation.
---

# API Design

An API is a **contract**: which requests a client may make and which responses it gets back, with the implementation hidden behind it. Design the contract first, then build to it. The bar: a developer can use the API correctly without reading its documentation.

For authentication, authorization and hardening, call the Skill tool with "api-security": every API design covers them.

## Process

1. **Requirements.** Write down the core use cases, what is in scope now and what is out, the performance needs (where the hot paths and bottlenecks are) and the security constraints. Done when the user has confirmed each list.
2. **Approach.** Pick how the contract is derived, and say which:
   - **Top-down**: from use cases and workflows, for new capabilities.
   - **Bottom-up**: from existing data models and capabilities, when the system already exists. Shape the contract around what clients need, not around the tables.
   - **Contract-first**: write the request and response shapes (OpenAPI, a `.graphql` schema, `.proto` files) before any implementation, and agree them with the user.
3. **Style and protocol**, from the tables below. Record the choice and the reason.
4. **Contract.** Design it with [REST.md](REST.md) or [GRAPHQL.md](GRAPHQL.md), whichever the style is. For gRPC, the `.proto` file is the contract: one service per capability, request and response messages per RPC, and field numbers never reused.
5. **Lifecycle.** State how the API is versioned, monitored once deployed, and eventually deprecated and retired (see Evolution below).

Done when every confirmed use case maps to an operation in the contract, and every operation names its success response, its error responses and who may call it.

## Principles

Check every operation against all four:

- **Consistent**: one naming scheme, one casing, one pattern for collections, errors and pagination across the whole API. Two endpoints that do the same kind of thing look the same.
- **Simple**: the core use cases, nothing speculative. Each operation does what its name says and nothing else; a read never changes state.
- **Secure**: authenticated, authorized, inputs validated, rate limited ("api-security").
- **Performant**: paginate collections, cache what can be cached, return only the data the client needs, and save round trips by including small related data the client will always need next.

## Choosing a style

| Style | Fits | Costs |
|---|---|---|
| **REST** | Public and web/mobile APIs; resource-shaped domains; HTTP caching | Several round trips for nested views; fixed response shapes |
| **GraphQL** | Complex UIs where each screen needs different nested data | One endpoint, so no HTTP caching; query cost needs limits |
| **gRPC** | Service-to-service calls inside the system; streaming | Binary Protobuf over HTTP/2; browsers need gRPC-Web or a gateway |

Default to REST. Pick another style when its "Fits" names this API's main client.

## Choosing a protocol

The interaction pattern decides it:

| Pattern | Protocol |
|---|---|
| Request and response | HTTPS (always TLS, never plain HTTP) |
| Server pushes to the client as events happen: chat, live updates | WebSockets (or Server-Sent Events when only the server talks). Polling wastes requests and adds latency. |
| Work handed off to be processed later, with guaranteed delivery: orders, payments, notifications | A message broker over AMQP: a producer publishes to a queue, a consumer pulls when it has capacity. Exchanges route direct, fan-out or by topic. |
| Fast internal service calls, streaming both ways | gRPC over HTTP/2 |

Also weigh client compatibility, payload size and encoding, and the tooling and documentation developers will work with. Under the application protocol, TCP (reliable, ordered) carries anything where loss is unacceptable: payments, auth, user data. UDP (fast, lossy) suits only live media and games, where a late packet is worthless.

## Evolution

- **Version** breaking changes: REST puts the major version in the path (`/api/v1/...`); GraphQL evolves the schema in place, adding fields and marking old ones `@deprecated`. Additive changes (a new optional field, a new endpoint) need no new version.
- **Deprecate before retiring**: announce it, mark it in the contract (the `Deprecation` and `Sunset` headers, `@deprecated`), keep the old version running until its clients have moved, then remove it.
