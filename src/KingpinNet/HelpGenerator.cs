using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KingpinNet
{
    public class HelpGenerator
    {
        private KingpinApplication _application;
        public HelpGenerator(KingpinApplication application)
        {
            _application = application;
        }

        public void Generate(TextWriter output)
        {
            GenerateUsage(output);
            GenerateDescription(output);
            GenerateFlags(output);
            GenerateArguments(output);
            GenerateCommands(output);
        }

        public void Generate(CommandItem command, TextWriter output)
        {
            GenerateCommandUsage(command, output);
            GenerateCommandDescription(command, output);
            GenerateCommandFlags(command, output);
            GenerateCommandArguments(command, output);
            GenerateNestedCommands(command, output);
        }


        private void GenerateCommands(TextWriter output)
        {
            if (_application.Commands.Count == 0)
                return;
            output.WriteLine("Commands:");
            var finalCommands = new List<Tuple<string, CommandLineItem>>();
            RecurseCommands("", _application.Commands, finalCommands);

            var commandNameLength = finalCommands.Max(c => c.Item1.Length);

            foreach (var command in finalCommands)
            {
                output.WriteLine($"  {command.Item1} " + CommandUsage(command.Item2));
                output.WriteLine($"    {command.Item2.Help}");
                output.WriteLine();
            }

        }

        private void GenerateNestedCommands(CommandItem command, TextWriter output)
        {
            if (command.Item.Commands.Count == 0)
                return;
            output.WriteLine("Commands:");
            var finalCommands = new List<Tuple<string, CommandLineItem>>();
            RecurseCommands("", command.Item.Commands, finalCommands);

            var commandNameLength = finalCommands.Max(c => c.Item1.Length);

            foreach (var finalCommand in finalCommands)
            {
                output.WriteLine($"  {finalCommand.Item1} " + CommandUsage(finalCommand.Item2));
                output.WriteLine($"    {finalCommand.Item2.Help}");
                output.WriteLine();
            }

        }

        private string CommandUsage(CommandLineItem item)
        {
            var result = "";
            if (item.Flags.Count == 1)
                result += "--" + item.Flags[0].Item.Name + "=<" + item.Flags[0].Item.ItemType + "> ";
            if (item.Flags.Count > 1)
                result += "[<flags>] ";
            foreach (var argument in item.Arguments)
                result += $"<{argument.Item.Name}> ";
            return result;
        }

        private void RecurseCommands(string currentCommand, List<CommandItem> commands, List<Tuple<string,
            CommandLineItem>> finalCommands)
        {
            foreach (var command in commands)
            {
                if (command.Item.Commands == null || command.Item.Commands.Count == 0)
                {
                    finalCommands.Add(new Tuple<string, CommandLineItem>((currentCommand + " " + command.Item.Name).Trim(), command.Item));
                }
                else
                {
                    if ((command.Item.Arguments != null && command.Item.Arguments.Count != 0) ||
                        (command.Item.Flags != null && command.Item.Flags.Count != 0))
                        finalCommands.Add(new Tuple<string, CommandLineItem>((currentCommand + " " + command.Item.Name).Trim(), command.Item));
                    RecurseCommands((currentCommand + " " + command.Item.Name).Trim(), command.Item.Commands, finalCommands);
                }
            }
        }

        private void GenerateFlags(TextWriter output)
        {
            if (_application.Flags == null || _application.Flags.Count == 0)
                return;
            output.WriteLine("Flags:");

            var flags = new List<string>();
            var maxFlagLength = _application.Flags.Max(x => x.Item.Name.Length) + 8;

            foreach (var flag in _application.Flags)
            {
                var flagName = "";
                if (flag.Item.ShortName != 0)
                    flagName = $"  -{flag.Item.ShortName}, --{flag.Item.Name}";
                else
                    flagName = $"      --{flag.Item.Name}";

                var spacing = maxFlagLength - flagName.Length;
                var finalString = flagName.PadRight(maxFlagLength);
                output.WriteLine($"{finalString}   {flag.Item.Help}");
            }
        }

        private void GenerateArguments(TextWriter output)
        {
            if (_application.Arguments == null || _application.Arguments.Count == 0)
                return;
            output.WriteLine("Args:");

            var Arguments = new List<string>();
            var maxArgLength = _application.Arguments.Max(x => x.Item.Name.Length) + 9;

            foreach (var arg in _application.Arguments)
            {
                var spacing = maxArgLength - arg.Item.Name.Length;
                var finalString = $"  [<{arg.Item.Name}>]".PadRight(spacing);
                output.WriteLine($"{finalString}   {arg.Item.Help}");
            }
        }

        private void GenerateCommandArguments(CommandItem command, TextWriter output)
        {
            if (command.Item.Arguments == null || command.Item.Arguments.Count == 0)
                return;
            output.WriteLine("Args:");

            var maxArgLength = command.Item.Arguments.Max(x => x.Item.Name.Length) + 9;

            foreach (var arg in command.Item.Arguments)
            {
                var spacing = maxArgLength - arg.Item.Name.Length;
                var finalString = $"  [<{arg.Item.Name}>]".PadRight(spacing);
                output.WriteLine($"{finalString}   {arg.Item.Help}");
            }
        }

        private void GenerateCommandFlags(CommandItem command, TextWriter output)
        {
            if (command.Item.Flags == null || command.Item.Flags.Count == 0)
                return;
            output.WriteLine("Flags:");

            var maxFlagLength = command.Item.Flags.Max(x => x.Item.Name.Length) + 8;

            foreach (var flag in command.Item.Flags)
            {
                var flagName = "";
                if (flag.Item.ShortName != 0)
                    flagName = $"  -{flag.Item.ShortName}, --{flag.Item.Name}";
                else
                    flagName = $"      --{flag.Item.Name}";

                var finalString = flagName.PadRight(maxFlagLength);
                output.WriteLine($"{finalString}   {flag.Item.Help}");
            }
        }

        private void GenerateDescription(TextWriter output)
        {
            if (string.IsNullOrWhiteSpace(_application.Help))
                return;
            output.WriteLine(_application.Help);
            output.WriteLine();
        }

        private void GenerateCommandDescription(CommandItem command, TextWriter output)
        {
            if (string.IsNullOrWhiteSpace(command.Item.Help))
                return;
            output.WriteLine(command.Item.Help);
            output.WriteLine();
        }

        public void GenerateUsage(TextWriter output)
        {
            var applicationText = "";
            if (!String.IsNullOrWhiteSpace(_application.Name))
                applicationText = _application.Name + " ";

            var flagsText = "";
            if (_application.Flags.Count > 0)
                flagsText = "[<flags>] ";

            var commandsText = "";
            if (_application.Commands.Count > 0)
                commandsText = "<command> ";

            var argsText = "";
            if (_application.Arguments.Count > 1)
                argsText = "[<args> ...]";
            else if (_application.Arguments.Count == 1)
                argsText = $"[<{_application.Arguments[0].Item.Name}>]";

            output.WriteLine($"usage: {applicationText}{flagsText}{commandsText}{argsText}");
            output.WriteLine();
        }

        public void GenerateCommandUsage(CommandItem command, TextWriter output)
        {
            var applicationText = "";
            if (!string.IsNullOrWhiteSpace(_application.Name))
                applicationText = _application.Name + " ";

            var flagsText = "";
            if (_application.Flags.Count > 0)
                flagsText = "[<flags>] ";

            var commandsText = command.Item.Name + " ";

            var argsText = "";
            if (_application.Arguments.Count > 1)
                argsText = "[<args> ...]";
            else if (_application.Arguments.Count == 1)
                argsText = $"[<{_application.Arguments[0].Item.Name}>]";

            output.WriteLine($"usage: {applicationText}{commandsText}{flagsText}{argsText}");
            output.WriteLine();
        }

    }
}
