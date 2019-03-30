using System.Collections.Generic;

namespace KingpinNet
{
    public class KingpinApplication
    {
        public readonly List<CommandBuilder> Commands = new List<CommandBuilder>();
        public readonly List<FlagBuilder> Flags = new List<FlagBuilder>();
        public readonly List<ArgumentBuilder> Arguments = new List<ArgumentBuilder>();
        public string Name;
        public string Help;
        public string Version;

        public CommandBuilder Command(string name, string help)
        {
            var result = new CommandBuilder(name, help);
            Commands.Add(result);
            return result;
        }

        public FlagBuilder Flag(string name, string help)
        {
            var result = new FlagBuilder(name, help);
            Flags.Add(result);
            return result;
        }
        public ArgumentBuilder Argument(string name, string help)
        {
            var result = new ArgumentBuilder(name, help);
            Arguments.Add(result);
            return result;
        }
    }
}
