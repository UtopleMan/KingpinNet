using System;

namespace KingpinNet
{
    public class ArgumentItem<T> : CommandLineItem<T>
    {
        internal ArgumentItem(string path, string name, string help)
            : base(path, name, help, ItemType.Argument)
        {
        }

        internal ArgumentItem(string path, string name, string help, ValueType valueType)
            : base(path, name, help, ItemType.Argument, valueType)
        {
        }

        public ArgumentItem<T> SetExamples(params string[] examples)
        {
            Examples = examples;
            return this;
        }
        public ArgumentItem<T> SetCompletions(params string[] completions)
        {
            Completions = completions;
            return this;
        }
        public ArgumentItem<T> IsUrl()
        {
            ValueType = ValueType.Url;
            return this;
        }

        public ArgumentItem<T> IsRequired()
        {
            Required = true;
            return this;
        }

        public ArgumentItem<T> IsBool()
        {
            ValueType = ValueType.Bool;
            return this;
        }

        public ArgumentItem<T> IsInt()
        {
            ValueType = ValueType.Int;
            return this;
        }

        public ArgumentItem<T> FileExists()
        {
            FileShouldExist = true;
            return this;
        }

        public ArgumentItem<T> DirectoryExists()
        {
            DirectoryShouldExist = true;
            return this;
        }

        public ArgumentItem<T> IsIp()
        {
            ValueType = ValueType.Ip;
            return this;
        }

        public ArgumentItem<T> IsEnum(Type type)
        {
            ValueType = ValueType.Enum;
            TypeOfEnum = type;
            return this;
        }

        public ArgumentItem<T> IsDuration()
        {
            ValueType = ValueType.Duration;
            return this;
        }

        public ArgumentItem<T> IsDate()
        {
            ValueType = ValueType.Date;
            return this;
        }


        public ArgumentItem<T> IsTcp()
        {
            ValueType = ValueType.Tcp;
            return this;
        }

        public ArgumentItem<T> IsFloat()
        {
            ValueType = ValueType.Float;
            return this;
        }

        public ArgumentItem<T> Short(char shortName)
        {
            ShortName = shortName;
            return this;
        }

        public ArgumentItem<T> Default(string defaultValue)
        {
            DefaultValue = defaultValue;
            return this;
        }

    }
}
