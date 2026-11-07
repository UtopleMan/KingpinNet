# Review report

A self-contained HTML page showing a reviewer what `pa-review` found on the integration branch: every changed method with its CRAP score, each one link away from its code in VS Code, and the smells, Standards and Spec findings beside them.

## 1. Collect the review

Take what `pa-review` produced for the integration branch, before any fixes are made:

- **The CRAP table** from its step 4: each changed non-UI, non-test method as `Type.Method`, with its file and start line, complexity, coverage and CRAP score. When `pa-review` passed on "no CRAP scores", keep its reason.
- **The Standards and Spec reports** from its step 6, each finding with the file and line it points at, where it has one.
- **The smells**: split out of the Standards report every baseline smell it named (the Standards report labels them as judgement calls, "possible Feature Envy"), each with its file and line, the hunk it quoted, and the smell baseline's fix for it. What stays under Standards is the documented-standard violations.

The report records the review as it ran, so the scores are the ones the fixes set out to bring down.

## 2. Write the report

Write everything to a fresh folder in the OS temp directory (`$TMPDIR`, falling back to `/tmp`, or `%TEMP%` on Windows): `<tmpdir>/review-report-<timestamp>/index.html`. Nothing lands in the repo.

Build `index.html` with a short script rather than by hand, so method names and quoted hunks are HTML-escaped. The page loads one script: Tailwind from `https://cdn.tailwindcss.com`.

**VS Code links** are `vscode://file/<absolute path>:<line>`, pointing into the main checkout with line numbers from the integration branch tip the review ran on.

The page, top to bottom:

- **Header**: the spec's title linked to its issue, the integration branch, the commit the review ran on, the date, and counts (methods scored, methods over 6, smells, Standards findings, Spec findings). One line under it: the links' line numbers match that commit, so check it out to follow them.
- **CRAP scores**: one table, worst score first, with columns method (`OrderService.PlaceOrder`, monospace, a VS Code link to its first line), file:line, complexity, coverage and CRAP. Rows scoring above 6 get a red score and an amber row (`bg-amber-100`); the rest stay plain. When there are no scores, the reason in place of the table. A method that also has smells lists their names in a last column.
- **Smells**: grouped by smell, in the baseline's order, each group headed by the smell's name and its fix. Under it, each place it was spotted: a VS Code link to the file and line (the method name when there is one, monospace), the quoted hunk in a code block, and one line on why it reads as that smell. When none were found, say so.
- **Standards**: the documented-standard violations as `pa-review` reported them, hard violations first, then judgement calls, each citing the standard's file and rule. Each finding with a location carries a VS Code link to it.
- **Spec**: the findings as `pa-review` reported them, grouped as missing or partial, scope creep, and looks wrong. Each finding with a location carries a VS Code link to it. When there was no spec, say so.

The page fills the full width of the browser: `w-full px-8` on the main container and no `max-w-*` anywhere. Style it like the implementation report: `bg-stone-50`, generous whitespace, one accent colour, amber and red only for scores over 6.

Done when every scored method, every smell and every finding appears in the page, and every link points at an existing file and line. Return the absolute path to `index.html`.
