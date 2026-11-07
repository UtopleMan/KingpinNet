# Implementation report

A self-contained HTML page showing a reviewer every production code path the ticket changed: where it lives, and how a real caller reaches it. The diagrams carry the weight; prose is one line per item, in the domain's names from `GLOSSARY.md`.

## 1. Collect the changes

Take every file the ticket changed: `git diff --name-status <base>` (the commit the work started from, against the working tree) plus the new files from `git ls-files --others --exclude-standard`. Sort every file into one of three sets:

- **UI code**: everything related to UI. HTML/CSS/XAML and other UI driver code.
- **Test code**: everything under `src/Tests` (Unit, Specs, Shared), every `.feature` file, step bindings, test doubles and fixtures.
- **Production code**: everything else.

Test code appears in the report only as the **Tests and specs** list: never as a diff, snippet, diagram participant or code path.

UI code appears in the report only as the **User Interface** list: never as a diff, snippet, diagram participant or code path.

In production code, find each **changed symbol**: every added or modified method, function, endpoint, handler or component (`git diff -U0` gives the hunks; map each hunk to the symbol that encloses it).

## 2. Trace each changed symbol to its entry point

Walk the callers of each changed symbol upward until you reach an **entry point**: a UI action (page, component, controller action the UI calls), an HTTP or gRPC endpoint, a scheduled or background job, a message consumer, or a CLI command. Search by symbol name, and through interfaces and DI registrations to the implementations they resolve to. A symbol reachable from several entry points gets one trace per entry point.

Then group the traces by entry point: one **code path** per entry point, holding every changed symbol it reaches.

Changed production code that no entry point reaches (configuration, migrations, DI wiring, new types not yet called) goes under **Other changes**.

Done when every changed symbol sits in at least one code path or under Other changes.

## 3. Draw each code path as a draw.io sequence diagram

One diagram per code path: participants are the entry point and every class or module the call passes through, left to right in call order; messages are the method calls, top to bottom.

- Changed participants: amber fill (`fillColor=#fde68a`). Changed calls: red, thick (`strokeColor=#dc2626;strokeWidth=2`). Unchanged ones stay plain, so the eye lands on the change.
- Returns are dashed open arrows, drawn only when they carry data the reader needs.
- Each participant links to its source file with a VS Code link (below).

Write each diagram as a `.drawio` file next to the report, so it opens and edits in draw.io or the VS Code draw.io extension. The skeleton, with lifelines 200 apart (a lifeline's centre is `x + 70`) and messages 40 apart:

```xml
<mxfile>
  <diagram name="Place order">
    <mxGraphModel>
      <root>
        <mxCell id="0" />
        <mxCell id="1" parent="0" />
        <UserObject id="p1" label="CheckoutPage" link="vscode://file/ABS/PATH/CheckoutPage.razor:12">
          <mxCell style="shape=umlLifeline;perimeter=lifelinePerimeter;container=1;collapsible=0;recursiveResize=0;size=40;" vertex="1" parent="1">
            <mxGeometry x="0" y="0" width="140" height="320" as="geometry" />
          </mxCell>
        </UserObject>
        <UserObject id="p2" label="OrderService" link="vscode://file/ABS/PATH/OrderService.cs:30">
          <mxCell style="shape=umlLifeline;perimeter=lifelinePerimeter;container=1;collapsible=0;recursiveResize=0;size=40;fillColor=#fde68a;" vertex="1" parent="1">
            <mxGeometry x="200" y="0" width="140" height="320" as="geometry" />
          </mxCell>
        </UserObject>
        <mxCell id="m1" value="PlaceOrder(cart)" style="html=1;verticalAlign=bottom;endArrow=block;strokeColor=#dc2626;strokeWidth=2;" edge="1" parent="1">
          <mxGeometry relative="1" as="geometry">
            <mxPoint x="70" y="80" as="sourcePoint" />
            <mxPoint x="270" y="80" as="targetPoint" />
          </mxGeometry>
        </mxCell>
        <mxCell id="r1" value="OrderId" style="html=1;verticalAlign=bottom;endArrow=open;dashed=1;" edge="1" parent="1">
          <mxGeometry relative="1" as="geometry">
            <mxPoint x="270" y="120" as="sourcePoint" />
            <mxPoint x="70" y="120" as="targetPoint" />
          </mxGeometry>
        </mxCell>
      </root>
    </mxGraphModel>
  </diagram>
</mxfile>
```

## 4. Write the report

Write everything to a fresh folder in the OS temp directory (`$TMPDIR`, falling back to `/tmp`, or `%TEMP%` on Windows): `<tmpdir>/implementation-report-<timestamp>/index.html` plus the `.drawio` files. Nothing lands in the repo.

Build `index.html` with a short script rather than by hand: it embeds each `.drawio` file's XML in the page, and the escaping is too easy to get wrong manually. The page loads only two scripts: Tailwind from `https://cdn.tailwindcss.com` and the draw.io viewer from `https://viewer.diagrams.net/js/viewer-static.min.js`. Each diagram is a viewer div whose `data-mxgraph` attribute is the HTML-escaped JSON `{"xml": "<the .drawio file's XML>", "toolbar": "zoom layers lightbox", "nav": true}`:

```html
<div class="mxgraph" data-mxgraph="{&quot;xml&quot;:&quot;&lt;mxfile&gt;...&quot;, ...}"></div>
```

**VS Code links** are `vscode://file/<absolute path>:<line>`, pointing into the checkout where the ticket was built, with line numbers from its working tree.

The page, top to bottom:

- **Header**: the ticket's title linked to its issue, the branch, the base commit, the date, and counts (code paths, changed symbols, tests, scenarios).
- **Code paths**: one card per entry point. The card's title names the behaviour in domain words ("Customer places an order"); under it the entry point, as a VS Code link. Then the sequence diagram. Then the changed symbols on that path, each a VS Code link to its first changed line (`OrderService.PlaceOrder`, monospace) with one line on what changed.
- **Other changes**: the same symbol list for changed production code no entry point reaches, each with one line on why it changed.
- **Tests and specs**: added and changed test classes and methods, and `.feature` scenarios, grouped by project (Unit, Specs). Each is a VS Code link with its name and nothing more.
- **User Interface**: added and changed UI classes and methods, and HTML/CSS/XAML and other. Each is a VS Code link with its name and nothing more.

The page fills the full width of the browser: `w-full px-8` on the main container and no `max-w-*` anywhere, and each diagram div stretches to its card's width (`w-full`), so wide sequence diagrams need no horizontal scrolling. Style it like an editorial page, not a dashboard: `bg-stone-50`, generous whitespace, one accent colour, amber and red only for change as in the diagrams.

Done when every changed symbol and every added test and scenario appears in the page, every `.drawio` file parses as XML, and every link points at an existing file and line. Return the absolute path to `index.html`.
