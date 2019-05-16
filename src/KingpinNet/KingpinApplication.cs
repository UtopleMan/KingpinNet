using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class KingpinApplication
    {
        public readonly List<CommandItem> Commands = new List<CommandItem>();
        public readonly List<FlagItem> Flags = new List<FlagItem>();
        public readonly List<ArgumentItem> Arguments = new List<ArgumentItem>();
        public string Name { get; internal set; }
        public string Help { get; internal set; }
        public string VersionString { get; private set; }
        public string AuthorName { get; private set; }
        public bool HelpShownOnParsingErrors { get; private set; }
        public bool ExitOnParseErrors { get; private set; }
        public bool ExitWhenHelpIsShown { get; internal set; }
        public void Initialize()
        {
            Flag("help", "Show context-sensitive help").Short('h').IsBool().Action(GenerateHelp);
        }
        public void GenerateHelp(string argument)
        {
            var helpGenerator = new HelpGenerator(this);
            helpGenerator.Generate(Console.Out);
        }
        private void GenerateCommandHelp(CommandItem command, string argument)
        {
            var helpGenerator = new HelpGenerator(this);
            helpGenerator.Generate(command, Console.Out);
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
                    Flag("help", "Show context-sensitive help").IsHidden().Short('h').IsBool().Action(x => GenerateCommandHelp(command, x));
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
    }
}
