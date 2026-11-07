---
name: clean-code
description: Clean code principles. Use before writing, reviewing or editing code in any language.
---

# Clean code

Uncle Bob's *Clean Code*: code reads like well-written prose to the next developer. When in doubt, default to clean.

## General

- Boy Scout rule: leave touched code cleaner than you found it.
- Simple over clever; follow standard conventions and the Principle of Least Surprise.
- DRY: each piece of knowledge has one authoritative home.
- Fix root causes. Keep safeties (warnings, checks, failing tests) switched on.
- Consistency: do similar things the same way.

## Names

- Intention-revealing: why it exists, what it does, how it is used. A name that needs a comment is the wrong name.
- Pronounceable, searchable, unambiguous; single letters only for loop counters.
- Names tell the truth (`accountList` only if it is a list) and make meaningful distinctions, with no encodings or type prefixes.
- Classes are nouns, methods are verbs, booleans read as questions (`isActive`, `hasPermission`).
- One word per concept: pick one of get/fetch/retrieve and keep it.
- Named constants in place of magic numbers.

## Functions

- Small, then smaller. Do one thing at one level of abstraction; extract until nothing meaningful remains.
- Zero to two arguments; at three, group them into an object.
- Split a boolean flag parameter into two functions.
- Command/query separation: a function either acts or answers, and its name declares every side effect.
- Explanatory variables for complex expressions; conditionals phrased positively (`if (isValid)`).

## Error handling

- Exceptions over error codes, thrown with enough context to diagnose. Expected business outcomes are return values.
- Isolate try/catch in its own function; error handling is its one thing.
- Return empty collections, special-case objects or optionals where null would go, and pass non-null arguments.

## Comments

- Explain yourself in code first; a comment marks a failure to express.
- Comment only intent, clarification, warnings of consequences, TODOs, legal notices and public-API docs.
- Delete commented-out code and redundant, noise, journal and closing-brace comments; version control remembers.

## Formatting

- Stepdown rule: callers above callees, so a file reads top-down like a narrative.
- Blank lines between concepts, density within one; declare variables near their use; keep dependent and similar functions close.
- Short lines, no horizontal alignment, real indentation even for one-line bodies.

## Objects and design

- Objects hide data behind behaviour; data structures expose data with no behaviour. Pick one per type.
- Law of Demeter: talk to direct collaborators only, never `a.getB().getC().run()`.
- Value objects over primitives; polymorphism over type-switching conditionals.
- SOLID. Classes are small by responsibility (one reason to change), with few instance variables and high cohesion.
- Base classes know nothing of their derivatives.
- Inject dependencies; depend on abstractions.
- Configuration lives at high levels, and only what truly varies is configurable.
- Wrap third-party APIs at the edges; encapsulate each boundary condition in one place.
- Keep concurrency code separate from other code.

## Tests

- Test-first: a failing test before the production code that passes it.
- F.I.R.S.T.: Fast, Independent, Repeatable, Self-validating, Timely.
- One concept per test; test code held to production standards.

## Smells

Rigidity, fragility, immobility, needless complexity, needless repetition, opacity: each one is a refactoring cue.
