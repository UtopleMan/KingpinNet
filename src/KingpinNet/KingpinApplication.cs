using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KingpinNet
{
    public class KingpinApplication
    {
        private List<CommandCategory> _categories = new List<CommandCategory>();
        private List<CommandItem> _commands = new List<CommandItem>();
        private List<IItem> _flags = new List<IItem>();
        private List<IItem> _arguments = new List<IItem>();

        // Used in the parser
        internal Action<Serverity, string, Exception> log = (a, b, c) => { };
        internal string exeFileName;
        internal string exeFileExtension;
        private string applicationHelpResource = "Phoenix.Shared.CommandLine.KingpinNet.Help.ApplicationHelp.liquid";
        private string commandHelpResource = "Phoenix.Shared.CommandLine.KingpinNet.Help.CommandHelp.liquid";

        public IEnumerable<CommandCategory> Categories => _categories;
        public IEnumerable<CommandItem> Commands => _commands;
        public IEnumerable<IItem> Flags => _flags;
        public IEnumerable<IItem> Arguments => _arguments;

        public string Name { get; private set; }
        public string HelpText { get; private set; }
        public string VersionString { get; private set; }
        public string AuthorName { get; private set; }
        public bool HelpShownOnParsingErrors { get; private set; }
        public bool ExitOnParseErrors { get; private set; }
        public bool ExitWhenHelpIsShown { get; private set; }

        public KingpinApplication()
        {
            exeFileName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
            exeFileExtension = Path.GetExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
        }
        public void Initialize()
        {
            Flag("help", "Show context-sensitive help").Short('h').IsBool().Action(x => GenerateHelp());
            Flag<bool>("suggestion-script-bash").IsHidden().Action(x => GenerateScript("bash.sh"));
            Flag<bool>("suggestion-script-zsh").IsHidden().Action(x => GenerateScript("zsh.sh"));
            Flag<bool>("suggestion-script-pwsh").IsHidden().Action(x => GenerateScript("pwsh.ps1"));
        }

        private void GenerateScript(string resource)
        {
            var content = GetResource(resource)
                .Replace("{{AppPath}}", Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar)
                .Replace("{{AppName}}", exeFileName)
                .Replace("{{AppExtension}}", exeFileExtension);

            Console.Out.Write(content.Replace("\r", ""));
            Environment.Exit(0);
        }
        private string GetResource(string name)
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream($"KingpinNet.Scripts.{name}");
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public void GenerateHelp()
        {
            var helpGenerator = new HelpGenerator(this);
            helpGenerator.GenerateWithLiquid(Console.Out, applicationHelpResource);
                                                           
            if (ExitWhenHelpIsShown)
            {
                Environment.Exit(0);
            }
        }
        private void GenerateCommandHelp(CommandItem command)
        {
            var helpGenerator = new HelpGenerator(this);    
            helpGenerator.GenerateWithLiquid(command, Console.Out, commandHelpResource);
            if (ExitWhenHelpIsShown)
            {
                Environment.Exit(0);
            }
        }

        internal void AddCommand(CommandItem result)
        {
            _commands.Add(result);
        }

        public CommandCategory Category(string name, string description)
        {
            var category = new CommandCategory(this, name, description);
            _categories.Add(category);
            return category;
        }

        public CommandItem Command(string name, string help = "")
        {
            var result = new CommandItem(name, name, help);
            _commands.Add(result);
            return result;
        }

        public FlagItem<string> Flag(string name, string help = "")
        {
            var result = new FlagItem<string>(name, name, help);
            _flags.Add(result);
            return result;
        }
        public ArgumentItem<string> Argument(string name, string help = "")
        {
            var result = new ArgumentItem<string>(name, name, help);
            _arguments.Add(result);
            return result;
        }

        public FlagItem<T> Flag<T>(string name, string help = "")
        {
            var result = new FlagItem<T>(name, name, help,
                ValueTypeConverter.Convert(typeof(T)));
            _flags.Add(result);
            return result;
        }
        public ArgumentItem<T> Argument<T>(string name, string help = "")
        {
            var result = new ArgumentItem<T>(name, name, help,
                ValueTypeConverter.Convert(typeof(T)));
            _arguments.Add(result);
            return result;
        }

        public KingpinApplication ExitOnParsingErrors()
        {
            ExitOnParseErrors = true;
            return this;
        }

        public KingpinApplication ExitOnHelp()
        {
            ExitWhenHelpIsShown = true;
            return this;
        }

        public KingpinApplication ShowHelpOnParsingErrors()
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
                //else
                command.Flag<string>("help", "Show context-sensitive help").IsHidden().Short('h').IsBool().Action(x => GenerateCommandHelp(command));
            }
        }

        public ParseResult Parse(IEnumerable<string> args)
        {
            var parser = new Parser(this, new CommandLineTokenizer());
            AddCommandHelpOnAllCommands();
            try
            {
                var result = parser.Parse(args);
                InvestigateSuggestions(result);
                return result;
            }
            catch (ParseException exception)
            {
                Console.WriteLine(exception.Message);
                foreach (var error in exception.Errors)
                    Console.WriteLine($"   {error}");

                if (HelpShownOnParsingErrors)
                {
                    GenerateHelp();
                }
                if (ExitOnParseErrors)
                    Environment.Exit(-1);
                throw;
            }
        }

        private void InvestigateSuggestions(ParseResult result)
        {
            if (result.IsSuggestion)
            {
                foreach (var suggestion in result.Suggestions)
                    Console.Out.WriteLine(suggestion);
                Environment.Exit(0);
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

        public KingpinApplication Help(string text)
        {
            HelpText = text;
            return this;
        }

        public KingpinApplication ApplicationName(string name)
        {
            Name = name;
            return this;
        }

        public KingpinApplication Template(string applicationHelpResource, string commandHelpResource)
        {
            this.applicationHelpResource = applicationHelpResource;
            this.commandHelpResource = commandHelpResource;
            return this;
        }
        public KingpinApplication Log(Action<Serverity, string, Exception> log)
        {
            this.log = log;
            return this;
        }
    }

    public enum Serverity
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error
    }
}
