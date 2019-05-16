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
            _application.Initialize();
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
                    _application.GenerateHelp("");
                }
                throw;
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
