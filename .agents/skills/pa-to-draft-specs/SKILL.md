---
name: pa-to-draft-specs
description: Break a finished wayfinder map into vertical-slice draft specs on the issue tracker, each small enough to grill, spec and ticket in one session.
disable-model-invocation: true
---

# PA To Draft Specs

A finished `/pa-wayfinder` map holds every decision but is too big for one `/pa-to-spec`. Cut it into **draft specs**: **vertical slices** of the destination, each delivering behaviour a user could observe end to end, through every layer it needs. Each draft spec later runs `/grill-with-docs`, `/pa-to-spec` and `/pa-to-tickets` in **one session**, so that session's budget is the size limit on every slice.

The issue tracker vocabulary should have been provided to you (`docs/agents/issue-tracker.md`). If not, tell the user to run `/pa-setup`.

## Process

### 1. Load the map

The user passes the map (URL or number); without one, take the most recently closed issue labelled `wayfinder:map` and name it to the user. Read its full body: Destination, Notes, Decisions so far, Not yet specified, Out of scope. Zoom into any decision ticket, ADR in `docs/adr/`, or `GLOSSARY.md` entry when a slice boundary hangs on its detail.

Done when the map is closed and none of its tickets is still open. If any is, stop: name the open tickets and send the user back to `/pa-wayfinder`.

### 2. Cut the slices

Draft the slice list in build order. Every slice:

- Is a **tracer bullet**: thin, but through every layer it touches. A layer on its own ("the database", "the API") is not a slice. The one exception is a first **walking skeleton** slice that stands up the solution, the test seam and CI for the rest to land on.
- Fits one session: at most a handful of open questions to grill, and roughly eight tickets or fewer once `/pa-to-tickets` splits it. A slice past that is two slices.
- Names the decisions it draws on, by ticket name.
- Declares the slices that **block** it.

Done when every decision on the map lands in at least one slice, every **Not yet specified** item is either an open question on a slice or listed back to the user as left out, and nothing from **Out of scope** appears in any slice.

### 3. Agree the cut

Show the list as a table: title, one-line scope, blocked by, open questions. Then grill the user on the cut (call the Skill tool with "grilling"), scoped to the boundaries: what merges, what splits, what moves earlier, which slice owns a contested decision. Questions about the content of a slice belong to its own `/grill-with-docs` later; note them as open questions on that slice and move on.

Done when the user approves the list.

### 4. Publish the draft specs

Create the `draft-spec` label if it doesn't exist. Publish one issue per slice in build order, labelled `draft-spec`, with the body below. Refer to every map ticket by its name wrapping its link. Then wire the blocking edges in a second pass, with the tracker doc's **Blocking** operation (issues need ids before they can reference each other). Finally comment on the map with the list of draft specs, by name.

<draft-spec-template>

Draft spec <n> of <total>, cut from the map [<map title>](<map url>). Next: `/grill-with-docs`, then `/pa-to-spec` and `/pa-to-tickets` in the same session.

## Scope

The behaviour this slice delivers, end to end, in the glossary's vocabulary.

## Decisions to draw on

- [<decision ticket name>](<url>)

## Open questions to grill

- <question>

## Out of scope

What a reader might expect here that belongs to another draft spec (by name) or to nobody.

</draft-spec-template>

Done when every approved slice is an issue and every blocking edge is wired.

### 5. Point at the first one

Suggest the draft spec to start with: the first in build order with no open blockers. Tell the user to open a fresh session and run `/grill-with-docs <issue>`, then `/pa-to-spec <issue>` and `/pa-to-tickets` in that same session.
