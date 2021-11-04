using DotLiquid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KingpinNet
{
    //
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

        public void Generate(TextWriter output, string liquidTemplateText)
        {
            try
            {
                var template = Template.Parse(liquidTemplateText);
                var application = ToDrop(_application);
                var result = template.Render(Hash.FromAnonymousObject(new { application = application }));
                result = result.Replace(Environment.NewLine, "").Replace(NewLine, Environment.NewLine);
                output.Write(result);
            }
            catch (DotLiquid.Exceptions.SyntaxException exception)
            {
                console.Out.WriteLine($"Syntax error in template [{liquidTemplateText}]: {exception.Message}");
            }
            catch (Exception exception)
            {
                console.Out.WriteLine(exception.ToString());
            }
        }

        private ILiquidizable ToDrop(KingpinApplication application)
        {
            return new ApplicationDrop(application);
        }
        private ILiquidizable ToDrop(CommandItem command)
        {
            return new CommandDrop(command);
        }

        public string ReadResourceInExecutingAssembly(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resource);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public void Generate(CommandItem command, TextWriter output, string liquidTemplateText)
        {
            try
            {
                var template = Template.Parse(liquidTemplateText);
                var applicationDrop = ToDrop(_application);
                var commandDrop = ToDrop(command);
                var result = template.Render(Hash.FromAnonymousObject(new { application = applicationDrop, command = commandDrop }));
                result = result.Replace(Environment.NewLine, "").Replace(NewLine, Environment.NewLine);
                output.Write(result);
            }
            catch (DotLiquid.Exceptions.SyntaxException exception)
            {
                console.Out.WriteLine($"Syntax error in template [{liquidTemplateText}]: {exception.Message}");
            }
            catch (Exception exception)
            {
                console.Out.WriteLine(exception.ToString());
            }
        }
    }

    internal class ApplicationDrop : Drop
    {
        private readonly KingpinApplication application;

        public ApplicationDrop(KingpinApplication application)
        {
            this.application = application;
        }

        public override object BeforeMethod(string method)
        {
            if (method == "Name")
                return application.Name;
            if (method == "Help")
                return application.HelpText;
            if (method == "Flags")
                return ToFlagsDrop(application.Flags);
            if (method == "Commands")
                return ToCommandsDrop(application.Commands);
            if (method == "RecursedCommands")
                return ToFinalCommandsDrop();
            if (method == "Arguments")
                return ToArgumentsDrop(application.Arguments);
            return null;
        }
        private object ToFinalCommandsDrop()
        {
            var commands = new List<CommandDrop>();
            RecurseCommands("", application.Commands, commands);
            return commands;
        }

        private object ToArgumentsDrop(IEnumerable<IItem> arguments)
        {
            return arguments.Select(x => new ItemDrop(x));
        }

        private object ToCommandsDrop(IEnumerable<CommandItem> commands)
        {
            return commands.Select(x => new CommandDrop(x));
        }

        private object ToFlagsDrop(IEnumerable<IItem> flags)
        {
            return flags.Select(x => new ItemDrop(x));
        }

        private void RecurseCommands(string currentCommand, IEnumerable<CommandItem> commands,
            List<CommandDrop> finalCommands)
        {
            foreach (var command in commands)
            {
                if (command.Commands == null || command.Commands.Count() == 0)
                {
                    finalCommands.Add(new CommandDrop((currentCommand + " " + command.Name).Trim(), command));
                }
                else
                {
                    if (!command.Hidden)
                        finalCommands.Add(new CommandDrop((currentCommand + " " + command.Name).Trim(), command));
                    RecurseCommands((currentCommand + " " + command.Name).Trim(), command.Commands, finalCommands);
                }
            }
        }
    }

    internal class CommandDrop : Drop
    {
        private string fullCommand;
        private CommandItem x;

        public CommandDrop(CommandItem x)
        {
            this.fullCommand = x.Name;
            this.x = x;
        }
        public CommandDrop(string fullCommand, CommandItem x)
        {
            this.fullCommand = fullCommand;
            this.x = x;
        }
        public override object BeforeMethod(string method)
        {
            if (method == "Name")
                return x.Name ?? "";
            if (method == "FullCommand")
                return fullCommand ?? "";
            if (method == "Examples")
                return GenerateExamples(x.Examples) ?? "";
            if (method == "Help")
                return x.Help ?? "";
            if (method == "Flags")
                return ToFlagsDrop(x.Flags);
            if (method == "Commands")
                return ToCommandsDrop(x.Commands);
            if (method == "Arguments")
                return ToArgumentsDrop(x.Arguments);
            if (method == "RecursedCommands")
                return ToFinalCommandsDrop();
            throw new NotImplementedException($"Didn't implement {method} on CommandDrop");
        }
        private object ToArgumentsDrop(IEnumerable<IItem> arguments)
        {
            return arguments.Select(x => new ItemDrop(x));
        }

        private object ToCommandsDrop(IEnumerable<CommandItem> commands)
        {
            return commands.Select(x => new CommandDrop(x));
        }

        private object ToFlagsDrop(IEnumerable<IItem> flags)
        {
            return flags.Select(x => new ItemDrop(x));
        }
        private object ToFinalCommandsDrop()
        {
            var commands = new List<CommandDrop>();
            RecurseCommands("", x.Commands, commands);
            return commands;
        }
        private void RecurseCommands(string currentCommand, IEnumerable<CommandItem> commands, List<CommandDrop> finalCommands)
        {
            foreach (var command in commands)
            {
                if (command.Commands == null || command.Commands.Count() == 0)
                {
                    finalCommands.Add(new CommandDrop((currentCommand + " " + command.Name).Trim(), command));
                }
                else
                {
                    if (!command.Hidden)
                        finalCommands.Add(new CommandDrop((currentCommand + " " + command.Name).Trim(), command));
                    RecurseCommands((currentCommand + " " + command.Name).Trim(), command.Commands, finalCommands);
                }
            }
        }
        private string GenerateExamples(string[] examples)
        {
            if (examples == null || examples.Length == 0)
                return "";
            var result = examples.Aggregate((current, example) => current + ", " + example);
            return "(e.g. " + result + ")";
        }
    }

    internal class ItemDrop : Drop
    {
        private IItem x;

        public ItemDrop(IItem x)
        {
            this.x = x;
        }
        public override object BeforeMethod(string method)
        {
            if (method == "Name")
                return x.Name ?? "";
            if (method == "ShortName")
                return x.ShortName == 0 ? "" : x.ShortName.ToString();
            if (method == "Help")
                return x.Help ?? "";
            if (method == "Examples")
                return GenerateExamples(x.Examples);
            if (method == "DefaultValue")
                return x.DefaultValue ?? "";
            if (method == "Hidden")
                return x.Hidden;
            if (method == "ValueType")
                return x.ValueType;
            if (method == "ValueName")
                return x.ValueName ?? "";
            throw new NotImplementedException($"Didn't implement {method} on ItemDrop");
        }
        private string GenerateExamples(string[] examples)
        {
            if (examples == null || examples.Length == 0)
                return "";
            var result = examples.Aggregate((current, example) => current + ", " + example);
            return "(e.g. " + result + ")";
        }
    }
}
