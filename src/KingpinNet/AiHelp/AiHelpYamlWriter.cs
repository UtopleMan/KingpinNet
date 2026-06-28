using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace KingpinNet.AiHelp;

public sealed class AiHelpYamlWriter
{
    private const string IndentUnit = "  ";

    private readonly TextWriter _out;

    public AiHelpYamlWriter(TextWriter output)
    {
        _out = output ?? throw new ArgumentNullException(nameof(output));
    }

    public void Write(KingpinApplication app)
    {
        Write(app, null);
    }

    public void Write(KingpinApplication app, CommandItem scope)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        var rootName = ResolveRootName(app);

        if (scope == null)
            WriteRoot(app, rootName);
        else
            WriteScoped(app, rootName, scope);

        WriteGlobalSections(app);
        WriteConventions();
    }

    private void WriteRoot(KingpinApplication app, string rootName)
    {
        WriteScalar(0, "command", rootName);
        if (!string.IsNullOrEmpty(app.HelpText))
            WriteScalar(0, "summary", app.HelpText);
        WriteScalar(0, "synopsis", BuildSynopsis(rootName, app));

        var visibleArgs = app.Arguments.Where(a => !a.Hidden).ToList();
        if (visibleArgs.Count > 0)
        {
            _out.WriteLine();
            _out.WriteLine("argument:");
            foreach (var arg in visibleArgs)
                WriteFlagOrArgument(arg, 1, false);
        }

        var visibleFlags = app.Flags.Where(f => !f.Hidden).ToList();
        if (visibleFlags.Count > 0)
        {
            _out.WriteLine();
            _out.WriteLine("global_flags:");
            foreach (var flag in visibleFlags)
                WriteFlagOrArgument(flag, 1, true);
        }

        var visibleCommands = app.Commands.Where(c => !c.Hidden).ToList();
        if (visibleCommands.Count > 0)
        {
            _out.WriteLine();
            _out.WriteLine("commands:");
            foreach (var cmd in visibleCommands)
                WriteCommand(rootName, cmd, new[] { cmd.Name }, 1);
        }
    }

    private void WriteScoped(KingpinApplication app, string rootName, CommandItem scope)
    {
        var path = ResolveCommandPath(app, scope);
        var fullCommand = string.Join(' ', new[] { rootName }.Concat(path));

        WriteScalar(0, "command", fullCommand);
        if (!string.IsNullOrEmpty(scope.Help))
            WriteScalar(0, "summary", scope.Help);
        WriteScalar(0, "synopsis", BuildCommandSynopsis(rootName, path, scope));

        WriteArgumentsBlock(scope.Arguments.Where(a => !a.Hidden).ToList(), 0);
        WriteFlagsBlock(scope.Flags.Where(f => !f.Hidden).ToList(), 0);

        var visibleChildren = scope.Commands.Where(c => !c.Hidden).ToList();
        if (visibleChildren.Count > 0)
        {
            _out.WriteLine();
            _out.WriteLine("commands:");
            foreach (var child in visibleChildren)
            {
                var childPath = path.Concat(new[] { child.Name }).ToArray();
                WriteCommand(rootName, child, childPath, 1);
            }
        }
    }

    private void WriteCommand(string rootName, CommandItem cmd, IReadOnlyList<string> path, int depth)
    {
        // Each command is a list item under `commands:`.
        var indent = new string(' ', depth * IndentUnit.Length);
        _out.Write(indent);
        _out.Write("- path: [");
        _out.Write(string.Join(", ", path.Select(YamlScalar)));
        _out.WriteLine("]");

        var fieldIndent = depth + 1;
        if (!string.IsNullOrEmpty(cmd.Help))
            WriteScalar(fieldIndent, "summary", cmd.Help);

        WriteScalar(fieldIndent, "synopsis", BuildCommandSynopsis(rootName, path, cmd));

        WriteArgumentsBlock(cmd.Arguments.Where(a => !a.Hidden).ToList(), fieldIndent);
        WriteFlagsBlock(cmd.Flags.Where(f => !f.Hidden).ToList(), fieldIndent);

        var visibleChildren = cmd.Commands.Where(c => !c.Hidden).ToList();
        if (visibleChildren.Count > 0)
        {
            WriteKey(fieldIndent, "commands");
            foreach (var child in visibleChildren)
            {
                var childPath = path.Concat(new[] { child.Name }).ToArray();
                WriteCommand(rootName, child, childPath, fieldIndent + 1);
            }
        }
    }

    private void WriteArgumentsBlock(IReadOnlyList<IItem> args, int depth)
    {
        if (args.Count == 0) return;
        WriteKey(depth, "argument");
        foreach (var a in args)
            WriteFlagOrArgument(a, depth + 1, false);
    }

    private void WriteFlagsBlock(IReadOnlyList<IItem> flags, int depth)
    {
        if (flags.Count == 0) return;
        WriteKey(depth, "flags");
        foreach (var f in flags)
            WriteFlagOrArgument(f, depth + 1, true);
    }

    private void WriteFlagOrArgument(IItem item, int depth, bool isFlag)
    {
        var indent = new string(' ', depth * IndentUnit.Length);
        if (isFlag)
        {
            _out.Write(indent);
            _out.Write("- long: --");
            _out.WriteLine(item.Name);
        }
        else
        {
            // First field on the list item starts the entry.
            _out.Write(indent);
            _out.Write("- type: ");
            _out.WriteLine(YamlTypeOf(item.ValueType));
        }

        var fieldDepth = depth + 1;

        if (isFlag && item.ShortName != '\0')
            WriteScalar(fieldDepth, "short", "-" + item.ShortName);

        if (isFlag)
            WriteScalar(fieldDepth, "type", YamlTypeOf(item.ValueType));

        WriteBool(fieldDepth, "takes_value", item.ValueType != ValueType.Bool);

        if (!string.IsNullOrEmpty(item.Unit))
            WriteScalar(fieldDepth, "unit", item.Unit);

        if (!string.IsNullOrEmpty(item.DefaultValue))
            WriteScalar(fieldDepth, "default", item.DefaultValue);

        WriteBool(fieldDepth, "required", item.Required);

        if (!string.IsNullOrEmpty(item.Help))
            WriteScalar(fieldDepth, "help", item.Help);

        if (!string.IsNullOrEmpty(item.Caution))
            WriteScalar(fieldDepth, "caution", item.Caution);
    }

    private void WriteGlobalSections(KingpinApplication app)
    {
        if (app.ExitCodes.Count > 0)
        {
            _out.WriteLine();
            _out.WriteLine("exit_codes:");
            foreach (var ec in app.ExitCodes)
                WriteScalar(1, ec.Code.ToString(CultureInfo.InvariantCulture), ec.Description);
        }

        if (app.Examples.Count > 0)
        {
            _out.WriteLine();
            _out.WriteLine("examples:");
            foreach (var ex in app.Examples)
            {
                _out.Write(IndentUnit);
                _out.Write("- intent: ");
                _out.WriteLine(YamlScalar(ex.Intent));
                WriteScalar(2, "command", ex.Command);
            }
        }

        if (app.Notes.Count > 0)
        {
            _out.WriteLine();
            _out.WriteLine("notes:");
            foreach (var note in app.Notes)
            {
                _out.Write(IndentUnit);
                _out.Write("- ");
                _out.WriteLine(YamlScalar(note.Text));
            }
        }

        if (app.Prefers.Count > 0)
        {
            _out.WriteLine();
            _out.WriteLine("prefer:");
            foreach (var p in app.Prefers)
            {
                _out.Write(IndentUnit);
                _out.Write("- rule: ");
                _out.WriteLine(YamlScalar(p.Rule));
                WriteScalar(2, "when", p.When);
                WriteScalar(2, "why", p.Why);
            }
        }

        if (app.Avoids.Count > 0)
        {
            _out.WriteLine();
            _out.WriteLine("avoid:");
            foreach (var a in app.Avoids)
            {
                _out.Write(IndentUnit);
                _out.Write("- rule: ");
                _out.WriteLine(YamlScalar(a.Rule));
                WriteScalar(2, "unless", a.Unless);
                WriteScalar(2, "why", a.Why);
            }
        }
    }

    private void WriteConventions()
    {
        _out.WriteLine();
        _out.WriteLine("conventions:");
        WriteScalar(1, "flag_form", "--flag=value preferred; bare for booleans");
        WriteScalar(1, "inheritance", "Commands inherit flags from each ancestor and from global_flags");
    }

    // --- helpers ---

    private void WriteScalar(int depth, string key, string value)
    {
        var indent = new string(' ', depth * IndentUnit.Length);
        if (ContainsNewline(value))
        {
            _out.Write(indent);
            _out.Write(key);
            _out.WriteLine(": |");
            var lineIndent = new string(' ', (depth + 1) * IndentUnit.Length);
            foreach (var line in value.Replace("\r\n", "\n").Split('\n'))
            {
                _out.Write(lineIndent);
                _out.WriteLine(line);
            }
        }
        else
        {
            _out.Write(indent);
            _out.Write(key);
            _out.Write(": ");
            _out.WriteLine(YamlScalar(value));
        }
    }

    private void WriteBool(int depth, string key, bool value)
    {
        var indent = new string(' ', depth * IndentUnit.Length);
        _out.Write(indent);
        _out.Write(key);
        _out.Write(": ");
        _out.WriteLine(value ? "true" : "false");
    }

    private void WriteKey(int depth, string key)
    {
        var indent = new string(' ', depth * IndentUnit.Length);
        _out.Write(indent);
        _out.Write(key);
        _out.WriteLine(":");
    }

    private static string YamlScalar(string value)
    {
        if (value == null) return "\"\"";
        if (value.Length == 0) return "\"\"";
        if (NeedsDoubleQuoting(value))
            return "\"" + EscapeForDoubleQuoted(value) + "\"";
        return value;
    }

    private static bool ContainsNewline(string value)
    {
        return !string.IsNullOrEmpty(value) && (value.IndexOf('\n') >= 0 || value.IndexOf('\r') >= 0);
    }

    private static bool NeedsDoubleQuoting(string v)
    {
        // Leading/trailing whitespace, YAML indicator chars, or values that
        // collide with reserved literals (true/false/null/numbers) require quoting.
        if (char.IsWhiteSpace(v[0]) || char.IsWhiteSpace(v[v.Length - 1])) return true;
        if (IsReservedLiteral(v)) return true;
        if (LooksLikeNumber(v)) return true;

        foreach (var c in v)
            switch (c)
            {
                case ':':
                case '#':
                case '"':
                case '\'':
                case '\\':
                case '[':
                case ']':
                case '{':
                case '}':
                case ',':
                case '&':
                case '*':
                case '!':
                case '|':
                case '>':
                case '%':
                case '@':
                case '`':
                case '?':
                case '\t':
                    return true;
            }

        switch (v[0])
        {
            case '-':
            case ' ':
                return true;
        }

        return false;
    }

    private static bool IsReservedLiteral(string v)
    {
        // YAML 1.1 booleans/null variants that need quoting when used as strings.
        var l = v.ToLowerInvariant();
        return l is "true" or "false" or "null" or "~"
            or "yes" or "no" or "on" or "off";
    }

    private static bool LooksLikeNumber(string v)
    {
        if (double.TryParse(v, NumberStyles.Float,
                CultureInfo.InvariantCulture, out _))
            return true;
        if (long.TryParse(v, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out _))
            return true;
        return false;
    }

    private static string EscapeForDoubleQuoted(string v)
    {
        var sb = new StringBuilder(v.Length + 8);
        foreach (var c in v)
            switch (c)
            {
                case '\\': sb.Append("\\\\"); break;
                case '"': sb.Append("\\\""); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default: sb.Append(c); break;
            }

        return sb.ToString();
    }

    private static string YamlTypeOf(ValueType vt)
    {
        return vt switch
        {
            ValueType.Bool => "bool",
            ValueType.Int => "int",
            ValueType.Long => "long",
            ValueType.Float => "float",
            ValueType.Url => "url",
            ValueType.Ip => "ip",
            ValueType.Tcp => "tcp",
            ValueType.Duration => "duration",
            ValueType.Date => "date",
            ValueType.Enum => "enum",
            ValueType.ListOfString => "list",
            _ => "string"
        };
    }

    private static string ResolveRootName(KingpinApplication app)
    {
        return !string.IsNullOrEmpty(app.Name) ? app.Name
            : !string.IsNullOrEmpty(app.exeFileName) ? app.exeFileName
            : "app";
    }

    private static string BuildSynopsis(string rootName, KingpinApplication app)
    {
        var parts = new List<string> { rootName };
        if (app.Commands.Any(c => !c.Hidden)) parts.Add("[commands]");
        if (app.Arguments.Any(a => !a.Hidden)) parts.Add("<arguments>");
        if (app.Flags.Any(f => !f.Hidden)) parts.Add("[flags]");
        return string.Join(' ', parts);
    }

    private static string BuildCommandSynopsis(string rootName, IReadOnlyList<string> path, CommandItem cmd)
    {
        var parts = new List<string> { rootName };
        parts.AddRange(path);
        if (cmd.Commands.Any(c => !c.Hidden)) parts.Add("<subcommand>");
        if (cmd.Arguments.Any(a => !a.Hidden)) parts.Add("<arguments>");
        if (cmd.Flags.Any(f => !f.Hidden)) parts.Add("[flags]");
        return string.Join(' ', parts);
    }

    private static IReadOnlyList<string> ResolveCommandPath(KingpinApplication app, CommandItem target)
    {
        foreach (var top in app.Commands)
        {
            var found = FindPath(top, target, new List<string>());
            if (found != null) return found;
        }

        // Fallback: just the command's own name.
        return new[] { target.Name };
    }

    private static List<string> FindPath(CommandItem current, CommandItem target, List<string> trail)
    {
        var withSelf = new List<string>(trail) { current.Name };
        if (ReferenceEquals(current, target)) return withSelf;
        foreach (var child in current.Commands)
        {
            var found = FindPath(child, target, withSelf);
            if (found != null) return found;
        }

        return null;
    }
}
