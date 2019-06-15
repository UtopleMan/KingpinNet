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
            _application.Initialize();
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

        public static CommandBuilder Command(string name, string help)
        {
            return _application.Command(name, help);
        }

        public static CommandLineItemBuilder<string> Flag(string name, string help)
        {
            return _application.Flag(name, help);
        }
        public static CommandLineItemBuilder<string> Flag<T>(string name, string help)
        {
            return _application.Flag<T>(name, help);
        }

        public static CommandLineItemBuilder<string> Argument(string name, string help)
        {
            return _application.Argument(name, help);
        }
        public static CommandLineItemBuilder<string> Argument<T>(string name, string help)
        {
            return _application.Argument<T>(name, help);
        }
        public static IDictionary<string, string> Parse(IEnumerable<string> args)
        {
            var parser = new Parser(_application);
            _application.AddCommandHelpOnAllCommands();
            try
            {
                return parser.Parse(args);
            }
            catch (ParseException exception)
            {
                if (_application.ShowHelpOnParsingErrors)
                {
                    Console.WriteLine(exception.Message);
                    _application.GenerateHelp("");
                }
                throw;
            }
        }

        public static IDictionary<string, string> Parse(List<CommandBuilder> commands, List<CommandLineItemBuilder<string>> flags,
            List<CommandLineItemBuilder<string>> arguments, IEnumerable<string> args)
        {
            var parser = new Parser(_application);
            return parser.Parse(args);
        }

    }
}
