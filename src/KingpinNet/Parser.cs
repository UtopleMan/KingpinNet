using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace KingpinNet
{
    public class ParseResult
    {
        public ParseResult()
        {
            Result = new Dictionary<string, string>();
            Suggestions = new List<string>();
            IsSuggestion = false;
        }
        public Dictionary<string, string> Result { get; }
        public List<string> Suggestions { get; }
        public bool IsSuggestion { get; set; }
    }
    public class Parser
    {
        private readonly List<CommandItem> _commands;
        private readonly List<IItem> _globalFlags;
        private readonly List<IItem> _globalArguments;
        private ParseResult _result;
        private List<string> _args;
        private int _currentItem;
        private string _suggestionString;

        public Parser(KingpinApplication application)
        {
            _commands = application.Commands.ToList();
            _globalFlags = application.Flags.ToList();
            _globalArguments = application.Arguments.ToList();
        }

        public ParseResult Parse(IEnumerable<string> args)
        {
            _result = new ParseResult();
            _args = args.ToList();
            _currentItem = 0;
            _suggestionString = "";
            MainParse();
            return _result;
        }

        private void MainParse()
        {
            SetDefaults();

            if (_args.Count > 0)
            {
                InvestigateSuggestions();
                while (_currentItem < _args.Count)
                {
                    if (IsCommand(_args[_currentItem], _commands, out CommandItem commandFound))
                    {
                        AddCommand("Command", commandFound);

                        _currentItem++;
                        CommandFound(commandFound);
                    }
                    else if (IsFlag(_args[_currentItem], new List<IItem>(), _globalFlags, out IItem flagFound))
                    {
                        Add(flagFound);
                        _currentItem++;
                    }
                    else if (IsArgument(_args[_currentItem], _globalArguments, out IItem argumentFound))
                    {
                        Add(argumentFound);
                        _currentItem++;
                    }
                    else
                    {
                        if (!_result.IsSuggestion)
                            throw new ParseException($"Didn't expect argument {_args[_currentItem]}");
                        _suggestionString = _args[_currentItem];
                        _currentItem++;
                    }
                }
            }
            if (_result.IsSuggestion)
            {
                _result.Suggestions.AddRange(ParseSuggestions(_suggestionString));
            }
            else
                CheckAllRequiredItemsIsSet();
        }

        private void InvestigateSuggestions()
        {
            if (!IsSuggestion(_args[0]))
                return;
            if (_args.Count > 1 && IsPosition(_args[1]))
            {
                _args = _args.Skip(2).ToList();
            }
            else
            {
                _args = _args.Skip(1).ToList();
            }
            _result.IsSuggestion = true;
        }

        private bool IsPosition(string flag)
        {
            return flag == "--position";
        }

        private bool IsSuggestion(string command)
        {
            return command == "suggest";
        }

        private IEnumerable<string> ParseSuggestions(string partString)
        {
            var result = new List<string>();
            result.AddRange(GetSuggestionsOnCommands(_commands, partString));
            result.AddRange(GetSuggestionsOnFlags(_globalFlags, partString));
            return result;
        }

        private IEnumerable<string> GetSuggestionsOnFlags(IEnumerable<IItem> flags, string partString)
        {
            var result = new List<string>();
            result.AddRange(flags.Where(x => !x.IsSet && ("--" + x.Name.ToLowerInvariant()).Contains(partString) && !x.Hidden)
                .Select(x => "--" + x.Name));
            result.AddRange(flags.Where(x => !x.IsSet && x.ShortName != 0 && ("-" + x.ShortName).Contains(partString) && !x.Hidden)
                .Select(x => "-" + x.ShortName));
            return result;
        }

        private IEnumerable<string> GetSuggestionsOnCommands(IEnumerable<CommandItem> commands, string partString)
        {
            var result = new List<string>();

            if (commands.All(x => !x.IsSet))
            {
                result.AddRange(
                    commands
                    .Where(x => x.Name.ToLowerInvariant().Contains(partString) && !x.Hidden)
                    .Select(x => x.Name));
                return result;
            }


            foreach (var command in commands)
            {
                if (!command.IsSet)
                    continue;

                if (command.Commands.All(x => !x.IsSet))
                {
                    result.AddRange(
                        command.Commands
                        .Where(x => x.Name.ToLowerInvariant().Contains(partString) && !x.Hidden)
                        .Select(x => x.Name));
                    result.AddRange(GetSuggestionsOnFlags(command.Flags, partString));
                    return result;
                }

                if (command.Commands != null && command.Commands.Count() > 0)
                    result.AddRange(GetSuggestionsOnCommands(command.Commands, partString));
            }
            return result;

        }

        private void SetDefaults()
        {
            SetCommandsToDefault(_commands);
            SetToDefault(_globalFlags, true);
            SetToDefault(_globalArguments, true);
        }

        private void SetCommandsToDefault(IEnumerable<CommandItem> commands)
        {
            foreach (var command in commands)
            {
                if (command.Commands != null && command.Commands.Count() > 0)
                    SetCommandsToDefault(command.Commands);
                SetToDefault(command.Flags, false);
                SetToDefault(command.Arguments, false);
            }
        }

        private void SetToDefault(IEnumerable<IItem> items, bool addToResult)
        {
            foreach (var item in items)
                if (!string.IsNullOrWhiteSpace(item.DefaultValue))
                {
                    item.IsSet = true;
                    item.StringValue = item.DefaultValue;
                    if (addToResult) Add(item);
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

                if (command.Commands != null && command.Commands.Count() > 0)
                    CheckCommands(command.Commands);
                CheckFlags(command.Flags);
                CheckArguments(command.Arguments);
            }
        }

        private void CheckArguments(IEnumerable<IItem> arguments)
        {
            foreach (var argument in arguments)
                if (argument.Required && !argument.IsSet)
                    throw new ParseException($"Required argument <{argument.Name}> not set");
        }

        private void CheckFlags(IEnumerable<IItem> flags)
        {
            foreach (var flag in flags)
                if (flag.Required && !flag.IsSet)
                    throw new ParseException($"Required flag --{flag.Name} not set");
        }

        private void CommandFound(CommandItem command)
        {
            //SetDefaults();
            while (_currentItem < _args.Count)
            {
                if (IsCommand(_args[_currentItem], command.Commands, out CommandItem commandFound))
                {
                    MergeCommand("Command", commandFound);
                    _currentItem++;
                    CommandFound(commandFound);
                }
                else if (IsFlag(_args[_currentItem], command.Flags, _globalFlags, out IItem flagFound))
                {
                    Merge("Command", flagFound);
                    _currentItem++;
                }
                else if (IsArgument(_args[_currentItem], command.Arguments, out IItem argumentFound))
                {
                    Merge("Command", argumentFound);
                    _currentItem++;
                }
                else
                {
                    if (!_result.IsSuggestion)
                        throw new ParseException($"Didn't expect argument {_args[_currentItem]}");
                    _suggestionString = _args[_currentItem];
                    _currentItem++;
                }
            }
        }

        private void MergeCommand(string name, CommandItem item)
        {

            item.IsSet = true;
            _result.Result[name] = _result.Result[name] + ":" + item.Name;
            CheckForDefaultValues(_result.Result[name], item.Flags);
            CheckForDefaultValues(_result.Result[name], item.Arguments);
        }

        private void CheckForDefaultValues(string name, IEnumerable<IItem> items)
        {
            foreach (var item in items.Where(x => x.IsSet))
                _result.Result[name + ":" + item.Name] = item.StringValue;
        }

        private void AddCommand(string name, IItem item)
        {
            item.IsSet = true;
            _result.Result.Add(name, item.Name);
        }

        private void Merge(string name, IItem item)
        {
            item.IsSet = true;
            var value = item.StringValue;
            if (string.IsNullOrWhiteSpace(item.StringValue) && !string.IsNullOrWhiteSpace(item.DefaultValue))
                value = item.DefaultValue;

            AddOrUpdateResult(name, value, item);
        }

        private void AddOrUpdateResult(string name, string value, IItem item)
        {
            if (_globalFlags.Contains(item) || _globalArguments.Contains(item))
            {
                if (_result.Result.ContainsKey(item.Name))
                    _result.Result[item.Name] = value;
                else
                    _result.Result.Add(item.Name, value);
            }
            else
            {
                if (_result.Result.ContainsKey(_result.Result[name] + ":" + item.Name))
                    _result.Result[_result.Result[name] + ":" + item.Name] = value;
                else
                    _result.Result.Add(_result.Result[name] + ":" + item.Name, value);
            }
        }

        private void Add(IItem item)
        {
            item.IsSet = true;
            var value = item.StringValue;
            if (string.IsNullOrWhiteSpace(item.StringValue) && !string.IsNullOrWhiteSpace(item.DefaultValue))
                value = item.DefaultValue;
            if (_result.Result.ContainsKey(item.Name))
                _result.Result[item.Name] = value;
            else
                _result.Result.Add(item.Name, value);
        }

        private bool IsArgument(string arg, IEnumerable<IItem> arguments,
            out IItem item)
        {
            item = null;
            var errors = new List<string>();

            if (arguments.Any())
            {
                var argumentsFound = arguments.Where(a => IsValidArgument(a, arg, errors)).ToList();
                if (argumentsFound.Count() > 1)
                    throw new ParseException("Found multiple arguments");
                if (argumentsFound.Count() == 0)
                    return false;
                item = argumentsFound.First();
                item.StringValue = arg;
                return true;
            }
            return false;
        }



        private string GetValue(IItem item, string arg)
        {
            var parts = arg.SplitFirst('=');

            if (parts.Length == 1)
                if (item.ValueType == ValueType.Bool)
                {
                    item.Action?.Invoke(arg);
                    return "true";
                }
                else
                    throw new ParseException("Not a boolean " + arg);

            item.Action?.Invoke(arg);
            return parts[1];
        }


        private bool IsValidArgument(IItem argument, string arg, List<string> listOfErrors)
        {
            var result = IsValidItem(argument, arg);
            if (!result.success)
                listOfErrors.Add(result.errorMessage);
            return result.success;
        }

        private bool IsValidFlag(IItem flag, string arg, List<string> listOfErrors)
        {
            var parts = arg.Split('=');

            if (parts.Length == 1)
                if (flag.ValueType == ValueType.Bool)
                {
                    return true;
                }
                else
                {
                    listOfErrors.Add($"--{flag.Name} need a value");
                    return false;
                }

            if (parts.Length >= 2)
            {
                var result = IsValidItem(flag, parts[1]);
                if (!result.success)
                    listOfErrors.Add(result.errorMessage);
                return result.success;
            }

            return false;
        }


        private (bool success, string errorMessage) IsValidItem(IItem item, string argument)
        {
            if (item.ValueType == ValueType.Bool)
            {
                if (bool.TryParse(argument, out _))

                    return (true, "");
                else
                    return (false, $"'{argument}' is not a boolean (true/false)");
            }
            else if (item.DirectoryShouldExist)
            {
                if (Directory.Exists(argument))
                    return (true, "");
                else
                    return (false, $"directory '{argument}' does not exist");
            }
            else if (item.FileShouldExist)
            {
                if (File.Exists(argument))
                    return (true, "");
                else
                    return (false, $"file '{argument}' does not exist");
            }
            else if (item.ValueType == ValueType.Duration)
            {
                if (TimeSpan.TryParse(argument, CultureInfo.InvariantCulture, out _))
                    return (true, "");
                else
                    return (false, $"'{argument}' is not a duration (Days.Hours:Minutes:Seconds.Milli)");
            }
            else if (item.ValueType == ValueType.Enum)
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
            else if (item.ValueType == ValueType.Float)
            {
                if (float.TryParse(argument, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                    return (true, "");
                else
                    return (false, $"'{argument}' is not a float");
            }
            else if (item.ValueType == ValueType.Int)
            {
                if (Int32.TryParse(argument, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                    return (true, "");
                else
                    return (false, $"'{argument}' is not an integer");
            }
            else if (item.ValueType == ValueType.Ip)
            {
                var result = Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b", argument);
                if (result)
                    return (true, "");
                else
                    return (false, $"'{argument}' is not an IP address");
            }
            else if (item.ValueType == ValueType.Tcp)
            {
                var result = Regex(@"(([0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3})|(\w*.\w*.\w*)):[0-9]{1,5}", argument);
                if (result)
                    return (true, "");
                else
                    return (false, $"'{argument}' is not a hostname");
            }
            else if (item.ValueType == ValueType.Url)
            {
                var result = Uri.IsWellFormedUriString(argument, UriKind.RelativeOrAbsolute);
                if (result)
                    return (true, "");
                else
                    return (false, $"'{argument}' is not a well formed URL");
            }
            else if (item.ValueType == ValueType.Date)
            {
                if (DateTime.TryParseExact(argument, new[] { "yyyy.MM.dd", "yyyy-MM-dd", "yyyy/MM/dd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    return (true, "");
                else
                    return (false, $"'{argument}' is not a date");
            }
            else if (item.ValueType == ValueType.String)
            {
                return (true, "");
            }
            return (false, "Unknown error");
        }

        private bool Regex(string regex, string input)
        {
            var regexEngine = new Regex(regex);
            var match = regexEngine.Match(input);
            return match.Success;
        }

        private bool IsFlag(string arg, IEnumerable<IItem> flags, IEnumerable<IItem> globalFlags, out IItem item)
        {
            item = null;

            if ((flags == null || flags.Count() == 0) && (globalFlags == null || globalFlags.Count() == 0))
                return false;

            var errors = new List<string>();

            if (arg.StartsWith("--"))
            {
                var foundFlags = flags.Where(f => GetFlagName(arg) == f.Name.ToLower() &&
                    IsValidFlag(f, arg, errors)).ToList();
                foundFlags.AddRange(globalFlags.Where(f => GetFlagName(arg) == f.Name.ToLower() &&
                    IsValidFlag(f, arg, errors)));
                if (!foundFlags.Any())
                {
                    if (_result.IsSuggestion) return false;
                    throw new ParseException("Illegal flag " + arg, errors);
                }
                if (foundFlags.Count() > 1)
                {
                    if (_result.IsSuggestion) return false;
                    throw new ParseException("Found multiple flags with same name " + arg);
                }
                item = foundFlags.First();
                item.StringValue = GetValue(foundFlags.First(), arg);
                return true;
            }
            if (arg.StartsWith("-"))
            {
                var flagName = GetFlagName(arg);
                if (flagName.Length == 0 || flagName.Length > 2)
                {
                    if (_result.IsSuggestion) return false;
                    throw new ParseException("Short name arguments are only one character " + flagName);
                }
                var foundFlags = flags.Where(f => f.ShortName == flagName[0] && IsValidFlag(f, arg, errors)).ToList();
                foundFlags.AddRange(globalFlags.Where(f => GetFlagName(arg)[0] == f.ShortName &&
                    IsValidFlag(f, arg, errors)));
                if (!foundFlags.Any())
                    throw new ParseException("Illegal flag " + arg, errors);
                if (foundFlags.Count() > 1)
                    throw new ParseException("Found multiple flags with same name" + arg);
                item = foundFlags.First();
                item.StringValue = GetValue(foundFlags.First(), arg);
                return true;
            }
            return false;
        }

        private string GetFlagName(string arg)
        {
            var flagName = arg.TrimStart('-').Split('=');
            return flagName[0].ToLower();
        }

        private bool IsCommand(string arg, IEnumerable<CommandItem> commands, out CommandItem commandFound)
        {
            commandFound = null;
            foreach (var command in commands)
                if (arg.ToLower() == command.Name.ToLower())
                {
                    commandFound = command;
                    return true;
                }
            return false;
        }
    }

    [Serializable]
    public class ParseException : Exception
    {
        private List<string> _errors = new List<string>();
        public List<string> Errors => _errors;
        public ParseException()
        {
        }

        public ParseException(object p)
        {
        }

        public ParseException(string message) : base(message)
        {
        }

        public ParseException(string message, List<string> errors) : base(message)
        {
            _errors.AddRange(errors);
        }

        public ParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
