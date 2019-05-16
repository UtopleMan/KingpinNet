using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class Kingpin
    {
        private static readonly KingpinApplication Application = new KingpinApplication();

        static Kingpin()
        {
            Application.ApplicationName(AppDomain.CurrentDomain.FriendlyName);
            Application.ApplicationHelp("");
            Flag("help", "Show context-sensitive help").Short('h').IsBool().Action(x => GenerateHelp(x));
            //Flag("completion-script-bash", "Generate completion script for bash.").IsHidden().Action(a.generateBashCompletionScript).Bool()
            //Flag("completion-script-zsh", "Generate completion script for ZSH.").IsHidden().Action(a.generateZSHCompletionScript).Bool()
        }

        public static KingpinApplication ShowHelpOnParsingErrors()
        {
            return Application.ShowHelpOnParsingErrors();
        }

        public static KingpinApplication ExitOnParsingErrors()
        {
            return Application.ExitOnParsingErrors();
        }

        public static KingpinApplication ExitOnHelp()
        {
            return Application.ExitOnHelp();
        }

        public static KingpinApplication Author(string author)
        {
            return Application.Author(author);
        }

        public static KingpinApplication Version(string version)
        {
            return Application.Version(version);
        }

        private static void GenerateHelp(string argument)
        {
            var helpGenerator = new HelpGenerator(Application);
            helpGenerator.Generate(Console.Out);
            if (Application.ExitWhenHelpIsShown)
                Environment.Exit(0);
        }
        private static void GenerateCommandHelp(CommandItem command, string argument)
        {
            var helpGenerator = new HelpGenerator(Application);
            helpGenerator.Generate(command, Console.Out);
        }
        public static CommandItem Command(string name, string help)
        {
            return Application.Command(name, help);
        }

        public static FlagItem Flag(string name, string help)
        {
            return Application.Flag(name, help);
        }

        public static ArgumentItem Argument(string name, string help)
        {
            return Application.Argument(name, help);
        }

        public static IDictionary<string, string> Parse(IEnumerable<string> args)
        {
            var parser = new Parser(Application);
            AddCommandHelpOnAllCommands(Application.Commands);
            try
            {
                return parser.Parse(args);
            }
            catch (ParseException exception)
            {
                if (Application.HelpShownOnParsingErrors)
                {
                    Console.WriteLine(exception.Message);
                    GenerateHelp("");
                }
                if (Application.ExitOnParseErrors)
                {
                    Environment.Exit(1);
                }
                return new Dictionary<string, string>() { { "KingpinError", exception.Message } };
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
            var parser = new Parser(Application);
            return parser.Parse(args);
        }

    }
}
