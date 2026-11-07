# Gherkin Conventions

How this repo's Gherkin scenarios are written and run. Tickets carry scenarios as their acceptance criteria (`/pa-to-tickets`); `/pa-implement` and the `bdd` skill turn them into `.feature` files that go red, then green. `/pa-to-spec` fills in the seam and step vocabulary when seams are agreed.

## Seam

_Not yet agreed._ The one public boundary scenarios act and observe through, and the fakes at its edges.

## Step vocabulary

_Not yet agreed._ Every step a scenario may use, grouped by keyword. Reuse a step before adding one; a new step goes here first.

### Given: arrange state

### When: act through the seam

### Then: observe through the seam

## Tags

A tag decides **where** a scenario runs. An untagged scenario runs everywhere, including the fast local lane.

| Tag | Meaning | Lane |
|---|---|---|
| `@slow` | Correct but slow (more than about 5 s) | Excluded from the fast lane; runs in CI |
| `@wip` | Written, not yet expected to pass | Excluded everywhere until the tag is removed |

Delete rows this repo will never use; add a row before using a new tag.

## Glue

- Step definitions are grouped by topic, one binding class per topic, sharing state through a per-topic world object injected by constructor.
- Bindings are global: grep the existing step patterns and reuse a phrase before adding one.
