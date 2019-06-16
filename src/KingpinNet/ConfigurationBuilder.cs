using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class CommandLineItemBuilder<T>
    {
        public CommandLineItem<T> Item { get; internal set; }

        internal CommandLineItemBuilder(string name, string help, ItemType itemType)
        {
            Item = new CommandLineItem<T>(name, help, itemType);
        }

        internal CommandLineItemBuilder(string name, string help, ItemType itemType, ValueType valueType)
        {
            Item = new CommandLineItem<T>(name, help, itemType, valueType);
        }

        public CommandLineItemBuilder<T> IsUrl()
        {
            Item.ValueType = ValueType.Url;
            return this;
        }

        public CommandLineItemBuilder<T> IsRequired()
        {
            Item.IsRequired = true;
            return this;
        }

        public CommandLineItemBuilder<T> IsBool()
        {
            Item.ValueType = ValueType.Bool;
            return this;
        }

        public CommandLineItemBuilder<T> IsInt()
        {
            Item.ValueType = ValueType.Int;
            return this;
        }

        internal void Action(Action<T> action)
        {
            Item.Action = action;
        }

        public CommandLineItemBuilder<T> FileExists()
        {
            Item.FileShouldExist = true;
            return this;
        }

        public CommandLineItemBuilder<T> DirectoryExists()
        {
            Item.DirectoryShouldExist = true;
            return this;
        }

        public CommandLineItemBuilder<T> IsIp()
        {
            Item.ValueType = ValueType.Ip;
            return this;
        }

        public CommandLineItemBuilder<T> IsEnum(Type type)
        {
            Item.ValueType = ValueType.Enum;
            Item.TypeOfEnum = type;
            return this;
        }

        public CommandLineItemBuilder<T> IsDuration()
        {
            Item.ValueType = ValueType.Duration;
            return this;
        }

        public CommandLineItemBuilder<T> IsDate()
        {
            Item.ValueType = ValueType.Date;
            return this;
        }


        public CommandLineItemBuilder<T> IsTcp()
        {
            Item.ValueType = ValueType.Tcp;
            return this;
        }

        public CommandLineItemBuilder<T> IsFloat()
        {
            Item.ValueType = ValueType.Float;
            return this;
        }

        public CommandLineItemBuilder<T> Short(char shortName)
        {
            Item.ShortName = shortName;
            return this;
        }

        public CommandLineItemBuilder<T> IsHidden()
        {
            Item.IsHidden = true;
            return this;
        }

        public CommandLineItemBuilder<T> Examples(params string[] examples)
        {
            Item.Examples = examples;
            return this;
        }

        public CommandLineItemBuilder<T> Default(string defaultValue)
        {
            Item.DefaultValue = defaultValue;
            return this;
        }

        public override string ToString()
        {
            return Item.Value.ToString();
        }

        public CommandLineItemBuilder<T> ValueName(string valueName)
        {
            Item.ValueName = valueName;
            return this;
        }
    }

    public class CommandBuilder : CommandLineItemBuilder<string>
    {
        internal CommandBuilder(string name, string help) : base(name, help, ItemType.Command)
        {
        }

        public CommandBuilder HintOptions(params string[] hints)
        {
            Item.HintOptions = hints;
            return this;
        }
        public CommandLineItemBuilder<string> Flag(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help, ItemType.Flag);
            Item._flags.Add(result);
            return result;
        }
        public CommandLineItemBuilder<string> Argument(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help, ItemType.Argument);
            Item._arguments.Add(result);
            return result;
        }
        public CommandLineItemBuilder<string> Flag<T>(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help, ItemType.Flag, ValueTypeConverter.Convert(typeof(T)));
            Item._flags.Add(result);
            return result;
        }
        public CommandLineItemBuilder<string> Argument<T>(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help, ItemType.Argument, ValueTypeConverter.Convert(typeof(T)));
            Item._arguments.Add(result);
            return result;
        }
        public CommandBuilder Command(string name, string help)
        {
            var result = new CommandBuilder(name, help);
            Item._commands.Add(result);
            return result;
        }

        public CommandBuilder IsDefault()
        {
            Item.IsDefault = true;
            return this;
        }
    }

    public enum ItemType
    {
        None,
        Command,
        Argument,
        Flag
    }

    public enum ValueType
    {
        String,
        Bool,
        Url,
        Int,
        Ip,
        Enum,
        Duration,
        Tcp,
        Float,
        Date
    }

    public class CommandLineItem<T>
    {
        public CommandLineItem(string name, string help, ItemType itemType): this(name, help, itemType, ValueType.String)
        {
        }

        public CommandLineItem(string name, string help, ItemType itemType, ValueType valueType)
        {
            Name = name;
            Help = help;
            ItemType = itemType;
            ValueType = valueType;
            DefaultValue = String.Empty;
        }

        internal List<CommandBuilder> _commands = new List<CommandBuilder>();
        internal List<CommandLineItemBuilder<string>> _flags = new List<CommandLineItemBuilder<string>>();
        internal List<CommandLineItemBuilder<string>> _arguments = new List<CommandLineItemBuilder<string>>();

        public IEnumerable<CommandBuilder> Commands => _commands;
        public IEnumerable<CommandLineItemBuilder<string>> Flags => _flags;
        public IEnumerable<CommandLineItemBuilder<string>> Arguments => _arguments;


        public ItemType ItemType = ItemType.None;
        public ValueType ValueType = ValueType.String;
        public string Name { get; internal set; }
        public string Help { get; internal set; }
        public bool IsRequired { get; internal set; }
        public bool FileShouldExist { get; internal set; }
        public bool DirectoryShouldExist { get; internal set; }
        public Type TypeOfEnum { get; internal set; }
        public string DefaultValue { get; internal set; }
        public T Value { get; internal set; }
        public string[] HintOptions { get; internal set; }
        public bool IsDefault { get; internal set; }
        public char ShortName { get; internal set; }
        public Action<T> Action { get; internal set; }
        public bool IsHidden { get; internal set; }
        public bool IsSet { get; internal set; }
        public string[] Examples { get; internal set; }
        public string ValueName { get; internal set; }
    }

    public static class ValueTypeConverter
    {
        internal static ValueType Convert(Type type)
        {
            if (type == typeof(Int32) || type == typeof(Int16) || type == typeof(Int64))
                return ValueType.Int;
            if (type == typeof(DateTime))
                return ValueType.Date;
            if (type == typeof(TimeSpan))
                return ValueType.Duration;
            if (type == typeof(float) || type == typeof(decimal))
                return ValueType.Float;
            if (type == typeof(bool))
                return ValueType.Bool;
            if (type == typeof(Uri))
                return ValueType.Url;
            if (type.IsEnum)
                return ValueType.Enum;
            return ValueType.String;
        }
    }
}
