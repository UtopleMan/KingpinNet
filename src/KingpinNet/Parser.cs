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
        private readonly List<CommandItem> _commands;
        private readonly List<FlagItem> _globalFlags;
        private readonly List<ArgumentItem> _globalArguments;
        private Dictionary<string, string> _result;
        private List<string> _args;
        private int _currentItem;

        public Parser(KingpinApplication application)
        {
            _commands = application.Commands;
            _globalFlags = application.Flags;
            _globalArguments = application.Arguments;
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
            while (_currentItem<_args.Count)
            {
                if (IsCommand(_args[_currentItem], _commands, out CommandLineItem commandFound))
                {
                    AddCommand("command", commandFound);
                    CommandFound(commandFound);
                }
                else if (IsFlag(_args[_currentItem], _globalFlags, out CommandLineItem flagFound))
                    Add(flagFound);
                else if (IsArgument(_args[_currentItem], _globalArguments, out CommandLineItem argumentFound))
                    Add(argumentFound);
                else
                    throw new ParseException($"Didn't expect argument {_args[_currentItem]}");
            }
            CheckAllRequiredItemsIsSet();
        }

        private void CheckAllRequiredItemsIsSet()
        {
            CheckCommands(_commands);
            CheckFlags(_globalFlags);
            CheckArguments(_globalArguments);
        }

        private void CheckCommands(List<CommandItem> commands)
        {
            foreach (var command in commands)
            {
                if (command.Item.IsRequired && !command.Item.IsSet)
                    throw new ParseException($"Required command <{command.Item.Name}> not set");

                if (command?.Item?.Commands != null && command.Item.Commands.Count > 0)
                    CheckCommands(command.Item.Commands);
                CheckFlags(command.Item.Flags);
                CheckArguments(command.Item.Arguments);
            }
        }

        private void CheckArguments(List<ArgumentItem> arguments)
        {
            foreach (var argument in arguments)
                if (argument.Item.IsRequired && !argument.Item.IsSet)
                    throw new ParseException($"Required argument <{argument.Item.Name}> not set");
        }

        private void CheckFlags(List<FlagItem> flags)
        {
            foreach (var flag in flags)
                if (flag.Item.IsRequired && !flag.Item.IsSet)
                    throw new ParseException($"Required flag --{flag.Item.Name} not set");
        }

        private void CommandFound(CommandLineItem command)
        {
            while (_currentItem < _args.Count)
            {
                if (IsCommand(_args[_currentItem], command.Commands, out CommandLineItem commandFound))
                {
                    MergeCommand("command", commandFound);
                    CommandFound(commandFound);
                } else if (IsFlag(_args[_currentItem], command.Flags, out CommandLineItem flagFound))
                    Add(flagFound);
                else if (IsArgument(_args[_currentItem], command.Arguments, out CommandLineItem argumentFound))
                    Add(argumentFound);
                else
                    throw new ParseException($"Didn't expect argument {_args[_currentItem]}");
            }
        }

        private void MergeCommand(string name, CommandLineItem item)
        {
            item.IsSet = true;
            _result[name] = _result[name] + "-" + item.Name;
            _currentItem++;
        }
        private void AddCommand(string name, CommandLineItem item)
        {
            item.IsSet = true;
            _result.Add(name, item.Name);
            _currentItem++;
        }
        private void Add(CommandLineItem item)
        {
            item.IsSet = true;
            _result.Add(item.Name, item.Value);
            _currentItem++;
        }

        private bool IsArgument(string arg, List<ArgumentItem> arguments, out CommandLineItem item)
        {
            item = null;

            if (arguments.Any()) {
                var argumentsFound = arguments.Where(a => IsValidArgument(a, arg)).ToList();
                if (argumentsFound.Count > 1)
                    throw new ParseException("Found multiple arguments");
                if (argumentsFound.Count == 0)
                    return false;
                item = argumentsFound.First().Item;
                item.Value = arg;
                return true;
            }
            return false;
        }

        private string GetValue(CommandLineItem item, string arg)
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

        private bool IsValidArgument(ArgumentItem argument, string arg)
        {
            return IsValidItem(argument.Item, arg);
        }

        private bool IsValidFlag(FlagItem flag, string arg)
        {
            var parts = arg.Split('=');

            if (parts.Length == 1)
                if (flag.Item.ValueType == ValueType.Bool)
                {
                    return true;
                }
                else
                    return false;

            if (parts.Length == 2)
                return IsValidItem(flag.Item, parts[1]);

            return false;
        }


        private bool IsValidItem(CommandLineItem item, string argument)
        {

            if (item.ValueType == ValueType.Bool)
            {
                if (bool.TryParse(argument, out bool result))
                    return true;
            }
            else if (item.DirectoryShouldExist)
            {
                if (Directory.Exists(argument))
                    return true;
            }
            else if (item.FileShouldExist)
            {
                if (File.Exists(argument))
                    return true;
            }
            else if (item.ValueType == ValueType.Duration)
            {
                if (TimeSpan.TryParse(argument, out TimeSpan result))
                    return true;
            }
            else if (item.ValueType == ValueType.Enum)
            {
                try
                {
                    var parse = Enum.Parse(item.TypeOfEnum, argument);
                    return true;
                }
                catch (ArgumentException)
                {
                }
            }
            else if (item.ValueType == ValueType.Float)
            {
                if (float.TryParse(argument, out float result))
                    return true;
            }
            else if (item.ValueType == ValueType.Int)
            {
                if (Int32.TryParse(argument, out Int32 result))
                    return true;
            }
            else if (item.ValueType == ValueType.Ip)
            {
                return Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b", argument);
            }
            else if (item.ValueType == ValueType.Tcp)
            {
                return Regex(@"(([0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3})|(\w*.\w*.\w*)):[0-9]{1,5}", argument);
            }
            else if (item.ValueType == ValueType.Url)
            {
                return Uri.IsWellFormedUriString(argument, UriKind.RelativeOrAbsolute);
            }
            else if (item.ValueType == ValueType.Date)
            {
                return DateTime.TryParseExact(argument, new[] { "yyyy.MM.dd", "yyyy-MM-dd", "yyyy/MM/dd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
            }
            else if (item.ValueType == ValueType.String)
            {
                return true;
            }
            return false;
        }

        private bool Regex(string regex, string input)
        {
            var regexEngine = new Regex(regex);
            var match = regexEngine.Match(input);
            return match.Success;
        }

        private bool IsFlag(string arg, List<FlagItem> flags, out CommandLineItem item)
        {
            item = null;

            if (flags == null || flags.Count == 0)
                return false;

            if (arg.StartsWith("--"))
            {
                var foundFlags = flags.Where(f => arg.Replace("--", "").ToLower().StartsWith(f.Item.Name.ToLower()) &&
                    IsValidFlag(f, arg));
                if (foundFlags.Count() == 0)
                    throw new ParseException("Illegal argument: " + arg);
                if (foundFlags.Count() > 1)
                    throw new ParseException("Found multiple flags with same name " + arg);
                item = foundFlags.First().Item;
                item.Value = GetValue(foundFlags.First().Item, arg);
                return true;
            }
            else if (arg.StartsWith("-"))
            {
                var parts = arg.Split('=');
                if (parts[0].Length > 2)
                    throw new ParseException("Short name arguments are only one character " + parts[0]);
                var foundFlags = flags.Where(f => f.Item.ShortName == parts[0][1] && IsValidFlag(f, arg));
                if (foundFlags.Count() > 1)
                    throw new ParseException("Found multiple flags with same name" + arg);
                item = foundFlags.First().Item;
                item.Value = GetValue(foundFlags.First().Item, arg);
                return true;
            }
            else
                return false;
        }


        private bool IsCommand(string arg, List<CommandItem> commands, out CommandLineItem commandFound)
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
        private object p;

        public ParseException()
        {
        }

        public ParseException(object p)
        {
            this.p = p;
        }

        public ParseException(string message) : base(message)
        {
        }

        public ParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
