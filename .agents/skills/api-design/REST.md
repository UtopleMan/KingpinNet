# REST

## Resources and URLs

- Resources are **plural nouns** from the domain, never verbs: `/products`, `/orders`, `/products/{id}/reviews`. The HTTP method carries the verb, so `/getProducts` and `/users/{id}/delete` are wrong.
- `/products` is the collection, `/products/{id}` one item. Nest only to express ownership, one level deep.
- One casing for every path segment and query parameter (kebab-case paths, camelCase JSON is common); pick it once for the API.
- Prefix every path with the major version: `/api/v1/products`.
- Requests are **stateless**: each carries everything needed to handle it, credentials included.

## Methods

| Method | Use | Safe | Idempotent | Success |
|---|---|---|---|---|
| `GET /products`, `GET /products/{id}` | Read a collection or item | yes | yes | `200` |
| `POST /products` | Create in a collection | no | no | `201` with a `Location` header and the new resource |
| `PUT /products/{id}` | Replace the whole resource | no | yes | `200` or `204` |
| `PATCH /products/{id}` | Change only the fields sent | no | no | `200` or `204` |
| `DELETE /products/{id}` | Remove it | no | yes | `204` |

A client that must retry a `POST` safely (payments, orders) sends an `Idempotency-Key` header, and the server returns the first result for a repeated key.

## Status codes and errors

| Code | When |
|---|---|
| `400` | Malformed request: bad JSON, invalid parameter |
| `401` | Not authenticated: no or invalid credentials |
| `403` | Authenticated, but not allowed this action |
| `404` | The resource does not exist (or the caller may not know it does) |
| `409` | Conflicts with the current state: duplicate, stale version |
| `422` | Well-formed but fails validation rules |
| `429` | Rate limit exceeded; send `Retry-After` |
| `500` | Unexpected server failure; log the detail, return none of it |

Every error body uses one shape across the API: Problem Details (`application/problem+json`, RFC 9457) with `type`, `title`, `status`, `detail`, and field errors for validation failures.

## Collections

Every collection endpoint takes these query parameters from the start, so clients never fetch everything to filter or sort it themselves:

- **Filtering** by field: `?category=books&inStock=true`.
- **Sorting**: `?sort=price` ascending, `?sort=-price` descending; several fields comma-separated.
- **Pagination**, always with a server-enforced maximum page size:
  - `?page=3&limit=20` or `?offset=40&limit=20` for small, stable data where users jump to a page.
  - `?cursor=<opaque>&limit=20` for large or fast-changing data and infinite scroll: rows inserted mid-read are neither skipped nor repeated. Return the next cursor in the response.

## Caching

`GET` responses carry `Cache-Control`, plus an `ETag` so clients revalidate with `If-None-Match` and get `304`. An `ETag` sent back in `If-Match` on `PUT`/`PATCH` makes a stale update fail with `412` instead of overwriting.
