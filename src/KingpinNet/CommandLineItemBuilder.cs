using System;

namespace KingpinNet
{
    public class FlagItem<T> : BaseItem<T>
    {
        public T Value {
            get
            {
                return Item.Value;
            }
        }

        internal FlagItem(string path, string name, string help)
            : base(path, name, help, ItemType.Flag)
        {
        }

        internal FlagItem(string path, string name, string help, ValueType valueType)
            : base(path, name, help, ItemType.Flag, valueType)
        {
        }

        public new FlagItem<T> ValueName(string valueName)
        {
            Item.ValueName = valueName;
            return this;
        }

        public FlagItem<T> SetExamples(params string[] examples)
        {
            Item.Examples = examples;
            return this;
        }

        public FlagItem<T> IsUrl()
        {
            Item.ValueType = ValueType.Url;
            return this;
        }

        public FlagItem<T> IsRequired()
        {
            Item.IsRequired = true;
            return this;
        }

        public FlagItem<T> IsBool()
        {
            Item.ValueType = ValueType.Bool;
            return this;
        }

        public FlagItem<T> IsInt()
        {
            Item.ValueType = ValueType.Int;
            return this;
        }

        public FlagItem<T> FileExists()
        {
            Item.FileShouldExist = true;
            return this;
        }

        public FlagItem<T> DirectoryExists()
        {
            Item.DirectoryShouldExist = true;
            return this;
        }

        public FlagItem<T> IsIp()
        {
            Item.ValueType = ValueType.Ip;
            return this;
        }

        public FlagItem<T> IsEnum(Type type)
        {
            Item.ValueType = ValueType.Enum;
            Item.TypeOfEnum = type;
            return this;
        }

        public FlagItem<T> IsDuration()
        {
            Item.ValueType = ValueType.Duration;
            return this;
        }

        public FlagItem<T> IsDate()
        {
            Item.ValueType = ValueType.Date;
            return this;
        }

        public FlagItem<T> IsTcp()
        {
            Item.ValueType = ValueType.Tcp;
            return this;
        }

        public FlagItem<T> IsFloat()
        {
            Item.ValueType = ValueType.Float;
            return this;
        }

        public FlagItem<T> Short(char shortName)
        {
            Item.ShortName = shortName;
            return this;
        }

        public FlagItem<T> IsHidden()
        {
            Item.IsHidden = true;
            return this;
        }

        public FlagItem<T> Default(string defaultValue)
        {
            Item.DefaultValue = defaultValue;
            return this;
        }
    }

    public class ArgumentItem<T> : BaseItem<T>
    {
        internal ArgumentItem(string path, string name, string help)
            : base(path, name, help, ItemType.Argument)
        {
        }

        internal ArgumentItem(string path, string name, string help, ValueType valueType)
            : base(path, name, help, ItemType.Argument, valueType)
        {
        }

        public T Value
        {
            get
            {
                return Item.Value;
            }
        }

        public ArgumentItem<T> SetExamples(params string[] examples)
        {
            Item.Examples = examples;
            return this;
        }

        public ArgumentItem<T> IsUrl()
        {
            Item.ValueType = ValueType.Url;
            return this;
        }

        public ArgumentItem<T> IsRequired()
        {
            Item.IsRequired = true;
            return this;
        }

        public ArgumentItem<T> IsBool()
        {
            Item.ValueType = ValueType.Bool;
            return this;
        }

        public ArgumentItem<T> IsInt()
        {
            Item.ValueType = ValueType.Int;
            return this;
        }

        public ArgumentItem<T> FileExists()
        {
            Item.FileShouldExist = true;
            return this;
        }

        public ArgumentItem<T> DirectoryExists()
        {
            Item.DirectoryShouldExist = true;
            return this;
        }

        public ArgumentItem<T> IsIp()
        {
            Item.ValueType = ValueType.Ip;
            return this;
        }

        public ArgumentItem<T> IsEnum(Type type)
        {
            Item.ValueType = ValueType.Enum;
            Item.TypeOfEnum = type;
            return this;
        }

        public ArgumentItem<T> IsDuration()
        {
            Item.ValueType = ValueType.Duration;
            return this;
        }

        public ArgumentItem<T> IsDate()
        {
            Item.ValueType = ValueType.Date;
            return this;
        }


        public ArgumentItem<T> IsTcp()
        {
            Item.ValueType = ValueType.Tcp;
            return this;
        }

        public ArgumentItem<T> IsFloat()
        {
            Item.ValueType = ValueType.Float;
            return this;
        }

        public ArgumentItem<T> Short(char shortName)
        {
            Item.ShortName = shortName;
            return this;
        }

        public ArgumentItem<T> Default(string defaultValue)
        {
            Item.DefaultValue = defaultValue;
            return this;
        }

    }

    public interface IItem
    {
        string DefaultValue { get; }
        string StringValue { get; set; }
        bool IsSet { get; set; }
        string Name { get; }
        char ShortName { get; }
        bool Required { get; }
        ValueType ValueType { get; }
        bool DirectoryShouldExist { get; }
        bool FileShouldExist { get; }
        Type TypeOfEnum { get; }
        Action<string> Action { get; }
        bool Hidden { get; }
        string ValueName { get; }
        string Help { get; }
        string[] Examples { get; }
        ItemType ItemType { get; }
    }

    public class BaseItem<T> : IItem
    {
        protected CommandLineItem<T> Item { get; set; }

        internal BaseItem(string path, string name, string help, ItemType itemType, ValueType valueType = ValueType.String)
        {
            Item = new CommandLineItem<T>(path, name, help, itemType, valueType);
        }

        public string Path => Item.Path;

        public bool IsSet {
            get
            {
                return Item.IsSet;
            }
            set
            {
                Item.IsSet = value;
            }
        }

        public bool Required => Item.IsRequired;

        public string DefaultValue => Item.DefaultValue;

        public ValueType ValueType
        {
            get
            {
                return Item.ValueType;
            }
        }

        public string StringValue
        {
            get
            {
                return Item.Value.ToString();
            }
            set
            {
                Item.ConvertAndSetValue(value);
            }
        }

        public string Name => Item.Name;

        public char ShortName => Item.ShortName;

        public bool DirectoryShouldExist => Item.DirectoryShouldExist;

        public bool FileShouldExist => Item.FileShouldExist;

        public Type TypeOfEnum => Item.TypeOfEnum;

        Action<string> IItem.Action => null;

        public bool Hidden => Item.IsHidden;

        public string ValueName => Item.ValueName;

        public string Help => Item.Help;

        public string[] Examples => Item.Examples;

        public ItemType ItemType => Item.ItemType;

        public void Action(Action<T> action)
        {
            Item.Action = action;
        }

        public override string ToString()
        {
            return Item.Value.ToString();
        }
    }
}
