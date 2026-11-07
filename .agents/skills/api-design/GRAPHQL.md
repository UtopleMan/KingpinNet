# GraphQL

One endpoint (`/graphql`); the client names the exact fields it wants, so one request replaces the several a REST view would need, without over-fetching.

## Schema

The schema is the contract. It mirrors the domain model, in the domain's own names.

- **Types** for domain objects; a field whose type is another object (`posts: [Post!]!`) is how clients traverse relations.
- **Queries** read (`user(id: ID!): User`). **Mutations** change state, one per business action (`createPost`, `publishPost`), not generic setters. **Subscriptions** push real-time events, over WebSockets.
- Every mutation takes a single **input type** and returns a payload type holding the changed object, so the mutation can grow without breaking callers:

```graphql
type Mutation {
  createPost(input: CreatePostInput!): CreatePostPayload!
}
```

- Keep the schema **modular**: one file or module per domain area.
- Mark fields non-null (`!`) only where the server can always supply them.
- Lists that can grow are paginated with cursor connections (`first`, `after`, returning `edges`, `pageInfo`).

## Errors

The HTTP status is `200` whenever the query was executed, even if parts failed. Failures go in the response's `errors` array, each with a `message`, the `path` of the failed field, and a machine-readable code under `extensions` (`NOT_FOUND`, `FORBIDDEN`). Fields that succeeded still return their data, so the client renders what it can.

Expected business outcomes a client must handle (a duplicate name, an invalid state) are better typed into the mutation's payload, for example a union of the success type and error types, so they appear in the schema instead of in free-text messages.

## Protecting the server

A client can ask for anything the schema allows, so cap what one query may cost:

- **Depth limit**, typically 6 to 7 levels, so `user → posts → comments → author → posts → ...` cannot nest without end.
- **Complexity or cost limit** on the number of fields and list sizes.
- **Batch** resolver lookups (DataLoader) so a list of 50 posts does not fire 50 author queries.
- Authorize in the resolvers, per field, through the same rules as the rest of the system ("api-security").
