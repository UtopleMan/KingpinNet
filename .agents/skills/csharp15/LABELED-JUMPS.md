# Labeled `break` and `continue`

Put a label directly on a loop or `switch` statement, then name it from an inner statement.

```csharp
rows: for (int row = 0; row < grid.Height; row++)
{
    for (int column = 0; column < grid.Width; column++)
    {
        if (grid[row, column].IsBlocked) continue rows;   // next row
        if (grid[row, column].IsGoal)    break rows;      // leave both loops
    }
}

dispatch: switch (command.Kind)
{
    case CommandKind.Batch:
        foreach (var step in command.Steps)
        {
            if (step.IsAbort) break dispatch;   // exits the switch statement, not just the foreach
            Run(step);
        }
        break;
    default:
        Run(command);
        break;
}
```

## Rules

- `break label;` targets the enclosing `for`, `foreach`, `while`, `do` or `switch` **statement** with that label. Switch *expressions* aren't targets.
- `continue label;` targets **loops** only.
- A label applies to the one statement immediately after it. In `a: b: while (...)`, the loop is labeled `b`. Give each loop one label.
- **Give each labeled loop in a method a distinct name** (`rows:`, `cols:`). Labels follow `goto`'s rules: a label's scope is its whole block including nested blocks, and two same-named labels with overlapping scopes are an error.
- One label serves both `break` and `continue`.
- The target must enclose the jump **in the same function**: a lambda or local function can't jump to an outer loop.
- A jump that crosses `try` blocks runs their `finally` blocks, innermost first. A jump can't leave a `finally` block.
- Unlabeled `break` and `continue` behave as before.

## Refactoring

| Before | After |
| --- | --- |
| A Boolean found-flag re-checked at each loop level | `break label;` |
| `goto END;` with `END:` right after the loop | `break label;` |
| `goto NEXT;` with `NEXT: ;` at the end of the body | `continue label;` |

Analyzer **IDE0410** ("Use labeled jump statement") flags these patterns and offers the fix. Its option is `csharp_style_prefer_labeled_jump_statements` (default `true`). Turn it off with `dotnet_diagnostic.IDE0410.severity = none`.

Extracting a method with an early `return` is often cleaner still. Labeled jumps win when the loops share many locals, or when extraction would need tuples or `out` parameters.

Sources: [Jump statements](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/jump-statements), [spec](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-15.0/labeled-break-continue), [IDE0410](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0410).
