---
name: pa-to-spec
description: "Turn the current conversation into a spec, with its BDD seam and Gherkin step vocabulary agreed, and publish it to the project issue tracker: no interview, just synthesis of what you've already discussed."
disable-model-invocation: true
---

This skill takes the current conversation context and codebase understanding and produces a spec. Do NOT interview the user; just synthesize what you already know.

The issue tracker and triage label vocabulary should have been provided to you. If not, tell the user to run `/pa-setup`.

If the user passes a **draft spec** (an issue labelled `draft-spec`, cut by `/pa-to-draft-specs`), read its full body and comments first, and every decision ticket it links. Its Scope and Out of scope bound the spec; the conversation (usually a `/grill-with-docs` on it) answers its open questions.

## Process

1. Explore the repo to understand the current state of the codebase, if you haven't already. Use the project's domain glossary vocabulary throughout the spec, and respect any ADRs in the area you're touching.

2. Sketch out the seams at which you're going to test the feature. Existing seams should be preferred to new ones. Use the highest seam possible. If new seams are needed, propose them at the highest point you can. The fewer seams across the codebase, the better - the ideal number is one.

At that seam, sketch the **step vocabulary** scenarios will speak: the `Given` steps that arrange state (including any fakes at the edges), the `When` steps that act through the seam, the `Then` steps that observe outcomes through it, and the tags that decide where a scenario runs (start from the tag table in `docs/agents/gherkin.md`).

Check with the user that these seams and the step vocabulary match their expectations. Then record the agreed seam, step vocabulary and tags in `docs/agents/gherkin.md` (create it from the `/pa-setup` seed if it doesn't exist): that file, not the spec, is their single source of truth.

3. Write the spec using the template below, then publish it to the project issue tracker. Apply the `ready-for-agent` triage label - no need for additional triage. Starting from a draft spec, publish into that issue instead of a new one: replace its body with the spec, keeping its first line (the pointer to the map), remove the `draft-spec` label and apply `ready-for-agent`. Its blocking edges stay as they are. Any open question the conversation left unanswered goes under Further Notes.

<spec-template>

## Problem Statement

The problem that the user is facing, from the user's perspective.

## Solution

The solution to the problem, from the user's perspective.

## User Stories

A LONG, numbered list of user stories. Each user story should be in the format of:

1. As an <actor>, I want a <feature>, so that <benefit>

<user-story-example>
1. As a mobile bank customer, I want to see balance on my accounts, so that I can make better informed decisions about my spending
</user-story-example>

This list of user stories should be extremely extensive and cover all aspects of the feature. They are the rules `/pa-to-tickets` turns into Gherkin scenarios, so each story names one behaviour with an observable outcome.

## Implementation Decisions

A list of implementation decisions that were made. This can include:

- The modules that will be built/modified
- The interfaces of those modules that will be modified
- Technical clarifications from the developer
- Architectural decisions
- Schema changes
- API contracts
- Specific interactions

Do NOT include specific file paths or code snippets. They may end up being outdated very quickly.

Exception: if a prototype produced a snippet that encodes a decision more precisely than prose can (state machine, reducer, schema, type shape), inline it within the relevant decision and note briefly that it came from a prototype. Trim to the decision-rich parts, not a working demo, just the important bits.

## Testing Decisions

A list of testing decisions that were made. Include:

- The BDD seam, in one line, with a pointer to `docs/agents/gherkin.md` for its step vocabulary and tags (never restate them here)
- Which behaviours cannot be reached through that seam, and how each is verified instead (a run-elsewhere tag, a setup check, a CI step)
- Prior art for the tests (i.e. similar feature files and step definitions in the codebase)

## Out of Scope

A description of the things that are out of scope for this spec.

## Further Notes

Any further notes about the feature.

</spec-template>
