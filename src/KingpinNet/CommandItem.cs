using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KingpinNet
{
    public class CommandItem : CommandLineItem<string>
    {
        internal CommandItem(string path, string name, string help, CommandCategory category) : base(path, name, help, ItemType.Command)
        {
            base.Category = category;
        }
        internal CommandItem(string path, string name, string help) : base(path, name, help, ItemType.Command)
        {
        }
        public FlagItem<string> Flag(string name, string help = "")
        {
            var result = new FlagItem<string>($"{Path}:{name}", name, help);
            base._flags.Add(result);
            return result;
        }
        public ArgumentItem<string> Argument(string name, string help = "")
        {
            var result = new ArgumentItem<string>($"{Path}:{name}", name, help);
            base._arguments.Add(result);
            return result;
        }
        public FlagItem<T> Flag<T>(string name, string help = "")
        {
            var result = new FlagItem<T>($"{Path}:{name}", name, help, ValueTypeConverter.Convert(typeof(T)));
            base._flags.Add(result);
            return result;
        }
        public ArgumentItem<T> Argument<T>(string name, string help = "")
        {
            var result = new ArgumentItem<T>($"{Path}:{name}", name, help, ValueTypeConverter.Convert(typeof(T)));
            base._arguments.Add(result);
            return result;
        }
        public CommandItem Command(string name, string help = "")
        {
            var result = new CommandItem($"{Path}:{name}", name, help);
            base._commands.Add(result);
            return result;
        }
        public new IEnumerable<IItem> Flags
        {
            get
            {
                return base.Flags;
            }
        }
        public new IEnumerable<IItem> Arguments
        {
            get
            {
                return base.Arguments;
            }
        }

        public new CommandItem IsDefault()
        {
            base.IsDefault = true;
            return this;
        }

        public CommandItem IsUrl()
        {
            ValueType = ValueType.Url;
            return this;
        }

        public CommandItem IsRequired()
        {
            Required = true;
            return this;
        }

        public CommandItem IsBool()
        {
            ValueType = ValueType.Bool;
            return this;
        }

        public CommandItem IsInt()
        {
            ValueType = ValueType.Int;
            return this;
        }

        public CommandItem FileExists()
        {
            FileShouldExist = true;
            return this;
        }

        public CommandItem DirectoryExists()
        {
            DirectoryShouldExist = true;
            return this;
        }

        public CommandItem IsIp()
        {
            ValueType = ValueType.Ip;
            return this;
        }

        public CommandItem IsEnum(Type type)
        {
            ValueType = ValueType.Enum;
            TypeOfEnum = type;
            return this;
        }

        public CommandItem IsDuration()
        {
            ValueType = ValueType.Duration;
            return this;
        }

        public CommandItem IsDate()
        {
            ValueType = ValueType.Date;
            return this;
        }


        public CommandItem IsTcp()
        {
            ValueType = ValueType.Tcp;
            return this;
        }

        public CommandItem IsFloat()
        {
            ValueType = ValueType.Float;
            return this;
        }

        public CommandItem Short(char shortName)
        {
            ShortName = shortName;
            return this;
        }

        public CommandItem IsHidden()
        {
            Hidden = true;
            return this;
        }

        public CommandItem SetExamples(params string[] examples)
        {
            Examples = examples;
            return this;
        }

        public CommandItem Default(string defaultValue)
        {
            DefaultValue = defaultValue;
            return this;
        }

        public new CommandItem ValueName(string valueName)
        {
            base.ValueName = valueName;
            return this;
        }
    }
}
