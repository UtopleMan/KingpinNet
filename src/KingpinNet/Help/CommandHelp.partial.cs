using System;
using System.Collections.Generic;
using System.Linq;

namespace KingpinNet.Help
{
    public partial class CommandHelp : IHelpTemplate
    {
        private string GenerateExamples(string[] examples)
        {
            if (examples == null || examples.Length == 0)
                return "";
            var result = examples.Aggregate((current, next) => current + ", " + next);
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
                    {
                        finalCommands.Add(new Tuple<string, CommandItem>((currentCommand + " " + command.Name).Trim(), command));
                    }
                    RecurseCommands((currentCommand + " " + command.Name).Trim(), command.Commands, finalCommands);
                }
            }
        }

        private string CommandUsage(CommandItem item)
        {
            var result = "";
            if (item.Flags.Count() == 1)
            {
                if (string.IsNullOrWhiteSpace(item.Flags.First().DefaultValue))
                    result += "--" + item.Flags.First().Name + "=<" + item.Flags.First().ItemType + "> ";
                else
                    result += "--" + item.Flags.First().Name + "=<" + item.Flags.First().DefaultValue + "> ";
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

