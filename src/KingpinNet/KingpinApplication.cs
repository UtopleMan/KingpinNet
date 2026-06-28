using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KingpinNet.AiHelp;

namespace KingpinNet;

public class KingpinApplication
{
    internal const string AiHelpFileSentinel = "__ai_help_default_location__";
    private readonly List<Avoid> _avoids = new();
    private readonly List<Example> _examples = new();
    private readonly List<ExitCode> _exitCodes = new();
    private readonly List<Note> _notes = new();
    private readonly List<Prefer> _prefers = new();
    private readonly IConsole console;
    private readonly List<IItem> _arguments = new();
    private readonly List<CommandCategory> _categories = new();
    private readonly List<CommandItem> _commands = new();
    private readonly List<IItem> _flags = new();
    private string applicationHelpResource = "KingpinNet.Help.ApplicationHelp.liquid";
    private string commandHelpResource = "KingpinNet.Help.CommandHelp.liquid";
    internal string exeFileExtension;
    internal string exeFileName;

    // Used in the parser
    internal Action<Serverity, string, Exception> log = (a, b, c) => { };

    public KingpinApplication()
    {
        var processPath = Environment.ProcessPath ?? "";
        exeFileName = Path.GetFileNameWithoutExtension(processPath);
        exeFileExtension = Path.GetExtension(processPath);
        console = new DefaultConsole();
    }

    public KingpinApplication(IConsole console)
    {
        var processPath = Environment.ProcessPath ?? "";
        exeFileName = Path.GetFileNameWithoutExtension(processPath);
        exeFileExtension = Path.GetExtension(processPath);
        this.console = console;
    }

    public IReadOnlyList<CommandCategory> Categories => _categories;
    public IReadOnlyList<CommandItem> Commands => _commands;
    public IReadOnlyList<IItem> Flags => _flags;
    public IReadOnlyList<IItem> Arguments => _arguments;
    public IReadOnlyList<ExitCode> ExitCodes => _exitCodes;
    public IReadOnlyList<Example> Examples => _examples;
    public IReadOnlyList<Note> Notes => _notes;
    public IReadOnlyList<Prefer> Prefers => _prefers;
    public IReadOnlyList<Avoid> Avoids => _avoids;

    public string Name { get; private set; }
    public string HelpText { get; private set; }
    public string VersionString { get; private set; }
    public string AuthorName { get; private set; }
    public bool HelpShownOnParsingErrors { get; private set; }
    public bool ExitOnParsingErrors { get; private set; }
    public bool ThrowOnParsingErrors { get; private set; }
    public bool ParsingErrorsShown { get; private set; } = true;
    public bool ExitWhenHelpIsShown { get; private set; }
    public bool HelpShownOnNoArguments { get; private set; }
    public bool ExitWhenNoArguments { get; private set; }

    public void Initialize()
    {
        Flag("help", "Show context-sensitive help").Short('h').IsBool().Action(x => GenerateHelp());
        Flag<bool>("help-ai").IsHidden().Action(x => GenerateAiHelp(null));
        Flag<string>("help-ai-file").IsHidden().Action(path => GenerateAiHelpFile(path, null));
        Flag<bool>("suggestion-script-bash").IsHidden().Action(x => GenerateScript("bash.sh"));
        Flag<bool>("suggestion-script-zsh").IsHidden().Action(x => GenerateScript("zsh.sh"));
        Flag<bool>("suggestion-script-pwsh").IsHidden().Action(x => GenerateScript("pwsh.ps1"));
    }

    private void GenerateScript(string resource)
    {
        var processPath = Environment.ProcessPath ?? "";
        var content = GetResource(resource)
            .Replace("{{AppPath}}", Path.GetDirectoryName(processPath) + Path.DirectorySeparatorChar)
            .Replace("{{AppName}}", exeFileName)
            .Replace("{{AppExtension}}", exeFileExtension);

        console.Out.Write(content.Replace("\r", ""));
        Environment.Exit(0);
    }

    private string GetResource(string name)
    {
        var stream = GetType().Assembly.GetManifestResourceStream($"KingpinNet.Scripts.{name}");
        var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public void GenerateHelp()
    {
        var helpGenerator = new HelpGenerator(this, console);
        var liquidText = helpGenerator.ReadResourceInExecutingAssembly(applicationHelpResource);
        helpGenerator.Generate(console.Out, liquidText);

        if (ExitWhenHelpIsShown) Environment.Exit(0);
    }

    private void GenerateCommandHelp(CommandItem command)
    {
        var helpGenerator = new HelpGenerator(this, console);
        var liquidText = helpGenerator.ReadResourceInExecutingAssembly(commandHelpResource);
        helpGenerator.Generate(command, console.Out, liquidText);
        if (ExitWhenHelpIsShown) Environment.Exit(0);
    }

    internal void GenerateAiHelp(CommandItem scope)
    {
        var writer = new AiHelpYamlWriter(console.Out);
        writer.Write(this, scope);
        if (ExitWhenHelpIsShown) Environment.Exit(0);
    }

    internal void GenerateAiHelpFile(string path, CommandItem scope)
    {
        var resolved = ResolveAiHelpFilePath(path);
        var displayName = !string.IsNullOrEmpty(Name) ? Name : exeFileName;
        if (string.IsNullOrEmpty(displayName)) displayName = "app";

        using (var file = new StreamWriter(resolved))
        {
            file.Write("# ");
            file.Write(displayName);
            file.WriteLine(" help");
            file.WriteLine();
            file.WriteLine("```yaml");
            var writer = new AiHelpYamlWriter(file);
            writer.Write(this, scope);
            file.WriteLine("```");
        }

        console.Out.WriteLine($"Wrote AI help to {resolved}");
        if (ExitWhenHelpIsShown) Environment.Exit(0);
    }

    private string ResolveAiHelpFilePath(string requested)
    {
        if (!string.IsNullOrWhiteSpace(requested) && requested != AiHelpFileSentinel)
            return requested;

        var displayName = !string.IsNullOrEmpty(Name) ? Name : exeFileName;
        if (string.IsNullOrEmpty(displayName)) displayName = "app";

        var processPath = Environment.ProcessPath ?? "";
        var dir = !string.IsNullOrEmpty(processPath)
            ? Path.GetDirectoryName(processPath)
            : AppContext.BaseDirectory;
        if (string.IsNullOrEmpty(dir)) dir = Directory.GetCurrentDirectory();

        return Path.Combine(dir, displayName + "-ai-help.md");
    }

    private static void ExpandBareHelpAiFile(List<string> args)
    {
        for (var i = 0; i < args.Count; i++)
            if (args[i] == "--help-ai-file")
                args[i] = "--help-ai-file=" + AiHelpFileSentinel;
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

    public KingpinApplication ExitOnParseErrors()
    {
        ExitOnParsingErrors = true;
        return this;
    }

    public KingpinApplication HideParseErrors()
    {
        ParsingErrorsShown = false;
        return this;
    }

    public KingpinApplication ThrowOnParseErrors()
    {
        ExitOnParsingErrors = true;
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

    public KingpinApplication ShowHelpOnNoArguments()
    {
        HelpShownOnNoArguments = true;
        return this;
    }

    public KingpinApplication ExitOnNoArguments()
    {
        ExitWhenNoArguments = true;
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
            var scope = command;
            command.Flag<string>("help", "Show context-sensitive help").IsHidden().Short('h').IsBool()
                .Action(x => GenerateCommandHelp(scope));
            command.Flag<bool>("help-ai").IsHidden().Action(x => GenerateAiHelp(scope));
            command.Flag<string>("help-ai-file").IsHidden().Action(path => GenerateAiHelpFile(path, scope));
        }
    }

    public ParseResult Parse(IEnumerable<string> args)
    {
        var argsList = args?.ToList() ?? new List<string>();
        if (argsList.Count == 0)
        {
            if (HelpShownOnNoArguments)
                GenerateHelp();
            if (ExitWhenNoArguments)
                Environment.Exit(-1);
        }

        ExpandBareHelpAiFile(argsList);

        var parser = new Parser(this, new CommandLineTokenizer());
        AddCommandHelpOnAllCommands();
        try
        {
            var result = parser.Parse(argsList);
            InvestigateSuggestions(result);
            return result;
        }
        catch (ParseException exception)
        {
            if (ParsingErrorsShown)
            {
                console.Out.WriteLine(exception.Message);
                foreach (var error in exception.Errors)
                    console.Out.WriteLine($"   {error}");
            }

            if (HelpShownOnParsingErrors)
                GenerateHelp();
            if (ExitOnParsingErrors)
                Environment.Exit(-1);
            if (ThrowOnParsingErrors)
                throw;
            return new ParseResult
                { ParsingFailed = true, ErrorMessage = exception.Message, Errors = exception.Errors };
        }
    }

    private void InvestigateSuggestions(ParseResult result)
    {
        if (result.IsSuggestion)
        {
            foreach (var suggestion in result.Suggestions)
                console.Out.WriteLine(suggestion);
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

    public KingpinApplication ExitCode(int code, string description)
    {
        _exitCodes.Add(new ExitCode(code, description));
        return this;
    }

    public KingpinApplication Example(string intent, string command)
    {
        _examples.Add(new Example(intent, command));
        return this;
    }

    public KingpinApplication Note(string text)
    {
        _notes.Add(new Note(text));
        return this;
    }

    public KingpinApplication Prefer(string rule, string when, string why)
    {
        _prefers.Add(new Prefer(rule, when, why));
        return this;
    }

    public KingpinApplication Avoid(string rule, string unless, string why)
    {
        _avoids.Add(new Avoid(rule, unless, why));
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
