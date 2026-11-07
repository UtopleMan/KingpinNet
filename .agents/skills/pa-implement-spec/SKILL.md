---
name: pa-implement-spec
description: "Implement the result of /pa-to-spec and /pa-to-tickets in code, each ticket driven by its Gherkin scenarios."
disable-model-invocation: true
---

You have been provided a spec. This spec should have tickets associated with it, describing how to implement the spec. Each ticket carries Gherkin **scenarios**: they are its acceptance criteria and its tests.

The issue tracker should have been provided to you. If not, tell the user to run `/pa-setup`.

The goal is the entire spec implemented on a single **integration branch**, with every ticket resolved the way the issue tracker closes work.

The tickets are not a list of steps. They are a **task graph** with blocking relationships between them. This means there is always a **frontier** of tickets which are ready to be grabbed.

Communication to and from subagents should be sparse. Communicate primarily through **context pointers**: to the spec, tickets, research notes, and previous commits. Don't duplicate information already available via pointers.

**Implementer subagents** should be run in the background where possible for maximum concurrency.

## Steps

1. Read the spec and tickets to understand the task graph.

2. (optional) Use an **exploration subagent** to conduct any exploration required by the tickets - relevant codebase files or external documentation. Ensure the exploration subagent can save files - it should save its markdown notes in a directory outside the repo, accessible by all future subagents. This lets **implementer subagents** focus on implementation rather than exploration.

3. Create the integration branch. If the issue tracker closes work through PRs, or the user asks for one, open a draft PR after the first merge in step 5 (a branch with no commits ahead of main can't open one), marked as closing the spec and tickets.

4. Use **implementer subagents** to implement each ticket, each in its own worktree on its own branch. Each implementer subagent:
   - confirms its worktree is based on the integration branch before starting, and resets onto it if not;
   - calls the Skill tool with `bdd` to build the ticket (bdd calls `pa-tdd` for the inner loop), starting at bdd's step 2 because the ticket's scenarios are the confirmed example map, at the seam the spec agreed and `docs/agents/gherkin.md` records;
   - is done when every ticket scenario has a matching green `.feature` scenario and the full suite is green, with scenarios tagged to run elsewhere reported as not run;
   - merges the integration branch tip into its own branch before reporting done

5. Once an **implementer subagent** completes, merge its work to the integration branch with a **merger subagent**.

6. If this changes the **frontier** of available tickets, kick off more **implementer subagents** to work on the new tickets. This allows for maximum concurrency.

7. Once all tickets are complete, call the Skill tool with `pa-review` on the integration branch, and check that every ticket scenario has a matching `.feature` scenario. Write the **review report** from what it found, before any fixes: an HTML page of every changed method with its CRAP score and a VS Code link to it, beside the smells and the Standards and Spec findings, built from [REVIEW-REPORT.md](REVIEW-REPORT.md). Then fix all issues raised in a single **implementer subagent**.

8. If a draft PR exists, mark it ready for review. Otherwise, resolve each ticket the way the issue tracker closes work, and report the integration branch.

9. Clean up all **implementer subagent** worktrees.

10. Run a **report subagent** in the foreground to write the **implementation report**: an HTML page of every production code path the spec changed, each traced as a sequence diagram from the entry point that starts it (UI, job or service) down to the changed code, built from [IMPLEMENTATION-REPORT.md](IMPLEMENTATION-REPORT.md). Point it at the spec, the integration branch and its merge-base with main, and tell it to read that file first. Open the report it returns and give the user its absolute path, and the review report's from step 7, alongside the integration branch.
