using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class KingpinApplication
    {
        public readonly List<CommandItem> Commands = new List<CommandItem>();
        public readonly List<FlagItem> Flags = new List<FlagItem>();
        public readonly List<ArgumentItem> Arguments = new List<ArgumentItem>();
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
        private void GenerateCommandHelp(CommandItem command)
        {
            var helpGenerator = new HelpGenerator(this);
            helpGenerator.Generate(command, Console.Out, _commandHelp);
        }

        public CommandItem Command(string name, string help)
        {
            var result = new CommandItem(name, help);
            Commands.Add(result);
            return result;
        }

        public FlagItem Flag(string name, string help)
        {
            var result = new FlagItem(name, help);
            Flags.Add(result);
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

        public ArgumentItem Argument(string name, string help)
        {
            var result = new ArgumentItem(name, help);
            Arguments.Add(result);
            return result;
        }

        public void AddCommandHelpOnAllCommands(List<CommandItem> commands)
        {
            foreach (var command in commands)
            {
                if (command.Item.Commands.Count > 0)
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
