using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class Kingpin
    {
        private static readonly List<CommandBuilder> _commands = new List<CommandBuilder>();
        private static readonly List<FlagBuilder> _flags = new List<FlagBuilder>();
        private static readonly List<ArgumentBuilder> _arguments = new List<ArgumentBuilder>();
        private static string _name;
        private static string _help;

        public static CommandBuilder Command(string name, string help)
        {
            _name = name;
            _help = help;
            var result = new CommandBuilder(name, help);
            _commands.Add(result);
            return result;
        }

        public static FlagBuilder Flag(string name, string help)
        {
            var result = new FlagBuilder(name, help);
            _flags.Add(result);
            return result;
        }
        public static ArgumentBuilder Argument(string name, string help)
        {
            var result = new ArgumentBuilder(name, help);
            _arguments.Add(result);
            return result;
        }

        int _current = 0;

        public static IDictionary<string, string> Parse(IEnumerable<string> args)
        {
            var result = new Dictionary<string, string>();

            var parser = new Parser(_commands, _flags, _arguments);
            return parser.Parse(args);
        }

        public static IDictionary<string, string> Parse(List<CommandBuilder> commands, List<FlagBuilder> flags,
            List<ArgumentBuilder> arguments, IEnumerable<string> args)
        {
            var result = new Dictionary<string, string>();

            var parser = new Parser(commands, flags, arguments);
            return parser.Parse(args);
        }

        public enum TokenType
        {
            Flag,

        }

    }
}
