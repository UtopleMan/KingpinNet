using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class KingpinApplication
    {
        public readonly List<CommandItem> Commands = new List<CommandItem>();
        public readonly List<FlagItem> Flags = new List<FlagItem>();
        public readonly List<ArgumentItem> Arguments = new List<ArgumentItem>();
        public string Name;
        public string Help;
        public string Version;
        public string Author;
        public bool ShowHelpOnParsingErrors { get; internal set; }
        public bool ExitOnParsingErrors { get; internal set; }
        public bool ExitOnHelp { get; internal set; }

        public void EnableHelp()
        {
            Flag("help", "Show context-sensitive help").Short('h').IsBool().Action(x => GenerateHelp(x));
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
        public ArgumentItem Argument(string name, string help)
        {
            var result = new ArgumentItem(name, help);
            Arguments.Add(result);
            return result;
        }

        public IDictionary<string, string> Parse(IEnumerable<string> args)
        {
            var parser = new Parser(this);
            AddCommandHelpOnAllCommands(Commands);
            try
            {
                return parser.Parse(args);
            }
            catch (ParseException exception)
            {
                if (this.ShowHelpOnParsingErrors)
                {
                    Console.WriteLine(exception.Message);
                    GenerateHelp("");
                }
                if (this.ExitOnParsingErrors)
                {
                    Environment.Exit(1);
                }
                return new Dictionary<string, string>() { { "KingpinError", exception.Message } };
            }
        }

        private void GenerateHelp(string argument)
        {
            var helpGenerator = new HelpGenerator(this);
            helpGenerator.Generate(Console.Out);
            if (ExitOnHelp)
                Environment.Exit(0);
        }

        private void AddCommandHelpOnAllCommands(List<CommandItem> commands)
        {
            foreach (var command in commands)
            {
                if (command.Item.Commands.Count > 0)
                    AddCommandHelpOnAllCommands(command.Item.Commands);
                else
                    Flag("help", "Show context-sensitive help").IsHidden().Short('h').IsBool().Action(x => GenerateCommandHelp(command, x));
            }
        }

        private void GenerateCommandHelp(CommandItem command, string argument)
        {
            var helpGenerator = new HelpGenerator(this);
            helpGenerator.Generate(command, Console.Out);
        }
    }
}
