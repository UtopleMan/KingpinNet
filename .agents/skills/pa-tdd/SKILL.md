---
name: pa-tdd
description: Test-driven development in C#. Use when the user wants to build features or fix bugs test-first, mentions "red-green-refactor", or wants integration tests.
---

# Test-Driven Development

TDD is the red → green loop. This skill is the reference that makes that loop produce tests worth keeping: what a good test is, where tests go, the anti-patterns, and the rules of the loop. Every section applies on every cycle: consult them before and during the loop, not after.

When exploring the codebase, read `GLOSSARY.md` (if it exists) so test names and interface vocabulary match the project's domain language, and respect ADRs in the area you're touching.

## What a good test is

Tests verify behavior through public interfaces, not implementation details. Code can change entirely; tests shouldn't. A good test reads like a specification: "user can checkout with valid cart" tells you exactly what capability exists, and it survives refactors because it doesn't care about internal structure.

See [tests-cs.md](tests-cs.md) for examples and [mocking-cs.md](mocking-cs.md) for mocking guidelines.

## Seams: where tests go

A **seam** is the public boundary you test at: the interface where you observe behavior without reaching inside. Tests live at seams, never against internals.

**Test only at pre-agreed seams.** Before writing any test, write down the seams under test and confirm them with the user. No test is written at an unconfirmed seam. You can't test everything, so agreeing the seams up front is how testing effort lands on the critical paths and complex logic instead of every edge case.

Ask: "What's the public interface, and which seams should we test?" Give each proposed seam a one-line note on what it catches and what it misses.

When the shape of that interface is itself in question (how deep the module is, where the seam belongs, what the interface should expose), call the Skill tool with "codebase-design" for the vocabulary. It is the shared source of the module, interface, depth, seam, adapter, leverage and locality terms, and it is a reference to consult, not a session to run.

## Where test files go

Tests live in three projects under `src/Tests`:

```text
src/Tests/
├── Unit/                   # Unit.csproj: unit tests (the pa-tdd skill)
├── Specs/                  # Specs.csproj: Gherkin scenarios and their glue (the bdd skill)
└── Shared/                 # Shared.csproj: test doubles and fixtures for Unit and Specs; holds no tests
    ├── Fakes/<Topic>/      # working in-memory implementations: FakeUserRepository
    ├── Stubs/<Topic>/      # canned answers, including HTTP stub servers and their JSON payloads
    ├── Mocks/<Topic>/      # doubles that record calls for the test to check: RecordingPublisher
    └── <Fixture>/          # fixtures named for what they provide: Containers/, Time/
```

### Shared

- **Every fake, stub and mock lives in Shared**, never in Unit or Specs, even when only one project uses it today. One double per boundary, used by unit tests and scenarios alike, keeps both agreeing on how that dependency behaves.
- **Reuse before adding.** Search Shared for a double of the dependency first, and extend it rather than write a second one.
- **Fixtures both projects need** (test containers, a controllable `TimeProvider`) live in Shared too, in a folder named for what they provide.
- **References run one way.** Unit and Specs reference Shared; Shared references only production projects. Unit and Specs never reference each other.
- **Doubles exist only at system boundaries** ([mocking-cs.md](mocking-cs.md)); needing one for your own class means the test is at the wrong seam.

### Unit

```text
src/Tests/Unit/
└── <Topic>/
    └── <Subject>Tests.cs
```

- **Unit mirrors production by topic**: a test of code in `src/Accounts` goes in `Unit/Accounts/`, one test class per seam, named `<Subject>Tests`.

## Anti-patterns

- **Implementation-coupled**: mocks internal collaborators, tests private methods, or verifies through a side channel (querying the database instead of using the interface). The tell: the test breaks when you refactor but behavior hasn't changed.
- **Tautological**: the assertion recomputes the expected value the way the code does (`Add(a, b).Should().Be(a + b)`, a snapshot derived by hand the same way, a constant asserted equal to itself), so it passes by construction and can never disagree with the code. Expected values must come from an independent source of truth: a known-good literal, a worked example, the spec.
- **Horizontal slicing**: writing all tests first, then all implementation. Bulk tests verify _imagined_ behavior: you test the _shape_ of things rather than user-facing behavior, the tests go insensitive to real changes, and you commit to test structure before understanding the implementation. Work in **vertical slices** instead: one test → one implementation → repeat, each test a **tracer bullet** that responds to what the last cycle taught you.

## Rules of the loop

- **Red before green.** Write the failing test first, then only enough code to pass it. Don't anticipate future tests or add speculative features.
- **One slice at a time.** One seam, one test, one minimal implementation per cycle.
- **Refactoring is not part of the loop.** It belongs to the review stage (see the `pa-review` skill), not the red → green implementation cycle.
