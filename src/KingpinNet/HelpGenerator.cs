using DotLiquid;
using System.Reflection;

namespace KingpinNet
{
    //
    public class HelpGenerator
    {
        private readonly KingpinApplication _application;

        public HelpGenerator(KingpinApplication application)
        {
            _application = application;
        }

        public void GenerateWithLiquid(TextWriter output, string templateResource)
        {
            try
            {
                var templateText = Read(templateResource);
                var template = Template.Parse(templateText);
                var application = ToDrop(_application);
                var result = template.Render(Hash.FromAnonymousObject(new { application = application, nl = "__NL__", sl = "__SL__" }));
                result = result.Replace(Environment.NewLine, "").Replace("__NL__", Environment.NewLine);

                var resultingLines = new List<string>();
                foreach (var line in result.Split(Environment.NewLine))
                    if (line.Contains("__SL__"))
                        resultingLines.Add(line.Substring(line.IndexOf("__SL__") + 6));
                    else
                        resultingLines.Add(line);
                output.Write(resultingLines.Aggregate((c, n) => c + Environment.NewLine + n));
            }
            catch (DotLiquid.Exceptions.SyntaxException exception)
            {
                Console.WriteLine($"Syntax error in template {templateResource}: {exception.Message}");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private ILiquidizable ToDrop(KingpinApplication application)
        {
            return new ApplicationDrop(application);
        }

        //public void Generate(TextWriter output, IHelpTemplate template = default(ApplicationHelp))
        //{

        //    if (template == null)
        //        template = new ApplicationHelp();
        //    template.Application = _application;
        //    output.Write(template.TransformText().Replace("\r\n", $"{Nl}"));
        //}

        //public void Generate(CommandItem command, TextWriter output, IHelpTemplate template = default(CommandHelp))
        //{
        //    if (template == null)
        //        template = new CommandHelp();
        //    template.Application = _application;
        //    template.Command = command;
        //    output.WriteLine(template.TransformText().Replace("\r\n", $"{Nl}"));
        //}

        private string Read(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resource);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public void GenerateWithLiquid(CommandItem command, TextWriter output, string templateResource)
        {
            throw new NotImplementedException();
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
                    if ((command.Arguments != null && command.Arguments.Count() != 0) ||
                        (command.Flags != null && command.Flags.Count() != 0))
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
                return x.Name;
            if (method == "FullCommand")
                return fullCommand;
            if (method == "CommandUsage")
                return CommandUsage(x);
            if (method == "Examples")
                return GenerateExamples(x.Examples);
            if (method == "Help")
                return x.Help;
            if (method == "Flags")
                return ToFlagsDrop(x.Flags);
            if (method == "Commands")
                return ToCommandsDrop(x.Commands);
            if (method == "Arguments")
                return ToArgumentsDrop(x.Arguments);

            return null;
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
        private string CommandUsage(CommandItem item)
        {
            var result = "";
            if (item.Flags.Count() == 1)
            {
                var defaultValue = "";
                if (string.IsNullOrWhiteSpace(item.Flags.First().DefaultValue))
                    defaultValue += "=<" + item.Flags.First().ItemType + "> ";
                else
                    defaultValue += "=<" + item.Flags.First().DefaultValue + "> ";

                if (!string.IsNullOrWhiteSpace(item.Flags.First().ValueName))
                    defaultValue = "=" + item.Flags.First().ValueName;

                if (item.Flags.First().ValueType == ValueType.Bool)
                    result += "--" + item.Flags.First().Name;
                else
                    result += "--" + item.Flags.First().Name + defaultValue;
            }

            if (item.Flags.Count() > 1)
                result += "[<flags>] ";
            foreach (var argument in item.Arguments)
                result += $"<{argument.Name}> ";
            return result;
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
                return x.Name;
            if (method == "ShortName")
                return x.ShortName;
            if (method == "Help")
                return x.Help;
            if (method == "Examples")
                return GenerateExamples(x.Examples);
            if (method == "DefaultValue")
                return x.DefaultValue;
            if (method == "Hidden")
                return x.Hidden;
            if (method == "ValueType")
                return x.ValueType;
            return null;
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
