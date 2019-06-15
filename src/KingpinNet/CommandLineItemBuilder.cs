using System;

namespace KingpinNet
{
    public class CommandLineItemBuilder<S>
    {
        public CommandLineItem<S> Item;

        public CommandLineItemBuilder(string name, string help)
        {
            Item = new CommandLineItem<S>
            {
                Name = name,
                Help = help,
                ItemType = ItemType.Command,
            };
        }

        public CommandLineItemBuilder<S> IsUrl()
        {
            Item.ValueType = ValueType.Url;
            return this;
        }

        public CommandLineItemBuilder<S> IsRequired()
        {
            Item.IsRequired = true;
            return this;
        }

        public CommandLineItemBuilder<S> IsBool()
        {
            Item.ValueType = ValueType.Bool;
            return this;
        }

        public CommandLineItemBuilder<S> IsInt()
        {
            Item.ValueType = ValueType.Int;
            return this;
        }

        internal void Action(Action<string> action)
        {
            Item.Action = action;
        }

        public CommandLineItemBuilder<S> FileExists()
        {
            Item.FileShouldExist = true;
            return this;
        }

        public CommandLineItemBuilder<S> DirectoryExists()
        {
            Item.DirectoryShouldExist = true;
            return this;
        }

        public CommandLineItemBuilder<S> IsIp()
        {
            Item.ValueType = ValueType.Ip;
            return this;
        }

        public CommandLineItemBuilder<S> IsEnum(Type type)
        {
            Item.ValueType = ValueType.Enum;
            Item.TypeOfEnum = type;
            return this;
        }

        public CommandLineItemBuilder<S> IsDuration()
        {
            Item.ValueType = ValueType.Duration;
            return this;
        }

        public CommandLineItemBuilder<S> IsDate()
        {
            Item.ValueType = ValueType.Date;
            return this;
        }


        public CommandLineItemBuilder<S> IsTcp()
        {
            Item.ValueType = ValueType.Tcp;
            return this;
        }

        public CommandLineItemBuilder<S> IsFloat()
        {
            Item.ValueType = ValueType.Float;
            return this;
        }

        public CommandLineItemBuilder<S> Short(char shortName)
        {
            Item.ShortName = shortName;
            return this;
        }

        public CommandLineItemBuilder<S> IsHidden()
        {
            Item.IsHidden = true;
            return this;
        }

        public CommandLineItemBuilder<S> Examples(params string[] examples)
        {
            Item.Examples = examples;
            return this;
        }

        public CommandLineItemBuilder<S> Default(string defaultValue)
        {
            Item.DefaultValue = defaultValue;
            return this;
        }

        public override string ToString()
        {
            if (Item.Value == null)
                return "";
            return Item.Value.ToString();
        }
    }
}
