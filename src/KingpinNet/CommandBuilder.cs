using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class CommandItem : BaseItem<string>
    {
        internal CommandItem(string path, string name, string help) : base(path, name, help, ItemType.Command)
        {
        }

        public CommandItem HintOptions(params string[] hints)
        {
            Item.HintOptions = hints;
            return this;
        }
        public FlagItem<string> Flag(string name, string help)
        {
            var result = new FlagItem<string>($"{Path}:{name}", name, help);
            Item._flags.Add(result);
            return result;
        }
        public ArgumentItem<string> Argument(string name, string help)
        {
            var result = new ArgumentItem<string>($"{Path}:{name}", name, help);
            Item._arguments.Add(result);
            return result;
        }
        public FlagItem<T> Flag<T>(string name, string help)
        {
            var result = new FlagItem<T>($"{Path}:{name}", name, help, ValueTypeConverter.Convert(typeof(T)));
            Item._flags.Add(result);
            return result;
        }
        public ArgumentItem<T> Argument<T>(string name, string help)
        {
            var result = new ArgumentItem<T>($"{Path}:{name}", name, help, ValueTypeConverter.Convert(typeof(T)));
            Item._arguments.Add(result);
            return result;
        }
        public CommandItem Command(string name, string help)
        {
            var result = new CommandItem($"{Path}:{name}", name, help);
            Item._commands.Add(result);
            return result;
        }

        public IEnumerable<CommandItem> Commands
        {
            get
            {
                return Item._commands;
            }
        }
        public IEnumerable<IItem> Flags
        {
            get
            {
                return Item.Flags;
            }
        }
        public IEnumerable<IItem> Arguments
        {
            get
            {
                return Item.Arguments;
            }
        }

        public CommandItem IsDefault()
        {
            Item.IsDefault = true;
            return this;
        }

        public CommandItem IsUrl()
        {
            Item.ValueType = ValueType.Url;
            return this;
        }

        public CommandItem IsRequired()
        {
            Item.IsRequired = true;
            return this;
        }

        public CommandItem IsBool()
        {
            Item.ValueType = ValueType.Bool;
            return this;
        }

        public CommandItem IsInt()
        {
            Item.ValueType = ValueType.Int;
            return this;
        }

        public CommandItem FileExists()
        {
            Item.FileShouldExist = true;
            return this;
        }

        public CommandItem DirectoryExists()
        {
            Item.DirectoryShouldExist = true;
            return this;
        }

        public CommandItem IsIp()
        {
            Item.ValueType = ValueType.Ip;
            return this;
        }

        public CommandItem IsEnum(Type type)
        {
            Item.ValueType = ValueType.Enum;
            Item.TypeOfEnum = type;
            return this;
        }

        public CommandItem IsDuration()
        {
            Item.ValueType = ValueType.Duration;
            return this;
        }

        public CommandItem IsDate()
        {
            Item.ValueType = ValueType.Date;
            return this;
        }


        public CommandItem IsTcp()
        {
            Item.ValueType = ValueType.Tcp;
            return this;
        }

        public CommandItem IsFloat()
        {
            Item.ValueType = ValueType.Float;
            return this;
        }

        public CommandItem Short(char shortName)
        {
            Item.ShortName = shortName;
            return this;
        }

        public CommandItem IsHidden()
        {
            Item.IsHidden = true;
            return this;
        }

        public CommandItem SetExamples(params string[] examples)
        {
            Item.Examples = examples;
            return this;
        }

        public CommandItem Default(string defaultValue)
        {
            Item.DefaultValue = defaultValue;
            return this;
        }

        public CommandItem ValueName(string valueName)
        {
            Item.ValueName = valueName;
            return this;
        }
    }
}
