using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class KingpinApplication
    {
        private readonly List<CommandBuilder> _commands = new List<CommandBuilder>();
        private readonly List<CommandLineItemBuilder<string>> _flags = new List<CommandLineItemBuilder<string>>();
        private readonly List<CommandLineItemBuilder<string>> _arguments = new List<CommandLineItemBuilder<string>>();

        public IEnumerable<CommandBuilder> Commands => _commands;
        public IEnumerable<CommandLineItemBuilder<string>> Flags => _flags;
        public IEnumerable<CommandLineItemBuilder<string>> Arguments => _arguments;

        public string Name;


        public void Initialize()
        {
            Flag("help", "Show context-sensitive help").Short('h').IsBool().Action(x => GenerateHelp(x));
        }

        public string Help;
        public string Version;
        public string Author;
        public bool ShowHelpOnParsingErrors;

        public void GenerateHelp(string argument)
        {
            var helpGenerator = new HelpGenerator(this);
            helpGenerator.Generate(Console.Out);
        }
        private void GenerateCommandHelp(CommandBuilder command, string argument)
        {
            var helpGenerator = new HelpGenerator(this);
            helpGenerator.Generate(command, Console.Out);
        }
        public CommandBuilder Command(string name, string help)
        {
            var result = new CommandBuilder(name, help);
            _commands.Add(result);
            return result;
        }
        public CommandLineItemBuilder<string> Flag(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help);
            _flags.Add(result);
            return result;
        }
        public CommandLineItemBuilder<string> Flag<T>(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help);
            result.Item.ValueType = ValueTypeConverter.Convert(typeof(T));
            _flags.Add(result);
            return result;
        }
        public CommandLineItemBuilder<string> Argument(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help);
            _arguments.Add(result);
            return result;
        }
        public CommandLineItemBuilder<string> Argument<T>(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help);
            result.Item.ValueType = ValueTypeConverter.Convert(typeof(T));
            _arguments.Add(result);
            return result;
        }

        public void AddCommandHelpOnAllCommands()
        {
            AddCommandHelpOnAllCommands(_commands);
        }

        private void AddCommandHelpOnAllCommands(List<CommandBuilder> commands)
        {
            foreach (var command in commands)
            {
                if (command.Item.Commands.Count > 0)
                    AddCommandHelpOnAllCommands(command.Item.Commands);
                else
                    Flag("help", "Show context-sensitive help").IsHidden().Short('h').IsBool().Action(x => GenerateCommandHelp(command, x));
            }
        }
    }
}
