---
name: pa-implement
description: "Implement a ticket or spec behaviour-first: its Gherkin scenarios go red, then green, via the bdd and pa-tdd skills."
disable-model-invocation: true
---

Implement the work described by the user in the spec or tickets.

If the user passes a ticket reference, fetch it from the issue tracker and state its title before starting. If the reference is ambiguous, ask. Note the `HEAD` commit before changing anything: it is the base the implementation report diffs against.

Call the Skill tool with "bdd" (it calls "pa-tdd" for the inner loop). The ticket's scenarios are the confirmed example map, so start at bdd's step 2. The seam is the one the parent spec agreed and `docs/agents/gherkin.md` records; pa-tdd's pre-agreed-seam rule is met by it. If the work has no scenarios yet, run bdd's example mapping with the user first.

Each ticket scenario becomes a `.feature` scenario, verbatim apart from glue-driven step rewording. Each goes **red** on a `Then` before it goes **green**.

Run typechecking regularly, single feature files regularly, and the full test suite once at the end. Scenarios tagged to run elsewhere (see `docs/agents/gherkin.md`) are excluded from the local run; report them as not run.

Done when every ticket scenario has a matching green `.feature` scenario and the full suite is green.

Then call the Skill tool with "pa-review" to review the work.

Run a **report subagent** in the foreground to write the **implementation report**: an HTML page of every production code path the ticket changed, each traced as a sequence diagram from the entry point that starts it (UI, job or service) down to the changed code, built from [IMPLEMENTATION-REPORT.md](IMPLEMENTATION-REPORT.md). Point it at the ticket and the commit the work started from, and tell it to read that file first. Open the report it returns and give the user its absolute path.

Don't commit your work to the current branch. 
