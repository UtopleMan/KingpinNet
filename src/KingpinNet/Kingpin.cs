using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class Kingpin
    {
        private static KingpinApplication _application = new KingpinApplication();

        static Kingpin()
        {
            _application.Name = AppDomain.CurrentDomain.FriendlyName;
            _application.Help = "";
            Flag("help", "Show context-sensitive help").Short('h').IsBool().Action(x => GenerateHelp(x));
            //Flag("completion-script-bash", "Generate completion script for bash.").IsHidden().Action(a.generateBashCompletionScript).Bool()
            //Flag("completion-script-zsh", "Generate completion script for ZSH.").IsHidden().Action(a.generateZSHCompletionScript).Bool()
        }

        public static void ShowHelpOnParsingErrors( )
        {
            _application.ShowHelpOnParsingErrors = true;
        }

        public static void Author(string author)
        {
            _application.Author = author;
        }

        public static void Version(string version)
        {
            _application.Version = version;
        }

        private static void GenerateHelp(string argument)
        {
            var helpGenerator = new HelpGenerator(_application);
            helpGenerator.Generate(Console.Out);
        }
        private static void GenerateCommandHelp(CommandItem command, string argument)
        {
            var helpGenerator = new HelpGenerator(_application);
            helpGenerator.Generate(command, Console.Out);
        }
        public static CommandItem Command(string name, string help)
        {
            return _application.Command(name, help);
        }

        public static FlagItem Flag(string name, string help)
        {
            return _application.Flag(name, help);
        }

        public static ArgumentItem Argument(string name, string help)
        {
            return _application.Argument(name, help);
        }

        public static IDictionary<string, string> Parse(IEnumerable<string> args)
        {
            var parser = new Parser(_application);
            AddCommandHelpOnAllCommands(_application.Commands);
            try
            {
                return parser.Parse(args);
            }
            catch (ParseException exception)
            {
                if (_application.ShowHelpOnParsingErrors)
                {
                    Console.WriteLine(exception.Message);
                    GenerateHelp("");
                }
                throw;
            }
        }

        private static void AddCommandHelpOnAllCommands(List<CommandItem> commands)
        {
            foreach (var command in commands)
            {
                if (command.Item.Commands.Count > 0)
                    AddCommandHelpOnAllCommands(command.Item.Commands);
                else
                    Flag("help", "Show context-sensitive help").IsHidden().Short('h').IsBool().Action(x => GenerateCommandHelp(command, x));
            }
        }

        public static IDictionary<string, string> Parse(List<CommandItem> commands, List<FlagItem> flags,
            List<ArgumentItem> arguments, IEnumerable<string> args)
        {
            var parser = new Parser(_application);
            return parser.Parse(args);
        }

    }
}
