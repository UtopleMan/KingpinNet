using KingpinNet.Help;
using KingpinNet.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace KingpinNet
{
    public class KingpinApplication
    {
        private readonly IConsole console;
        private List<CommandCategory> _categories = new List<CommandCategory>();
        private List<CommandItem> _commands = new List<CommandItem>();
        private List<IItem> _flags = new List<IItem>();
        private List<IItem> _arguments = new List<IItem>();

        internal Action<Serverity, string, Exception> log = (a, b, c) => { };
        internal string exeFileName;
        internal string exeFileExtension;

        public IEnumerable<CommandCategory> Categories => _categories;
        public IEnumerable<CommandItem> Commands => _commands;
        public IEnumerable<IItem> Flags => _flags;
        public IEnumerable<IItem> Arguments => _arguments;

        private IHelpTemplate applicationHelp;
        private IHelpTemplate commandHelp;
        private OptionsParser optionsParser;

        public string Name { get; internal set; }
        public string Help { get; internal set; }
        public string VersionString { get; private set; }
        public string AuthorName { get; private set; }
        public bool HelpShownOnParsingErrors { get; private set; }
        public bool ExitOnParseErrors { get; private set; }
        public bool ExitWhenHelpIsShown { get; internal set; }

        public KingpinApplication()
        {
            this.console = new UI.Console();
        }
        public KingpinApplication(IConsole console)
        {
            this.console = console;
            exeFileName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
            exeFileExtension = Path.GetExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        public void Initialize()
        {
            applicationHelp = new ApplicationHelp();
            commandHelp = new CommandHelp();
            optionsParser = new OptionsParser(this);
            Flag("help", "Show context-sensitive help").Short('h').IsBool().Run(x => GenerateHelp());
            Flag<bool>("completion-script-bash").IsHidden().Run(x => GenerateScript("bash.sh"));
            Flag<bool>("completion-script-zsh").IsHidden().Run(x => GenerateScript("zsh.sh"));
            Flag<bool>("completion-script-pwsh").IsHidden().Run(x => GenerateScript("pwsh.ps1"));
        }

        private void GenerateScript(string resource)
        {
            var content = GetResource(resource)
                .Replace("{{AppPath}}", Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar)
                .Replace("{{AppName}}", exeFileName)
                .Replace("{{AppExtension}}", exeFileExtension);

            console.Out.Write(content.Replace("\r", ""));
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
            helpGenerator.Generate(console.Out, applicationHelp);
            if (ExitWhenHelpIsShown)
            {
                Environment.Exit(0);
            }
        }
        private void GenerateCommandHelp(CommandItem command)
        {
            var helpGenerator = new HelpGenerator(this);    
            helpGenerator.Generate(command, console.Out, commandHelp);
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
        public KingpinApplication Options(Type optionsType)
        {
            optionsParser.Parse(optionsType);
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
                command.Flag<string>("help", "Show context-sensitive help").IsHidden().Short('h').IsBool().Run(x => GenerateCommandHelp(command));
            }
        }

        public ParseResult Parse(IEnumerable<string> args)
        {
            var parser = new Parser(this, new CommandLineTokenizer());
            AddCommandHelpOnAllCommands();
            try
            {
                var result = parser.Parse(args);
                InvestigateCompletions(result);
                return result;
            }
            catch (ParseException exception)
            {
                if (string.IsNullOrEmpty(exception.Suggestion))
                    console.WriteLine(exception.Message);
                else
                    console.WriteLine(exception.Message + $". Did you mean '{exception.Suggestion}'?");

                foreach (var error in exception.Errors)
                    console.WriteLine($"   {error}");

                if (HelpShownOnParsingErrors)
                {
                    GenerateHelp();
                }
                if (ExitOnParseErrors)
                    Environment.Exit(-1);
                throw;
            }
        }

        private void InvestigateCompletions(ParseResult result)
        {
            if (result.IsCompletion)
            {
                foreach (var completions in result.Completions)
                    console.Out.WriteLine(completions);
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
            this.applicationHelp = applicationHelp;
            this.commandHelp = commandHelp;
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
