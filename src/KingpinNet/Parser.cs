using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace KingpinNet
{
    public class Parser
    {
        private readonly List<CommandBuilder> _commands;
        private readonly List<FlagBuilder> _globalFlags;
        private readonly List<ArgumentBuilder> _globalArguments;
        private Dictionary<string, string> _result;
        private List<string> _args;
        private int _currentItem;

        public Parser(List<CommandBuilder> commands, List<FlagBuilder> flags, List<ArgumentBuilder> arguments)
        {
            _commands = commands;
            _globalFlags = flags;
            _globalArguments = arguments;
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
                    Push("command", commandFound.Name);
                    CommandFound(commandFound);
                }
                else if (IsFlag(_args[_currentItem], _globalFlags, out CommandLineItem flagFound))
                    Push(flagFound.Name, flagFound.Value);
                else if (IsArgument(_args[_currentItem], _globalArguments, out CommandLineItem argumentFound))
                    Push(argumentFound.Name, argumentFound.Value);
                else
                    throw new ParseException("Something went wrong");
            }
        }

        private void CommandFound(CommandLineItem command)
        {
            while (_currentItem < _args.Count)
            {
                if (IsFlag(_args[_currentItem], _globalFlags, out CommandLineItem flagFound))
                    Push(flagFound.Name, flagFound.Value);
                if (IsFlag(_args[_currentItem], command.Flags, out flagFound))
                    Push(flagFound.Name, flagFound.Value);
                else if (IsArgument(_args[_currentItem], command.Arguments, out CommandLineItem argumentFound))
                    Push(argumentFound.Name, argumentFound.Value);
                else
                    throw new ParseException("Something is out of place");
            }
        }

        private void Push(string name, string value)
        {
            _result.Add(name, value);
            _currentItem++;
        }

        private bool IsArgument(string arg, List<ArgumentBuilder> arguments, out CommandLineItem item)
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
                    return "true";
                }
                else
                    throw new ParseException("Not a boolean");

            if (parts.Length == 2)
                return parts[1];

            throw new ParseException("Found too many = signs");
        }

        private bool IsValidArgument(ArgumentBuilder argument, string arg)
        {
            return IsValidItem(argument.Item, arg);
        }

        private bool IsValidFlag(FlagBuilder flag, string arg)
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
            if (item.ValueType == ValueType.String)
            {
                return true;
            }
            if (item.ValueType == ValueType.Bool)
            {
                if (bool.TryParse(argument, out bool result))
                    return true;
            }
            else if (item.DirectoryShouldExist)
            {
                return true;
            }
            else if (item.FileShouldExist)
            {
                return true;
            }
            else if (item.ValueType == ValueType.Duration)
            {
                return true;
            }
            else if (item.ValueType == ValueType.Enum)
            {
                return true;
            }
            else if (item.ValueType == ValueType.Float)
            {
                return true;
            }
            else if (item.ValueType == ValueType.Int)
            {
                return true;
            }
            else if (item.ValueType == ValueType.Ip)
            {
                return true;
            }
            else if (item.ValueType == ValueType.Tcp)
            {
                return true;
            }
            else if (item.ValueType == ValueType.Url)
            {
                return true;
            }
            return false;
        }

        private bool IsFlag(string arg, List<FlagBuilder> flags, out CommandLineItem item)
        {
            item = null;

            if (flags == null || flags.Count == 0)
                return false;

            if (arg.StartsWith("--"))
            {
                var foundFlags = flags.Where(f => arg.Replace("--", "").ToLower().StartsWith(f.Item.Name.ToLower()) &&
                    IsValidFlag(f, arg));
                if (foundFlags.Count() > 1)
                    throw new ParseException("Found multiple flags with same name");
                item = foundFlags.First().Item;
                item.Value = GetValue(foundFlags.First().Item, arg);
                return true;
            }
            else if (arg.StartsWith("-"))
            {
                var parts = arg.Split('=');
                if (parts[0].Length > 2)
                    throw new ParseException("Short name arguments are only one character");
                var foundFlags = flags.Where(f => f.Item.ShortName == parts[0][1] && IsValidFlag(f, arg));
                if (foundFlags.Count() > 1)
                    throw new ParseException("Found multiple flags with same name");
                item = foundFlags.First().Item;
                item.Value = GetValue(foundFlags.First().Item, arg);
                return true;
            }
            else
                return false;
        }


        private bool IsCommand(string arg, List<CommandBuilder> commands, out CommandLineItem commandFound)
        {
            commandFound = null;
            foreach (var command in commands)
                if (arg.ToLower() == command.Name.ToLower())
                {
                    //if (_command != null)
                    //    throw new ParseException("Command already set to " + _command.Name);

                    commandFound = command.Item;
                    return true;
                }
            return false;
        }

        //MainParse
        //{
        //    *(CommandFound || FlagFound || Argument)
        //}

        //FlagFound
        //{
        //   AddFlag
        //}

        //CommandFound
        //{
        //    SetCommand
        //    *(FlagFound || ArgumentFound || CommandFlagFound)
        //}

        //ArgumentFound
        //{
        //    SetArgument
        //}

        //CommandFlagFound
        //{
        //    AddFlag
        //}

    }

    [Serializable]
    internal class ParseException : Exception
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
