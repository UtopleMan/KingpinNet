Find the next incomplete phase in specs/roadmap.md (the first phase whose checkboxes are not all checked).

Create a git branch named `feature/<slug>` where `<slug>` is a short kebab-case name derived from the phase title.

Read specs/mission.md and specs/tech-stack.md for context before forming questions.

Use the AskUserQuestion tool to interview the user with exactly 3 questions — one per group — before writing any files:
1. **Scope** — which items from the roadmap phase are in scope for this branch (multiSelect)
2. **Decisions** — the key technical decision or approach for the most complex item (single select with previews where helpful)
3. **Validation** — what must be true for the branch to be considered mergeable (multiSelect)

After the user answers, create a directory under specs/ named `YYYY-MM-DD-<slug>` using today's date, then write three files into it:

**requirements.md** — scope, context, decisions made, and what is explicitly out of scope. Reference mission.md and tech-stack.md where relevant.

**plan.md** — numbered task groups (Group 1, Group 2, …). Each group has numbered steps (1.1, 1.2, …). Groups should be ordered by dependency.

**validation.md** — merge checklist with exact shell commands to run locally, and a checkbox list of every gate that must pass before the PR is raised.
