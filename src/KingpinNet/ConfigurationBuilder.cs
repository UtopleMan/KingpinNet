using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class CommandLineItemBuilder<T>
    {
        public CommandLineItem Item;
        public T IsUrl()
        {
            Item.ValueType = ValueType.Url;
            var obj = (object)this;
            return (T)obj;
        }

        public T IsRequired()
        {
            Item.IsRequired = true;
            var obj = (object)this;
            return (T)obj;
        }

        public T IsBool()
        {
            Item.ValueType = ValueType.Bool;
            var obj = (object)this;
            return (T)obj;
        }

        public T IsInt()
        {
            Item.ValueType = ValueType.Int;
            var obj = (object)this;
            return (T)obj;
        }

        internal void Action(Action<string> action)
        {
            Item.Action = action;
        }

        public T FileExists()
        {
            Item.FileShouldExist = true;
            var obj = (object)this;
            return (T)obj;

        }

        public T DirectoryExists()
        {
            Item.DirectoryShouldExist = true;
            var obj = (object)this;
            return (T)obj;
        }

        public T IsIp()
        {
            Item.ValueType = ValueType.Ip;
            var obj = (object)this;
            return (T)obj;
        }

        public T IsEnum(Type type)
        {
            Item.ValueType = ValueType.Enum;
            Item.TypeOfEnum = type;

            var obj = (object)this;
            return (T)obj;
        }

        public T IsDuration()
        {
            Item.ValueType = ValueType.Duration;
            var obj = (object)this;
            return (T)obj;
        }

        public T IsDate()
        {
            Item.ValueType = ValueType.Date;
            var obj = (object)this;
            return (T)obj;
        }


        public T IsTcp()
        {
            Item.ValueType = ValueType.Tcp;
            var obj = (object)this;
            return (T)obj;
        }

        public T IsFloat()
        {
            Item.ValueType = ValueType.Float;
            var obj = (object)this;
            return (T)obj;
        }

        public T Short(char shortName)
        {
            Item.ShortName = shortName;
            var obj = (object)this;
            return (T)obj;
        }

        public T IsHidden()
        {
            Item.IsHidden = true;
            var obj = (object)this;
            return (T)obj;
        }

        public T Examples(params string[] examples)
        {
            Item.Examples = examples;
            var obj = (object)this;
            return (T)obj;
        }

        public T Default(string defaultValue)
        {
            Item.DefaultValue = defaultValue;
            var obj = (object)this;
            return (T)obj;
        }



        public override string ToString()
        {
            return Item?.Value;
        }
    }
    public class ArgumentItem : CommandLineItemBuilder<ArgumentItem>
    {
        public ArgumentItem(string name, string help)
        {
            Item = new CommandLineItem { Name = name, Help = help, ItemType = ItemType.Argument };
        }
    }

    public class FlagItem : CommandLineItemBuilder<FlagItem>
    {
        public FlagItem(string name, string help)
        {
            Item = new CommandLineItem { Name = name, Help = help, ItemType = ItemType.Flag };
        }
    }

    public class CommandItem : CommandLineItemBuilder<CommandItem>
    {
        public CommandItem(string name, string help)
        {
            Item = new CommandLineItem
            {
                Name = name,
                Help = help,
                ItemType = ItemType.Command,
            };
        }
        public CommandItem HintOptions(params string[] hints)
        {
            Item.HintOptions = hints;
            return this;
        }
        public FlagItem Flag(string name, string help)
        {
            var result = new FlagItem(name, help);
            Item.Flags.Add(result);
            return result;
        }
        public ArgumentItem Argument(string name, string help)
        {
            var result = new ArgumentItem(name, help);
            Item.Arguments.Add(result);
            return result;
        }
        public CommandItem Command(string name, string help)
        {
            var result = new CommandItem(name, help);
            Item.Commands.Add(result);
            return result;
        }

        public CommandItem IsDefault()
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

    public class CommandLineItem
    {
        public readonly List<CommandItem> Commands = new List<CommandItem>();
        public readonly List<FlagItem> Flags = new List<FlagItem>();
        public readonly List<ArgumentItem> Arguments = new List<ArgumentItem>();
        public ItemType ItemType = ItemType.None;
        public ValueType ValueType = ValueType.String;
        public string Name;
        public string Help;
        public bool IsRequired;
        public bool FileShouldExist;
        public bool DirectoryShouldExist;
        public Type TypeOfEnum;
        public string DefaultValue = "";
        public string Value;
        public string[] HintOptions;
        public bool IsDefault;
        public char ShortName;
        public Action<string> Action;
        public bool IsHidden;
        public bool IsSet;
        public string[] Examples;
    }
}
