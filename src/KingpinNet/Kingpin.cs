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
            //Flag("completion-script-bash", "Generate completion script for bash.").IsHidden().Action(a.generateBashCompletionScript).Bool()
            //Flag("completion-script-zsh", "Generate completion script for ZSH.").IsHidden().Action(a.generateZSHCompletionScript).Bool()
        }

        public static void ShowHelpOnParsingErrors( )
        {
            _application.ShowHelpOnParsingErrors = true;
        }

        public static void ExitOnParsingErrors()
        {
            _application.ExitOnParsingErrors = true;
        }

        public static void ExitOnHelp()
        {
            _application.ExitOnHelp = true;
        }

        public static void Author(string author)
        {
            _application.Author = author;
        }

        public static void Version(string version)
        {
            _application.Version = version;
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
            return _application.Parse(args);
        }
    }
}
