using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class CommandLineItemBuilder
    {
        public CommandLineItem Item;
        public CommandLineItemBuilder IsUrl()
        {
            Item.ValueType = ValueType.Url;
            return this;
        }

        public CommandLineItemBuilder IsRequired()
        {
            Item.IsRequired = true;
            return this;
        }

        public CommandLineItemBuilder IsBool()
        {
            Item.ValueType = ValueType.Bool;
            return this;
        }

        public CommandLineItemBuilder IsInt()
        {
            Item.ValueType = ValueType.Int;
            return this;
        }

        public CommandLineItemBuilder FileExists()
        {
            Item.FileShouldExist = true;
            return this;

        }

        public CommandLineItemBuilder DirectoryExists()
        {
            Item.DirectoryShouldExist = true;
            return this;
        }

        public CommandLineItemBuilder IsIp()
        {
            Item.ValueType = ValueType.Ip;
            return this;
        }

        public CommandLineItemBuilder IsEnum(Type type)
        {
            Item.ValueType = ValueType.Enum;
            Item.TypeOfEnum = type;

            return this;
        }

        public CommandLineItemBuilder IsDuration()
        {
            Item.ValueType = ValueType.Duration;
            return this;
        }

        public CommandLineItemBuilder IsTcp()
        {
            Item.ValueType = ValueType.Tcp;
            return this;
        }

        public CommandLineItemBuilder IsFloat()
        {
            Item.ValueType = ValueType.Float;
            return this;
        }

        public CommandLineItemBuilder Default(string defaultValue)
        {
            Item.DefaultValue = defaultValue;
            return this;
        }
    }
    public class ArgumentBuilder : CommandLineItemBuilder
    {
        public ArgumentBuilder(string name, string help)
        {
            Item = new CommandLineItem { Name = name, Help = help, ItemType = ItemType.Argument };
        }
    }

    public class FlagBuilder : CommandLineItemBuilder
    {
        public FlagBuilder(string name, string help)
        {
            Item = new CommandLineItem { Name = name, Help = help, ItemType = ItemType.Flag };
        }

        public FlagBuilder Short(char shortName)
        {
            Item.ShortName = shortName;
            return this;
        }
    }

    public class CommandBuilder : CommandLineItemBuilder
    {
        public CommandBuilder(string name, string help)
        {
            Item = new CommandLineItem {
                Name = name,
                Help = help,
                ItemType = ItemType.Command,
            };
        }

        public string Name => Item.Name;

        public CommandBuilder HintOptions(params string[] hints)
        {
            Item.HintOptions = hints;
            return this;
        }

        public FlagBuilder Flag(string name, string help)
        {
            var result = new FlagBuilder(name, help);
            Item.Flags.Add(result);
            return result;
        }
        public ArgumentBuilder Argument(string name, string help)
        {
            var result = new ArgumentBuilder(name, help);
            Item.Arguments.Add(result);
            return result;
        }
        public CommandBuilder Command(string name, string help)
        {
            var result = new CommandBuilder(name, help);
            Item.Commands.Add(result);
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
        Float
    }

    public class CommandLineItem
    {
        public readonly List<CommandBuilder> Commands = new List<CommandBuilder>();
        public readonly List<FlagBuilder> Flags = new List<FlagBuilder>();
        public readonly List<ArgumentBuilder> Arguments = new List<ArgumentBuilder>();
        public ItemType ItemType = ItemType.None;
        public ValueType ValueType = ValueType.String;
        public string Name;
        public string Help;
        public bool IsRequired;
        public bool FileShouldExist;
        public bool DirectoryShouldExist;
        public Type TypeOfEnum;
        public string DefaultValue;
        public string Value;
        public string[] HintOptions;
        public bool IsDefault;
        public char ShortName;

        //public CommandLineItem Copy()
        //{
        //    return new CommandLineItem
        //    {
        //        ItemType = ItemType,
        //        Name = Name,
        //        Help = Help,
        //        IsBool = IsBool,
        //        IsUrl = IsUrl,
        //        IsRequired = IsRequired,
        //        IsInt = IsInt,
        //        FileShouldExist = FileShouldExist,
        //        DirectoryShouldExist = DirectoryShouldExist,
        //        IsIp = IsIp,
        //        IsEnum = IsEnum,
        //        IsDuration = IsDuration,
        //        IsTcp = IsTcp,
        //        IsFloat = IsFloat,
        //        EnumType = EnumType,
        //        DefaultValue = DefaultValue,
        //        Value = Value,
        //        HintOptions = HintOptions,
        //        IsDefault = IsDefault,
        //        ShortName = ShortName
        //    };
        //}
    }
}
