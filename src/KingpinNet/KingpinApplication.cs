using System;
using System.Collections.Generic;
using System.Linq;

namespace KingpinNet
{
    public class KingpinApplication
    {
        private List<CommandBuilder> _commands = new List<CommandBuilder>();
        private List<CommandLineItemBuilder<string>> _flags = new List<CommandLineItemBuilder<string>>();
        private List<CommandLineItemBuilder<string>> _arguments = new List<CommandLineItemBuilder<string>>();

        public IEnumerable<CommandBuilder> Commands => _commands;
        public IEnumerable<CommandLineItemBuilder<string>> Flags => _flags;
        public IEnumerable<CommandLineItemBuilder<string>> Arguments => _arguments;

        private IHelpTemplate _applicationHelp;
        private IHelpTemplate _commandHelp;
        public string Name { get; internal set; }
        public string Help { get; internal set; }
        public string VersionString { get; private set; }
        public string AuthorName { get; private set; }
        public bool HelpShownOnParsingErrors { get; private set; }
        public bool ExitOnParseErrors { get; private set; }
        public bool ExitWhenHelpIsShown { get; internal set; }
        public void Initialize()
        {
            _applicationHelp = new ApplicationHelp();
            _commandHelp = new CommandHelp();
            Flag("help", "Show context-sensitive help").Short('h').IsBool().Action(x => GenerateHelp());
        }
        public void GenerateHelp()
        {
            var helpGenerator = new HelpGenerator(this);
            helpGenerator.Generate(Console.Out, _applicationHelp);
        }
        private void GenerateCommandHelp(CommandBuilder command)
        {
            var helpGenerator = new HelpGenerator(this);
            helpGenerator.Generate(command, Console.Out, _commandHelp);
        }

        public CommandBuilder Command(string name, string help)
        {
            var result = new CommandBuilder(name, help);
            _commands.Add(result);
            return result;
        }

        public CommandLineItemBuilder<string> Flag(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help, ItemType.Flag);
            _flags.Add(result);
            return result;
        }
        public CommandLineItemBuilder<string> Argument(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help, ItemType.Argument);
            _arguments.Add(result);
            return result;
        }

        public CommandLineItemBuilder<string> Flag<T>(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help, ItemType.Flag,
                ValueTypeConverter.Convert(typeof(T)));
            _flags.Add(result);
            return result;
        }
        public CommandLineItemBuilder<string> Argument<T>(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help, ItemType.Argument,
                ValueTypeConverter.Convert(typeof(T)));
            _arguments.Add(result);
            return result;
        }

        internal KingpinApplication ExitOnParsingErrors()
        {
            ExitOnParseErrors = true;
            return this;
        }

        internal KingpinApplication ExitOnHelp()
        {
            ExitWhenHelpIsShown = true;
            return this;
        }

        internal KingpinApplication ShowHelpOnParsingErrors()
        {
            HelpShownOnParsingErrors = true;
            return this;
        }



        public void AddCommandHelpOnAllCommands()
        {
            AddCommandHelpOnAllCommands(_commands);
        }

        private void AddCommandHelpOnAllCommands(IEnumerable<CommandBuilder> commands)
        {
            foreach (var command in commands)
            {
                if (command.Item.Commands.Count() > 0)
                    AddCommandHelpOnAllCommands(command.Item.Commands);
                else
                    Flag("help", "Show context-sensitive help").IsHidden().Short('h').IsBool().Action(x => GenerateCommandHelp(command));
            }
        }

        public KingpinApplication Author(string author)
        {
            AuthorName = author;
            return this;
        }

        public KingpinApplication Version(string version)
        {
            VersionString = version;
            return this;
        }

        public KingpinApplication ApplicationHelp(string text)
        {
            Help = text;
            return this;
        }

        public KingpinApplication ApplicationName(string name)
        {
            Name = name;
            return this;
        }

        public KingpinApplication Template(IHelpTemplate applicationHelp, IHelpTemplate commandHelp)
        {
            _applicationHelp = applicationHelp;
            _commandHelp = commandHelp;
            return this;
        }
    }
}
