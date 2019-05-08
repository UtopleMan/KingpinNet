using System;
using System.Collections.Generic;
using System.Linq;

namespace KingpinNet
{
    public partial class DefaultHelp
    {
        public KingpinApplication Application;
        private string GenerateExamples(string[] examples)
        {
            if (examples == null)
                return "";
            var result = examples.Aggregate((current, example) => current + ", " + example);
            return "(e.g. " + result + ")";
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

        private string CommandUsage(CommandLineItem item)
        {
            var result = "";
            if (item.Flags.Count == 1)
            {
                if (string.IsNullOrWhiteSpace(item.Flags[0].Item.DefaultValue))
                    result += "--" + item.Flags[0].Item.Name + "=<" + item.Flags[0].Item.ItemType + "> ";
                else
                    result += "--" + item.Flags[0].Item.Name + "=<" + item.Flags[0].Item.DefaultValue + "> ";
            }

            if (item.Flags.Count > 1)
                result += "[<flags>] ";
            foreach (var argument in item.Arguments)
                result += $"<{argument.Item.Name}> ";
            return result;
        }


    }

}

namespace System.CodeDom.Compiler
{
    public class CompilerErrorCollection : List<CompilerError>
    {
    }

    public class CompilerError
    {
        public string ErrorText { get; set; }

        public bool IsWarning { get; set; }
    }
}
