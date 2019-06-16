using System;
using System.Collections.Generic;
using System.Linq;

namespace KingpinNet
{
    public partial class ApplicationHelp : IHelpTemplate
    {
        private string GenerateExamples(string[] examples)
        {
            if (examples == null)
                return "";
            var result = examples.Aggregate((current, example) => current + ", " + example);
            return "(e.g. " + result + ")";
        }

        private void RecurseCommands(string currentCommand, IEnumerable<CommandBuilder> commands, List<Tuple<string,
            CommandLineItem<string>>> finalCommands)
        {
            foreach (var command in commands)
            {
                if (command.Item.Commands == null || command.Item.Commands.Count() == 0)
                {
                    finalCommands.Add(new Tuple<string, CommandLineItem<string>>((currentCommand + " " + command.Item.Name).Trim(), command.Item));
                }
                else
                {
                    if ((command.Item.Arguments != null && command.Item.Arguments.Count() != 0) ||
                        (command.Item.Flags != null && command.Item.Flags.Count() != 0))
                        finalCommands.Add(new Tuple<string, CommandLineItem<string>>((currentCommand + " " + command.Item.Name).Trim(), command.Item));
                    RecurseCommands((currentCommand + " " + command.Item.Name).Trim(), command.Item.Commands, finalCommands);
                }
            }
        }

        private string CommandUsage(CommandLineItem<string> item)
        {
            var result = "";
            if (item.Flags.Count() == 1)
            {
                var defaultValue = "";
                if (string.IsNullOrWhiteSpace(item.Flags.First().Item.DefaultValue))
                    defaultValue += "=<" + item.Flags.First().Item.ItemType + "> ";
                else
                    defaultValue += "=<" + item.Flags.First().Item.DefaultValue + "> ";

                if (!string.IsNullOrWhiteSpace(item.Flags.First().Item.ValueName))
                    defaultValue = "=" + item.Flags.First().Item.ValueName;

                result += "--" + item.Flags.First().Item.Name + defaultValue;
            }

            if (item.Flags.Count() > 1)
                result += "[<flags>] ";
            foreach (var argument in item.Arguments)
                result += $"<{argument.Item.Name}> ";
            return result;
        }


        public KingpinApplication Application { get; set; }
        public CommandBuilder Command { get; set; }
    }
}

