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
        public bool ShowHelpOnParsingErrors;
        public bool ExitOnParsingErrors;

        public bool ExitOnHelp { get; internal set; }

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
    }
}
