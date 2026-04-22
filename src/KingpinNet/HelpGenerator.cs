using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KingpinNet;
public class HelpGenerator
{
    private readonly KingpinApplication _application;
    private readonly IConsole console;
    private const string NewLine = "__NL__";
    public HelpGenerator(KingpinApplication application, IConsole console)
    {
        _application = application;
        this.console = console;
    }

    public void Generate(TextWriter output, string templateText)
    {
        try
        {
            var template = Template.ParseLiquid(templateText);
            if (template.HasErrors)
            {
                console.Out.WriteLine($"Syntax error in template: {string.Join(", ", template.Messages)}");
                return;
            }
            var root = new ScriptObject();
            root["application"] = BuildApplicationObject(_application);
            var result = RenderWithRoot(template, root);
            result = result.Replace("\n", "").Replace("\r", "").Replace(NewLine, Environment.NewLine);
            output.Write(result);
        }
        catch (Exception exception)
        {
            console.Out.WriteLine(exception.ToString());
        }
    }

    public string ReadResourceInExecutingAssembly(string resource)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resource);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public void Generate(CommandItem command, TextWriter output, string templateText)
    {
        try
        {
            var template = Template.ParseLiquid(templateText);
            if (template.HasErrors)
            {
                console.Out.WriteLine($"Syntax error in template: {string.Join(", ", template.Messages)}");
                return;
            }
            var root = new ScriptObject();
            root["application"] = BuildApplicationObject(_application);
            root["command"] = BuildCommandObject(command, command.Name);
            var result = RenderWithRoot(template, root);
            result = result.Replace("\n", "").Replace("\r", "").Replace(NewLine, Environment.NewLine);
            output.Write(result);
        }
        catch (Exception exception)
        {
            console.Out.WriteLine(exception.ToString());
        }
    }

    private static string RenderWithRoot(Template template, ScriptObject root)
    {
        var context = new LiquidTemplateContext { MemberRenamer = member => member.Name };
        context.PushGlobal(root);
        return template.Render(context);
    }

    private static ScriptObject BuildApplicationObject(KingpinApplication application)
    {
        var obj = new ScriptObject();
        obj["Name"] = application.Name ?? "";
        obj["Help"] = application.HelpText ?? "";
        obj["Flags"] = ToScriptArray(application.Flags.Select(BuildItemObject));
        obj["Commands"] = ToScriptArray(application.Commands.Select(c => BuildCommandObject(c, c.Name)));
        obj["RecursedCommands"] = ToScriptArray(RecurseCommands("", application.Commands));
        obj["Arguments"] = ToScriptArray(application.Arguments.Select(BuildItemObject));
        return obj;
    }

    private static ScriptObject BuildCommandObject(CommandItem command, string fullCommand)
    {
        var obj = new ScriptObject();
        obj["Name"] = command.Name ?? "";
        obj["FullCommand"] = fullCommand ?? "";
        obj["Help"] = command.Help ?? "";
        obj["Examples"] = FormatExamples(command.Examples);
        obj["Flags"] = ToScriptArray(command.Flags.Select(BuildItemObject));
        obj["Commands"] = ToScriptArray(command.Commands.Select(c => BuildCommandObject(c, c.Name)));
        obj["Arguments"] = ToScriptArray(command.Arguments.Select(BuildItemObject));
        obj["RecursedCommands"] = ToScriptArray(RecurseCommands("", command.Commands));
        return obj;
    }

    private static ScriptArray ToScriptArray(IEnumerable<ScriptObject> items)
    {
        var array = new ScriptArray();
        foreach (var item in items)
            array.Add(item);
        return array;
    }

    private static ScriptObject BuildItemObject(IItem item)
    {
        var obj = new ScriptObject();
        obj["Name"] = item.Name ?? "";
        obj["ShortName"] = item.ShortName == 0 ? "" : item.ShortName.ToString();
        obj["Help"] = item.Help ?? "";
        obj["Examples"] = FormatExamples(item.Examples);
        obj["DefaultValue"] = item.DefaultValue ?? "";
        obj["Hidden"] = item.Hidden;
        obj["ValueType"] = item.ValueType.ToString();
        obj["ValueName"] = item.ValueName ?? "";
        return obj;
    }

    private static List<ScriptObject> RecurseCommands(string currentCommand, IEnumerable<CommandItem> commands)
    {
        var result = new List<ScriptObject>();
        foreach (var command in commands)
        {
            var path = (currentCommand + " " + command.Name).Trim();
            if (command.Commands == null || !command.Commands.Any())
            {
                result.Add(BuildCommandObject(command, path));
            }
            else
            {
                if (!command.Hidden)
                    result.Add(BuildCommandObject(command, path));
                result.AddRange(RecurseCommands(path, command.Commands));
            }
        }
        return result;
    }

    private static string FormatExamples(string[] examples)
    {
        if (examples == null || examples.Length == 0)
            return "";
        return "(e.g. " + string.Join(", ", examples) + ")";
    }
}
