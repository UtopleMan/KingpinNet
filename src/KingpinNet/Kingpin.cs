using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class Kingpin
    {
        private static KingpinApplication _application = new KingpinApplication();

        static Kingpin()
        {
            _application.Name = System.AppDomain.CurrentDomain.FriendlyName;
            _application.Help = "";
            Flag("help", "Show context-sensitive help").Short('h').IsBool().Action(x => GenerateHelp(x));
            //Flag("completion-script-bash", "Generate completion script for bash.").IsHidden().Action(a.generateBashCompletionScript).Bool()
            //Flag("completion-script-zsh", "Generate completion script for ZSH.").IsHidden().Action(a.generateZSHCompletionScript).Bool()
        }

        public static void ShowHelpOnException( )
        {
            _application.ShowHelpOnException = true;
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
            try
            {
                return parser.Parse(args);
            }
            catch (ParseException exception)
            {
                if (_application.ShowHelpOnException)
                {
                    Console.WriteLine(exception.Message);
                    GenerateHelp("");
                }
                if (_application.ExitOnException)
                {
                    Environment.Exit(1);
                }
                return new Dictionary<string, string>() { { "KingpinError", exception.Message } };
            }
        }


        public static IDictionary<string, string> Parse(List<CommandItem> commands, List<FlagItem> flags,
            List<ArgumentItem> arguments, IEnumerable<string> args)
        {
            var result = new Dictionary<string, string>();

            var parser = new Parser(_application);
            return parser.Parse(args);
        }

        public static void ExitOnException()
        {
            _application.ExitOnException = true;
        }
    }
}
