using System.Collections.Generic;

namespace KingpinNet
{
    public class KingpinApplication
    {
        public readonly List<Command> Commands = new List<Command>();
        public readonly List<Flag> Flags = new List<Flag>();
        public readonly List<Argument> Arguments = new List<Argument>();
        public string Name;
        public string Help;
        public string Version;
        public string Author;

        public Command Command(string name, string help)
        {
            var result = new Command(name, help);
            Commands.Add(result);
            return result;
        }

        public Flag Flag(string name, string help)
        {
            var result = new Flag(name, help);
            Flags.Add(result);
            return result;
        }
        public Argument Argument(string name, string help)
        {
            var result = new Argument(name, help);
            Arguments.Add(result);
            return result;
        }
    }
}
