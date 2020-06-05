using System;
using System.Collections.Generic;
using System.Linq;

namespace KingpinNet
{
    public partial class ApplicationHelp : IHelpTemplate
    {
        private string GenerateExamples(string[] examples)
        {
            if (examples == null ||examples.Length == 0)
                return "";
            var result = examples.Aggregate((current, example) => current + ", " + example);
            return "(e.g. " + result + ")";
        }

        private void RecurseCommands(string currentCommand, IEnumerable<CommandItem> commands,
            List<Tuple<string, CommandItem>> finalCommands)
        {
            foreach (var command in commands)
            {
                if (command.Commands == null || command.Commands.Count() == 0)
                {
                    finalCommands.Add(new Tuple<string, CommandItem>((currentCommand + " " + command.Name).Trim(), command));
                }
                else
                {
                    if ((command.Arguments != null && command.Arguments.Count() != 0) ||
                        (command.Flags != null && command.Flags.Count() != 0))
                        finalCommands.Add(new Tuple<string, CommandItem>((currentCommand + " " + command.Name).Trim(), command));
                    RecurseCommands((currentCommand + " " + command.Name).Trim(), command.Commands, finalCommands);
                }
            }
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

                result += "--" + item.Flags.First().Name + defaultValue;
            }

            if (item.Flags.Count() > 1)
                result += "[<flags>] ";
            foreach (var argument in item.Arguments)
                result += $"<{argument.Name}> ";
            return result;
        }


        public KingpinApplication Application { get; set; }
        public CommandItem Command { get; set; }
    }
}

