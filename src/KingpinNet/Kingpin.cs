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
            Flag("help", "Show context-sensitive help").Short('h').IsBool().Action(() => GenerateHelp());
            //Flag("completion-script-bash", "Generate completion script for bash.").IsHidden().Action(a.generateBashCompletionScript).Bool()
            //Flag("completion-script-zsh", "Generate completion script for ZSH.").IsHidden().Action(a.generateZSHCompletionScript).Bool()
        }

        private static void GenerateHelp()
        {
            var helpGenerator = new HelpGenerator(_application);
            helpGenerator.Generate(Console.Out);
        }

        public static CommandBuilder Command(string name, string help)
        {
            return _application.Command(name, help);
        }

        public static FlagBuilder Flag(string name, string help)
        {
            return _application.Flag(name, help);
        }
        public static ArgumentBuilder Argument(string name, string help)
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

        public static IDictionary<string, string> Parse(List<CommandBuilder> commands, List<FlagBuilder> flags,
            List<ArgumentBuilder> arguments, IEnumerable<string> args)
        {
            var result = new Dictionary<string, string>();

            var parser = new Parser(_application);
            return parser.Parse(args);
        }
    }
}
