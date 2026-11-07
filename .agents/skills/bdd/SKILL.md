---
name: bdd
description: Behaviour-driven development with Gherkin. Use when the user wants to specify behaviour as scenarios, mentions BDD or Given/When/Then, or writes .feature files or Reqnroll step definitions.
---

# Behaviour-Driven Development

BDD is the outer loop around TDD: a Gherkin scenario goes red, and the `pa-tdd` skill's red → green loop drives it green. What a good test is, seams and the anti-patterns all come from `pa-tdd`; call the Skill tool with "pa-tdd" before starting.

Read `GLOSSARY.md`: every step speaks the domain's language. Read `docs/agents/gherkin.md` if it exists: its seam, step vocabulary, tags and glue layout are this repo's conventions and take precedence over the defaults below.

## 1. Map examples

If confirmed scenarios already exist (a ticket's, for example), they are the example map: skip to step 2.

Otherwise, before any Gherkin, build an **example map** with the user:

- **Rules**: the acceptance criteria, one line each.
- **Examples**: concrete cases per rule, with real values.
- **Questions**: every unknown, put to the user.

Done when every rule has a confirmed example and no question is open. The confirmed examples are the scenarios; they replace `pa-tdd`'s pre-agreed seams.

## 2. One example at a time

For each example, in order:

1. **Formulate** it as a scenario (rules below).
2. **Glue** it: write the missing step definitions.
3. Run it **red**: it must fail on a `Then`, because the behaviour is missing. A pending step or a glue exception is not red yet.
4. Drive it **green** with the `pa-tdd` loop, then take the next example.

Done when every example on the map is a green scenario.

## Scenario rules

- **Declarative**: steps state intent and outcome in domain terms. The test: would the wording survive a rewrite of the implementation?
- **One behaviour**: one `When`; `Given` is prior state, `Then` is an outcome observed through the seam. Name the scenario after the outcome.
- **Only the details that decide the outcome** appear in steps; incidental data lives in the glue.
- `Rule:` groups scenarios under the map's rules. `Scenario Outline` when examples differ only in data. `Background` only for `Given`s every scenario shares.

## Glue

- Steps are thin: translate the phrase, call the seam, store the result. State lives in a per-topic world object injected into the binding class, unless `docs/agents/gherkin.md` lays glue out differently.
- Bindings are global: grep the existing `[Given]`/`[When]`/`[Then]` patterns and reuse a phrase before adding one.
- Tags decide where a scenario runs (`@requires-docker`, `@requires-device` and the rest): tag every scenario that needs what a tag names, using the tag table in `docs/agents/gherkin.md`. Untagged scenarios run in the fast lane.

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
- **Doubles exist only at system boundaries** (the `pa-tdd` skill's mocking-cs.md); needing one for your own class means the test is at the wrong seam.

### Specs

```text
src/Tests/Specs/
├── Features/<Topic>/       # .feature files, one per capability: Login.feature
├── Steps/<Topic>/          # thin step bindings; Common/ for steps every topic uses
├── Support/                # Reqnroll hooks ([BeforeScenario], [AfterScenario]) and plugin config
└── Infrastructure/         # how steps act through the seam
    ├── Drivers/<Topic>/    # thin API or system drivers
    └── PageObjects/        # UI screen elements
```

- **Features, Steps and Drivers share the same business topics**, so a capability's scenarios, glue and drivers sit under one name.
