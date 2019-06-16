using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace KingpinNet
{
    public class Parser
    {
        private readonly List<CommandBuilder> _commands;
        private readonly List<CommandLineItemBuilder<string>> _globalFlags;
        private readonly List<CommandLineItemBuilder<string>> _globalArguments;
        private Dictionary<string, string> _result;
        private List<string> _args;
        private int _currentItem;

        public Parser(KingpinApplication application)
        {
            _commands = application.Commands.ToList();
            _globalFlags = application.Flags.ToList();
            _globalArguments = application.Arguments.ToList();
        }

        public IDictionary<string, string> Parse(IEnumerable<string> args)
        {
            _result = new Dictionary<string, string>();
            _args = args.ToList();
            _currentItem = 0;
            MainParse();
            return _result;
        }

        private void MainParse()
        {
            SetDefaults();

            if (_args.Count > 0)
                while (_currentItem<_args.Count)
                {
                    if (IsCommand(_args[_currentItem], _commands, out CommandLineItem<string> commandFound))
                    {
                        AddCommand("command", commandFound);
                        _currentItem++;
                        CommandFound(commandFound);
                    }
                    else if (IsFlag(_args[_currentItem], _globalFlags, out CommandLineItem<string> flagFound))
                    {
                        Add(flagFound);
                        _currentItem++;
                    }
                    else if (IsArgument(_args[_currentItem], _globalArguments, out CommandLineItem<string> argumentFound))
                    {
                        Add(argumentFound);
                        _currentItem++;
                    }
                    else
                        throw new ParseException($"Didn't expect argument {_args[_currentItem]}");
                }
            CheckAllRequiredItemsIsSet();
        }

        private void SetDefaults()
        {
            SetCommandsToDefault(_commands);
            SetFlagsToDefault(_globalFlags, true);
            SetArgumentsToDefault(_globalArguments, true);
        }

        private void SetCommandsToDefault(IEnumerable<CommandBuilder> commands)
        {
            foreach (var command in commands)
            {
                if (command.Item?.Commands != null && command.Item.Commands.Count() > 0)
                    SetCommandsToDefault(command.Item.Commands);
                SetFlagsToDefault(command.Item?.Flags, false);
                SetArgumentsToDefault(command.Item?.Arguments, false);
            }
        }

        private void SetArgumentsToDefault(IEnumerable<CommandLineItemBuilder<string>> arguments, bool addToResult)
        {
            foreach (var argument in arguments)
                if (!string.IsNullOrWhiteSpace(argument.Item.DefaultValue))
                {
                    argument.Item.IsSet = true;
                    argument.Item.Value = argument.Item.DefaultValue;
                    if (addToResult) Add(argument.Item);
                }
        }

        private void SetFlagsToDefault(IEnumerable<CommandLineItemBuilder<string>> flags, bool addToResult)
        {
            foreach (var flag in flags)
                if (!string.IsNullOrWhiteSpace(flag.Item.DefaultValue))
                {
                    flag.Item.IsSet = true;
                    flag.Item.Value = flag.Item.DefaultValue;
                    if (addToResult) Add(flag.Item);
                }
        }

        private void CheckAllRequiredItemsIsSet()
        {
            CheckCommands(_commands);
            CheckFlags(_globalFlags);
            CheckArguments(_globalArguments);
        }

        private void CheckCommands(IEnumerable<CommandBuilder> commands)
        {
            foreach (var command in commands)
            {
                if (command.Item.IsRequired && !command.Item.IsSet)
                    throw new ParseException($"Required command <{command.Item.Name}> not set");

                if (!command.Item.IsSet)
                    continue;

                if (command.Item?.Commands != null && command.Item.Commands.Count() > 0)
                    CheckCommands(command.Item.Commands);
                CheckFlags(command.Item.Flags);
                CheckArguments(command.Item.Arguments);
            }
        }

        private void CheckArguments(IEnumerable<CommandLineItemBuilder<string>> arguments)
        {
            foreach (var argument in arguments)
                if (argument.Item.IsRequired && !argument.Item.IsSet)
                    throw new ParseException($"Required argument <{argument.Item.Name}> not set");
        }

        private void CheckFlags(IEnumerable<CommandLineItemBuilder<string>> flags)
        {
            foreach (var flag in flags)
                if (flag.Item.IsRequired && !flag.Item.IsSet)
                    throw new ParseException($"Required flag --{flag.Item.Name} not set");
        }

        private void CommandFound(CommandLineItem<string> command)
        {
            SetDefaults();
            while (_currentItem < _args.Count)
            {
                if (IsCommand(_args[_currentItem], command.Commands, out CommandLineItem<string> commandFound))
                {
                    MergeCommand("command", commandFound);
                    _currentItem++;
                    CommandFound(commandFound);
                } else if (IsFlag(_args[_currentItem], command.Flags, out CommandLineItem<string> flagFound))
                {
                    Merge("command", flagFound);
                    _currentItem++;
                }
                else if (IsArgument(_args[_currentItem], command.Arguments, out CommandLineItem<string> argumentFound))
                {
                    Merge("command", argumentFound);
                    _currentItem++;
                }
                else
                    throw new ParseException($"Didn't expect argument {_args[_currentItem]}");
            }
        }

        private void MergeCommand(string name, CommandLineItem<string> item)
        {
            item.IsSet = true;
            _result[name] = _result[name] + ":" + item.Name;
            CheckFlagsForDefaultValues(_result[name], item.Flags);
            CheckArgumentsForDefaultValues(_result[name], item.Arguments);
        }

        private void CheckFlagsForDefaultValues(string name, IEnumerable<CommandLineItemBuilder<string>> items)
        {
            foreach (var item in items.Where(x => x.Item.IsSet))
                _result[name + ":" + item.Item.Name] = item.Item.Value;
        }

        private void CheckArgumentsForDefaultValues(string name, IEnumerable<CommandLineItemBuilder<string>> items)
        {
            foreach (var item in items.Where(x => x.Item.IsSet))
                _result[name + ":" + item.Item.Name] = item.Item.Value;
        }

        private void AddCommand(string name, CommandLineItem<string> item)
        {
            item.IsSet = true;
            _result.Add(name, item.Name);
        }

        private void Merge(string name, CommandLineItem<string> item)
        {
            item.IsSet = true;
            if (string.IsNullOrWhiteSpace(item.Value) && !string.IsNullOrWhiteSpace(item.DefaultValue))
                _result.Add(_result[name] + ":" + item.Name, item.DefaultValue);
            else
                _result.Add(_result[name] + ":" + item.Name, item.Value);
        }
        private void Add(CommandLineItem<string> item)
        {
            item.IsSet = true;
            if (string.IsNullOrWhiteSpace(item.Value) && !string.IsNullOrWhiteSpace(item.DefaultValue))
                _result.Add(item.Name, item.DefaultValue);
            else
                _result.Add(item.Name, item.Value);
        }

        private bool IsArgument(string arg, IEnumerable<CommandLineItemBuilder<string>> arguments,
            out CommandLineItem<string> item)
        {
            item = null;
            var errors = new List<string>();

            if (arguments.Any()) {
                var argumentsFound = arguments.Where(a => IsValidArgument(a, arg, errors)).ToList();
                if (argumentsFound.Count() > 1)
                    throw new ParseException("Found multiple arguments");
                if (argumentsFound.Count() == 0)
                    return false;
                item = argumentsFound.First().Item;
                item.Value = arg;
                return true;
            }
            return false;
        }

        private string GetValue(CommandLineItem<string> item, string arg)
        {
            var parts = arg.Split('=');

            if (parts.Length == 1)
                if (item.ValueType == ValueType.Bool)
                {
                    item.Action?.Invoke(arg);
                    return "true";
                }
                else
                    throw new ParseException("Not a boolean " + arg);

            if (parts.Length == 2)
            {
                item.Action?.Invoke(arg);
                return parts[1];
            }

            throw new ParseException("Found too many = signs" + arg);
        }

        private bool IsValidArgument(CommandLineItemBuilder<string> argument, string arg, List<string> listOfErrors)
        {
            var result = IsValidItem(argument.Item, arg);
            if (!result.success)
                listOfErrors.Add(result.errorMessage);
            return result.success;
        }

        private bool IsValidFlag(CommandLineItemBuilder<string> flag, string arg, List<string> listOfErrors)
        {
            var parts = arg.Split('=');

            if (parts.Length == 1)
                if (flag.Item.ValueType == ValueType.Bool)
                {
                    return true;
                }
                else
                {
                    listOfErrors.Add($"--{flag.Item.Name} need a value");
                    return false;
                }

            if (parts.Length == 2)
            {
                var result = IsValidItem(flag.Item, parts[1]);
                if (!result.success)
                    listOfErrors.Add(result.errorMessage);
                return result.success;
            }

            return false;
        }


        private (bool success, string errorMessage) IsValidItem(CommandLineItem<string> item, string argument)
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
                if (TimeSpan.TryParse(argument, out _))
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
                if (float.TryParse(argument, out _))
                    return (true, "");
                else
                    return (false, $"'{argument}' is not a float");
            }
            else if (item.ValueType == ValueType.Int)
            {
                if (Int32.TryParse(argument, out _))
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

        private bool IsFlag(string arg, IEnumerable<CommandLineItemBuilder<string>> flags, out CommandLineItem<string> item)
        {
            item = null;

            if (flags == null || flags.Count() == 0)
                return false;

            var errors = new List<string>();

            if (arg.StartsWith("--"))
            {
                var foundFlags = flags.Where(f => arg.Replace("--", "").ToLower().StartsWith(f.Item.Name.ToLower()) &&
                    IsValidFlag(f, arg, errors)).ToArray();
                if (!foundFlags.Any())
                    throw new ParseException("Illegal flag " + arg, errors);
                if (foundFlags.Count() > 1)
                    throw new ParseException("Found multiple flags with same name " + arg);
                item = foundFlags.First().Item;
                item.Value = GetValue(foundFlags.First().Item, arg);
                return true;
            }
            if (arg.StartsWith("-"))
            {
                var parts = arg.Split('=');
                if (parts[0].Length > 2)
                    throw new ParseException("Short name arguments are only one character " + parts[0]);
                var foundFlags = flags.Where(f => f.Item.ShortName == parts[0][1] && IsValidFlag(f, arg, errors)).ToArray();
                if (!foundFlags.Any())
                    throw new ParseException("Illegal flag " + arg);
                if (foundFlags.Count() > 1)
                    throw new ParseException("Found multiple flags with same name" + arg);
                item = foundFlags.First().Item;
                item.Value = GetValue(foundFlags.First().Item, arg);
                return true;
            }
            return false;
        }


        private bool IsCommand(string arg, IEnumerable<CommandBuilder> commands, out CommandLineItem<string> commandFound)
        {
            commandFound = null;
            foreach (var command in commands)
                if (arg.ToLower() == command.Item.Name.ToLower())
                {
                    commandFound = command.Item;
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
