using System;

namespace KingpinNet
{
    public class FlagItem<T> : CommandLineItem<T>
    {
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
            base.ValueName = valueName;
            return this;
        }

        public FlagItem<T> SetExamples(params string[] examples)
        {
            Examples = examples;
            return this;
        }

        public FlagItem<T> IsUrl()
        {
            ValueType = ValueType.Url;
            return this;
        }

        public FlagItem<T> IsRequired()
        {
            Required = true;
            return this;
        }

        public FlagItem<T> IsBool()
        {
            ValueType = ValueType.Bool;
            return this;
        }

        public FlagItem<T> IsInt()
        {
            ValueType = ValueType.Int;
            return this;
        }

        public FlagItem<T> FileExists()
        {
            FileShouldExist = true;
            return this;
        }

        public FlagItem<T> DirectoryExists()
        {
            DirectoryShouldExist = true;
            return this;
        }

        public FlagItem<T> IsIp()
        {
            ValueType = ValueType.Ip;
            return this;
        }

        public FlagItem<T> IsEnum(Type type)
        {
            ValueType = ValueType.Enum;
            TypeOfEnum = type;
            return this;
        }

        public FlagItem<T> IsDuration()
        {
            ValueType = ValueType.Duration;
            return this;
        }

        public FlagItem<T> IsDate()
        {
            ValueType = ValueType.Date;
            return this;
        }

        public FlagItem<T> IsTcp()
        {
            ValueType = ValueType.Tcp;
            return this;
        }

        public FlagItem<T> IsFloat()
        {
            ValueType = ValueType.Float;
            return this;
        }

        public FlagItem<T> Short(char shortName)
        {
            ShortName = shortName;
            return this;
        }

        public FlagItem<T> IsHidden()
        {
            Hidden = true;
            return this;
        }
        public FlagItem<T> Run(Action<CommandLineItem<T>> action)
        {
            Action = action;
            return this;
        }

        public FlagItem<T> Default(string defaultValue)
        {
            DefaultValue = defaultValue;
            return this;
        }
    }
}
