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
            AddFlag("help", "Show context-sensitive help").Short('h').IsBool().Action(x => GenerateHelp(x));
            //Flag("completion-script-bash", "Generate completion script for bash.").IsHidden().Action(a.generateBashCompletionScript).Bool()
            //Flag("completion-script-zsh", "Generate completion script for ZSH.").IsHidden().Action(a.generateZSHCompletionScript).Bool()
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

        public static Command AddCommand(string name, string help)
        {
            return _application.Command(name, help);
        }

        public static Flag AddFlag(string name, string help)
        {
            return _application.Flag(name, help);
        }
        public static Argument AddArgument(string name, string help)
        {
            return _application.Argument(name, help);
        }

        int _current = 0;

        public static IDictionary<string, string> Parse(IEnumerable<string> args)
        {
            var result = new Dictionary<string, string>();

            var parser = new Parser(_application);
            return parser.Parse(args);
        }

        public static IDictionary<string, string> Parse(List<Command> commands, List<Flag> flags,
            List<Argument> arguments, IEnumerable<string> args)
        {
            var result = new Dictionary<string, string>();

            var parser = new Parser(_application);
            return parser.Parse(args);
        }
    }
}
