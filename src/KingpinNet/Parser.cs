using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace KingpinNet;

public class Parser
{
    private readonly List<CommandItem> _commands;
    private readonly List<IItem> _globalFlags;
    private readonly List<IItem> _globalArguments;
    private readonly Action<Serverity, string, Exception> _logger;
    private readonly string _exeFileName;
    private readonly CommandLineTokenizer _commandLineTokenizer;

    public Parser(KingpinApplication application)
        : this(application, new CommandLineTokenizer()) { }

    public Parser(KingpinApplication application, CommandLineTokenizer commandLineTokenizer)
    {
        _commands = application.Commands.ToList();
        _globalFlags = application.Flags.ToList();
        _globalArguments = application.Arguments.ToList();
        _logger = application.log;
        _exeFileName = application.exeFileName;
        _commandLineTokenizer = commandLineTokenizer;
    }

    public ParseResult Parse(IEnumerable<string> args)
    {
        var ctx = new ParseContext { Args = args.ToList() };
        MainParse(ctx);
        return ctx.Result;
    }

    private sealed class ParseContext
    {
        public ParseResult Result { get; } = new ParseResult();
        public List<string> Args { get; set; } = new List<string>();
        public int CurrentItem { get; set; }
        public string SuggestionString { get; set; } = "";
    }

    private void Log(Serverity severity, string message, Exception exception = null)
    {
        if (_logger == null)
            return;
        try { _logger.Invoke(severity, message, exception); }
        catch { }
    }

    private void MainParse(ParseContext ctx)
    {
        SetDefaults(ctx);

        if (ctx.Args.Count > 0)
        {
            InvestigateSuggestions(ctx);
            while (ctx.CurrentItem < ctx.Args.Count)
            {
                if (IsCommand(ctx.Args[ctx.CurrentItem], _commands, out CommandItem commandFound))
                {
                    AddCommand("Command", commandFound, ctx);
                    ctx.CurrentItem++;
                    CommandFound(commandFound, ctx);
                }
                else if (IsFlag(ctx.Args[ctx.CurrentItem], new List<IItem>(), _globalFlags, ctx, out IItem flagFound))
                {
                    Add(flagFound, ctx);
                    ctx.CurrentItem++;
                }
                else if (IsArgument(ctx.Args[ctx.CurrentItem], _globalArguments, out IItem argumentFound) && !ctx.Result.IsSuggestion)
                {
                    Add(argumentFound, ctx);
                    ctx.CurrentItem++;
                }
                else
                {
                    if (!ctx.Result.IsSuggestion)
                        throw new ParseException($"Didn't expect argument {ctx.Args[ctx.CurrentItem]}");
                    ctx.SuggestionString = ctx.Args[ctx.CurrentItem];
                    ctx.CurrentItem++;
                }
            }
        }

        if (ctx.Result.IsSuggestion)
            ctx.Result.Suggestions.AddRange(ParseSuggestions(ctx.SuggestionString));
        else
            CheckAllRequiredItemsIsSet();
    }

    private void InvestigateSuggestions(ParseContext ctx)
    {
        if (!IsSuggestion(ctx.Args[0]))
            return;

        ctx.Args = ctx.Args.Count > 1 && IsPosition(ctx.Args[1])
            ? ctx.Args.Skip(3).ToList()
            : ctx.Args.Skip(1).ToList();

        ctx.Result.IsSuggestion = true;
        if (ctx.Args.Count == 0) return;

        var argsStr = ctx.Args.Aggregate((c, n) => c + " " + n);
        Log(Serverity.Info, $"Suggestion arguments: '{argsStr}'");
        argsStr = RemoveApplicationName(argsStr);
        ctx.Args = _commandLineTokenizer.ToTokens(argsStr);
    }

    private bool IsPosition(string flag)
    {
        if (flag == "--position")
            Log(Serverity.Info, "Found --position flag");
        return flag == "--position";
    }

    private bool IsSuggestion(string command)
    {
        if (command == "suggest")
            Log(Serverity.Info, "Found suggest command");
        return command == "suggest";
    }

    private IEnumerable<string> ParseSuggestions(string partString)
    {
        var result = new List<string>();
        result.AddRange(GetSuggestionsOnCommands(_commands, partString));
        result.AddRange(GetSuggestionsOnFlags(_globalFlags, partString));
        result.AddRange(GetSuggestionsOnCommandArgument(_commands, partString));
        result.AddRange(GetSuggestionsOnGlobalArguments(partString));

        if (result.Any())
        {
            var suggestions = result.Aggregate((c, n) => c + ", " + n);
            Log(Serverity.Info, $"Found suggestions for string '{partString}': {suggestions}");
        }
        return result;
    }

    private IEnumerable<string> GetSuggestionsOnGlobalArguments(string partString)
    {
        if (_commands.All(x => !x.IsSet))
        {
            return _globalArguments
                .SelectMany(x => x.Examples)
                .Where(x => string.IsNullOrWhiteSpace(partString) || x.ToLowerInvariant().Contains(partString));
        }
        return Enumerable.Empty<string>();
    }

    private string RemoveApplicationName(string partString)
    {
        var index = partString.IndexOf(_exeFileName);
        if (index == -1)
            return partString;
        index += _exeFileName.Length;
        var firstSpace = partString.Substring(index).IndexOf(" ");
        if (firstSpace == -1)
            return "";
        return partString.Substring(index + firstSpace + 1);
    }

    private IEnumerable<string> GetSuggestionsOnFlags(IEnumerable<IItem> flags, string partString)
    {
        var result = new List<string>();
        result.AddRange(flags
            .Where(x => (!x.IsSet || !string.IsNullOrEmpty(x.DefaultValue))
                && (string.IsNullOrWhiteSpace(partString) || ("--" + x.Name.ToLowerInvariant()).Contains(partString))
                && !x.Hidden)
            .Select(x => "--" + x.Name));
        result.AddRange(flags
            .Where(x => !x.IsSet
                && x.ShortName != '\0'
                && (string.IsNullOrWhiteSpace(partString) || ("-" + x.ShortName).Contains(partString))
                && !x.Hidden)
            .Select(x => "-" + x.ShortName));
        return result;
    }

    private IEnumerable<string> GetSuggestionsOnCommands(IEnumerable<CommandItem> commands, string partString)
    {
        var result = new List<string>();

        if (commands.All(x => !x.IsSet))
        {
            result.AddRange(commands
                .Where(x => (string.IsNullOrWhiteSpace(partString) || x.Name.ToLowerInvariant().Contains(partString))
                    && !x.Hidden)
                .Select(x => x.Name));
            return result;
        }

        foreach (var command in commands)
        {
            if (!command.IsSet)
                continue;

            if (command.Commands.All(x => !x.IsSet))
            {
                result.AddRange(command.Commands
                    .Where(x => (string.IsNullOrWhiteSpace(partString) || x.Name.ToLowerInvariant().Contains(partString))
                        && !x.Hidden)
                    .Select(x => x.Name));
                result.AddRange(GetSuggestionsOnFlags(command.Flags, partString));
                return result;
            }

            if (command.Commands.Any())
                result.AddRange(GetSuggestionsOnCommands(command.Commands, partString));
        }
        return result;
    }

    private IEnumerable<string> GetSuggestionsOnCommandArgument(IEnumerable<CommandItem> commands, string partString)
    {
        var result = new List<string>();

        if (commands.All(x => !x.IsSet))
            return result;

        foreach (var command in commands)
        {
            if (!command.IsSet)
                continue;

            if (!command.Commands.Any() || command.Commands.All(x => !x.IsSet))
            {
                result.AddRange(command.Arguments
                    .SelectMany(x => x.Suggestions)
                    .Where(x => string.IsNullOrWhiteSpace(partString) || x.ToLowerInvariant().Contains(partString)));
                return result;
            }

            result.AddRange(GetSuggestionsOnCommandArgument(command.Commands, partString));
        }
        return result;
    }

    private void SetDefaults(ParseContext ctx)
    {
        SetCommandsToDefault(_commands);
        SetToDefault(_globalFlags, ctx);
        SetToDefault(_globalArguments, ctx);
    }

    private static void SetCommandsToDefault(IEnumerable<CommandItem> commands)
    {
        foreach (var command in commands)
        {
            if (command.Commands.Any())
                SetCommandsToDefault(command.Commands);
            ApplyDefaults(command.Flags);
            ApplyDefaults(command.Arguments);
        }
    }

    private static void ApplyDefaults(IEnumerable<IItem> items)
    {
        foreach (var item in items.Where(x => !string.IsNullOrWhiteSpace(x.DefaultValue)))
        {
            item.IsSet = true;
            item.StringValue = item.DefaultValue;
        }
    }

    private void SetToDefault(IEnumerable<IItem> items, ParseContext ctx)
    {
        foreach (var item in items.Where(x => !string.IsNullOrWhiteSpace(x.DefaultValue)))
        {
            item.IsSet = true;
            item.StringValue = item.DefaultValue;
            Add(item, ctx);
        }
    }

    private void CheckAllRequiredItemsIsSet()
    {
        CheckCommands(_commands);
        CheckFlags(_globalFlags);
        CheckArguments(_globalArguments);
    }

    private void CheckCommands(IEnumerable<CommandItem> commands)
    {
        foreach (var command in commands)
        {
            if (command.Required && !command.IsSet)
                throw new ParseException($"Required command <{command.Name}> not set");

            if (!command.IsSet)
                continue;

            if (command.Commands.Any())
                CheckCommands(command.Commands);
            CheckFlags(command.Flags);
            CheckArguments(command.Arguments);
        }
    }

    private static void CheckArguments(IEnumerable<IItem> arguments)
    {
        foreach (var argument in arguments)
            if (argument.Required && !argument.IsSet)
                throw new ParseException($"Required argument <{argument.Name}> not set");
    }

    private static void CheckFlags(IEnumerable<IItem> flags)
    {
        foreach (var flag in flags)
            if (flag.Required && !flag.IsSet)
                throw new ParseException($"Required flag --{flag.Name} not set");
    }

    private void CommandFound(CommandItem command, ParseContext ctx)
    {
        while (ctx.CurrentItem < ctx.Args.Count)
        {
            if (IsCommand(ctx.Args[ctx.CurrentItem], command.Commands, out CommandItem commandFound))
            {
                MergeCommand("Command", commandFound, ctx);
                ctx.CurrentItem++;
                CommandFound(commandFound, ctx);
            }
            else if (IsFlag(ctx.Args[ctx.CurrentItem], command.Flags, _globalFlags, ctx, out IItem flagFound))
            {
                Merge("Command", flagFound, ctx);
                ctx.CurrentItem++;
            }
            else if (IsArgument(ctx.Args[ctx.CurrentItem], command.Arguments, out IItem argumentFound) && !ctx.Result.IsSuggestion)
            {
                if (argumentFound.ValueType == ValueType.ListOfString)
                {
                    argumentFound.StringValues = ctx.Args[ctx.CurrentItem].Split(',').Select(x => x.Trim()).ToList();
                    ctx.CurrentItem = ctx.Args.Count;
                }
                Merge("Command", argumentFound, ctx);
                ctx.CurrentItem++;
            }
            else
            {
                if (!ctx.Result.IsSuggestion)
                    throw new ParseException($"Didn't expect argument {ctx.Args[ctx.CurrentItem]}");
                ctx.SuggestionString = ctx.Args[ctx.CurrentItem];
                ctx.CurrentItem++;
            }
        }
    }

    private void MergeCommand(string name, CommandItem item, ParseContext ctx)
    {
        item.IsSet = true;
        ctx.Result.Result[name] = ctx.Result.Result[name] + ":" + item.Name;
        Log(Serverity.Info, $"Found command {ctx.Result.Result[name]}");

        CheckForDefaultValues(ctx.Result.Result[name], item.Flags, ctx);
        CheckForDefaultValues(ctx.Result.Result[name], item.Arguments, ctx);
    }

    private static void CheckForDefaultValues(string name, IEnumerable<IItem> items, ParseContext ctx)
    {
        foreach (var item in items.Where(x => x.IsSet))
            ctx.Result.Result[name + ":" + item.Name] = item.StringValue;
    }

    private void AddCommand(string name, IItem item, ParseContext ctx)
    {
        item.IsSet = true;
        ctx.Result.Result.Add(name, item.Name);
        Log(Serverity.Info, $"Found command {item.Name}");
    }

    private void Merge(string name, IItem item, ParseContext ctx)
    {
        item.IsSet = true;
        var value = string.IsNullOrWhiteSpace(item.StringValue) && !string.IsNullOrWhiteSpace(item.DefaultValue)
            ? item.DefaultValue
            : item.StringValue;
        AddOrUpdateResult(name, value, item, ctx);
    }

    private void AddOrUpdateResult(string name, string value, IItem item, ParseContext ctx)
    {
        if (_globalFlags.Contains(item) || _globalArguments.Contains(item))
        {
            ctx.Result.Result[item.Name] = value;
            Log(Serverity.Info, $"Found/updated global flag {item.Name} with value {value}");
        }
        else
        {
            var key = ctx.Result.Result[name] + ":" + item.Name;
            ctx.Result.Result[key] = value;
            Log(Serverity.Info, $"Found/updated {key} with value {value}");
        }
    }

    private void Add(IItem item, ParseContext ctx)
    {
        item.IsSet = true;
        var value = string.IsNullOrWhiteSpace(item.StringValue) && !string.IsNullOrWhiteSpace(item.DefaultValue)
            ? item.DefaultValue
            : item.StringValue;
        ctx.Result.Result[item.Name] = value;
        Log(Serverity.Info, $"Found/updated {item.Name} with value {value}");
    }

    private bool IsArgument(string arg, IEnumerable<IItem> arguments, out IItem item)
    {
        item = null;
        var errors = new List<string>();

        if (!arguments.Any())
            return false;

        var argumentsFound = arguments.Where(a => IsValidArgument(a, arg, errors)).ToList();
        if (argumentsFound.Count > 1)
            throw new ParseException("Found multiple arguments");
        if (argumentsFound.Count == 0)
            return false;

        item = argumentsFound[0];
        item.StringValue = arg;
        return true;
    }

    private static string GetValue(IItem item, string arg)
    {
        var parts = arg.SplitFirst('=');

        if (parts.Length == 1)
        {
            if (item.ValueType == ValueType.Bool)
                return "true";
            throw new ParseException("Not a boolean " + arg);
        }

        return parts[1];
    }

    private static List<string> GetValues(IItem item, string arg)
    {
        var parts = arg.SplitFirst('=');

        if (parts.Length == 1)
            throw new ParseException("Not a list value " + arg);

        var list = parts[1].Split(',').Select(x => x.Trim()).ToList();
        if (list.Count == 0)
            throw new ParseException("Couldn't parse " + parts[1] + " to list of strings");

        return list;
    }

    private static bool IsValidArgument(IItem argument, string arg, List<string> listOfErrors)
    {
        var result = IsValidItem(argument, arg);
        if (!result.success)
            listOfErrors.Add(result.errorMessage);
        return result.success;
    }

    private static bool IsValidFlag(IItem flag, string arg, List<string> listOfErrors)
    {
        var parts = arg.SplitFirst('=');

        if (parts.Length == 1)
        {
            if (flag.ValueType == ValueType.Bool)
                return true;
            listOfErrors.Add($"--{flag.Name} need a value");
            return false;
        }

        var result = IsValidItem(flag, parts[1]);
        if (!result.success)
            listOfErrors.Add(result.errorMessage);
        return result.success;
    }

    private static (bool success, string errorMessage) IsValidItem(IItem item, string argument)
    {
        if (item.ValueType == ValueType.Bool)
            return bool.TryParse(argument, out _)
                ? (true, "")
                : (false, $"'{argument}' is not a boolean (true/false)");

        if (item.DirectoryShouldExist)
            return Directory.Exists(argument)
                ? (true, "")
                : (false, $"directory '{argument}' does not exist");

        if (item.FileShouldExist)
            return File.Exists(argument)
                ? (true, "")
                : (false, $"file '{argument}' does not exist");

        if (item.ValueType == ValueType.Duration)
            return TimeSpan.TryParse(argument, CultureInfo.InvariantCulture, out _)
                ? (true, "")
                : (false, $"'{argument}' is not a duration (Days.Hours:Minutes:Seconds.Milli)");

        if (item.ValueType == ValueType.Enum)
        {
            try
            {
                Enum.Parse(item.TypeOfEnum, argument);
                return (true, "");
            }
            catch (ArgumentException)
            {
                var values = string.Join(",", Enum.GetNames(item.TypeOfEnum));
                return (false, $"'{argument}' is not any for the values {values}");
            }
        }

        if (item.ValueType == ValueType.Float)
            return float.TryParse(argument, NumberStyles.Float, CultureInfo.InvariantCulture, out _)
                ? (true, "")
                : (false, $"'{argument}' is not a float");

        if (item.ValueType == ValueType.Int)
            return int.TryParse(argument, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)
                ? (true, "")
                : (false, $"'{argument}' is not an integer");

        if (item.ValueType == ValueType.Long)
            return long.TryParse(argument, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)
                ? (true, "")
                : (false, $"'{argument}' is not a long");

        if (item.ValueType == ValueType.Ip)
            return MatchesRegex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b", argument)
                ? (true, "")
                : (false, $"'{argument}' is not an IP address");

        if (item.ValueType == ValueType.Tcp)
            return MatchesRegex(@"(([0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3})|(\w*.\w*.\w*)):[0-9]{1,5}", argument)
                ? (true, "")
                : (false, $"'{argument}' is not a hostname");

        if (item.ValueType == ValueType.Url)
            return Uri.IsWellFormedUriString(argument, UriKind.RelativeOrAbsolute)
                ? (true, "")
                : (false, $"'{argument}' is not a well formed URL");

        if (item.ValueType == ValueType.Date)
            return DateTime.TryParseExact(argument, new[] { "yyyy.MM.dd", "yyyy-MM-dd", "yyyy/MM/dd" },
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                ? (true, "")
                : (false, $"'{argument}' is not a date");

        if (item.ValueType == ValueType.String || item.ValueType == ValueType.ListOfString)
            return (true, "");

        return (false, "Unknown error");
    }

    private static bool MatchesRegex(string pattern, string input)
    {
        return new Regex(pattern).IsMatch(input);
    }

    private bool IsFlag(string arg, IEnumerable<IItem> flags, IEnumerable<IItem> globalFlags, ParseContext ctx, out IItem item)
    {
        item = null;

        var localFlags = flags?.ToList() ?? new List<IItem>();
        var globalFlagsList = globalFlags?.ToList() ?? new List<IItem>();

        if (!localFlags.Any() && !globalFlagsList.Any())
            return false;

        var errors = new List<string>();

        if (arg.StartsWith("--"))
        {
            var flagName = GetFlagName(arg);
            var foundLocal = localFlags.Where(f => flagName == f.Name.ToLower() && IsValidFlag(f, arg, errors)).ToList();
            var foundGlobal = globalFlagsList.Where(f => flagName == f.Name.ToLower() && IsValidFlag(f, arg, errors)).ToList();
            return EvaluateItem(foundLocal, foundGlobal, arg, errors, ctx, out item);
        }

        if (arg.StartsWith("-"))
        {
            var flagName = GetFlagName(arg);
            if (flagName.Length == 0 || flagName.Length > 2)
            {
                if (ctx.Result.IsSuggestion) return false;
                throw new ParseException("Short name arguments are only one character " + flagName);
            }
            var foundLocal = localFlags.Where(f => f.ShortName == flagName[0] && IsValidFlag(f, arg, errors)).ToList();
            var foundGlobal = globalFlagsList.Where(f => flagName[0] == f.ShortName && IsValidFlag(f, arg, errors)).ToList();
            return EvaluateItem(foundLocal, foundGlobal, arg, errors, ctx, out item);
        }

        return false;
    }

    private bool EvaluateItem(List<IItem> localItems, List<IItem> globalItems, string arg, List<string> errors, ParseContext ctx, out IItem item)
    {
        item = null;

        if (!localItems.Any() && !globalItems.Any())
        {
            if (ctx.Result.IsSuggestion) return false;
            throw new ParseException("Illegal flag " + arg, errors);
        }
        if (localItems.Count > 1 || globalItems.Count > 1)
        {
            if (ctx.Result.IsSuggestion) return false;
            throw new ParseException("Found multiple flags with same name " + arg);
        }

        item = localItems.FirstOrDefault() ?? globalItems[0];

        if (item.ValueType == ValueType.ListOfString)
            item.StringValues = GetValues(item, arg);
        else
            item.StringValue = GetValue(item, arg);
        return true;
    }

    private static string GetFlagName(string arg)
    {
        return arg.TrimStart('-').Split('=')[0].ToLower();
    }

    private static bool IsCommand(string arg, IEnumerable<CommandItem> commands, out CommandItem commandFound)
    {
        commandFound = null;
        foreach (var command in commands)
        {
            if (arg.Equals(command.Name, StringComparison.OrdinalIgnoreCase))
            {
                commandFound = command;
                return true;
            }
        }
        return false;
    }
}

[Serializable]
public class ParseException : Exception
{
    private readonly List<string> _errors = new List<string>();
    public List<string> Errors => _errors;

    public ParseException() { }

    public ParseException(object p) : base(p?.ToString()) { }

    public ParseException(string message) : base(message) { }

    public ParseException(string message, List<string> errors) : base(message)
    {
        _errors.AddRange(errors);
    }

    public ParseException(string message, Exception innerException) : base(message, innerException) { }

#pragma warning disable SYSLIB0051
    protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051
}
