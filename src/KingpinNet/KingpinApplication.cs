using System;
using System.Collections.Generic;
using System.Linq;

namespace KingpinNet
{
    public class KingpinApplication
    {
        private List<CommandItem> _commands = new List<CommandItem>();
        private List<IItem> _flags = new List<IItem>();
        private List<IItem> _arguments = new List<IItem>();

        public IEnumerable<CommandItem> Commands => _commands;
        public IEnumerable<IItem> Flags => _flags;
        public IEnumerable<IItem> Arguments => _arguments;

        private IHelpTemplate _applicationHelp;
        private IHelpTemplate _commandHelp;
        public string Name { get; internal set; }
        public string Help { get; internal set; }
        public string VersionString { get; private set; }
        public string AuthorName { get; private set; }
        public bool HelpShownOnParsingErrors { get; private set; }
        public bool ExitOnParseErrors { get; private set; }
        public bool ExitWhenHelpIsShown { get; internal set; }
        public void Initialize()
        {
            _applicationHelp = new ApplicationHelp();
            _commandHelp = new CommandHelp();
            Flag("help", "Show context-sensitive help").Short('h').IsBool().Action(x => GenerateHelp());
        }
        public void GenerateHelp()
        {
            var helpGenerator = new HelpGenerator(this);
            helpGenerator.Generate(Console.Out, _applicationHelp);
        }
        private void GenerateCommandHelp(CommandItem command)
        {
            var helpGenerator = new HelpGenerator(this);
            helpGenerator.Generate(command, Console.Out, _commandHelp);
        }

        public CommandItem Command(string name, string help)
        {
            var result = new CommandItem(name, name, help);
            _commands.Add(result);
            return result;
        }

        public FlagItem<string> Flag(string name, string help)
        {
            var result = new FlagItem<string>(name, name, help);
            _flags.Add(result);
            return result;
        }
        public ArgumentItem<string> Argument(string name, string help)
        {
            var result = new ArgumentItem<string>(name, name, help);
            _arguments.Add(result);
            return result;
        }

        public FlagItem<T> Flag<T>(string name, string help)
        {
            var result = new FlagItem<T>(name, name, help,
                ValueTypeConverter.Convert(typeof(T)));
            _flags.Add(result);
            return result;
        }
        public ArgumentItem<T> Argument<T>(string name, string help)
        {
            var result = new ArgumentItem<T>(name, name, help,
                ValueTypeConverter.Convert(typeof(T)));
            _arguments.Add(result);
            return result;
        }

        internal KingpinApplication ExitOnParsingErrors()
        {
            ExitOnParseErrors = true;
            return this;
        }

        internal KingpinApplication ExitOnHelp()
        {
            ExitWhenHelpIsShown = true;
            return this;
        }

        internal KingpinApplication ShowHelpOnParsingErrors()
        {
            HelpShownOnParsingErrors = true;
            return this;
        }



        public void AddCommandHelpOnAllCommands()
        {
            AddCommandHelpOnAllCommands(_commands);
        }

        private void AddCommandHelpOnAllCommands(IEnumerable<CommandItem> commands)
        {
            foreach (var command in commands)
            {
                if (command.Commands.Count() > 0)
                    AddCommandHelpOnAllCommands(command.Commands);
                else
                    Flag<string>("help", "Show context-sensitive help").IsHidden().Short('h').IsBool().Action(x => GenerateCommandHelp(command));
            }
        }

        public IDictionary<string, string> Parse(IEnumerable<string> args)
        {
            var parser = new Parser(this);
            AddCommandHelpOnAllCommands();
            try
            {
                return parser.Parse(args);
            }
            catch (ParseException exception)
            {
                if (HelpShownOnParsingErrors)
                {
                    Console.WriteLine(exception.Message);
                    foreach (var error in exception.Errors)
                        Console.WriteLine($"   {error}");
                    GenerateHelp();
                }
                throw;
            }
        }

        public KingpinApplication Author(string author)
        {
            AuthorName = author;
            return this;
        }

        public KingpinApplication Version(string version)
        {
            VersionString = version;
            return this;
        }

        public KingpinApplication ApplicationHelp(string text)
        {
            Help = text;
            return this;
        }

        public KingpinApplication ApplicationName(string name)
        {
            Name = name;
            return this;
        }

        public KingpinApplication Template(IHelpTemplate applicationHelp, IHelpTemplate commandHelp)
        {
            _applicationHelp = applicationHelp;
            _commandHelp = commandHelp;
            return this;
        }
    }
}
