using System;
using System.Collections.Generic;
using System.Globalization;

namespace KingpinNet
{

    public class CommandLineItem<T>: IItem
    {
        public CommandLineItem(string path, string name, string help, ItemType itemType):
            this(path, name, help, itemType, ValueType.String, null)
        {
        }
        public CommandLineItem(string path, string name, string help, ItemType itemType, ValueType valueType):
            this(path, name, help, itemType, valueType, null)
        {

        }
        public CommandLineItem(string path, string name, string help, ItemType itemType,
            ValueType valueType, CommandCategory category)
        {
            Path = path;
            Name = name;
            Help = help;
            ItemType = itemType;
            ValueType = valueType;
            DefaultValue = String.Empty;
            if (typeof(T).IsEnum)
                TypeOfEnum = typeof(T);
            Completions = new string[0];
            Examples = new string[0];
            Category = category;
        }

        internal List<CommandItem> _commands = new List<CommandItem>();
        internal List<IItem> _flags = new List<IItem>();
        internal List<IItem> _arguments = new List<IItem>();

        public IEnumerable<CommandItem> Commands => _commands;
        public IEnumerable<IItem> Flags => _flags;
        public IEnumerable<IItem> Arguments => _arguments;
        public ItemType ItemType { get; set; }
        public ValueType ValueType { get; set; }
        public string Path { get; internal set; }
        public string Name { get; internal set; }
        public string Help { get; internal set; }
        public bool Required { get; internal set; }
        public bool FileShouldExist { get; internal set; }
        public bool DirectoryShouldExist { get; internal set; }
        public Type TypeOfEnum { get; internal set; }
        public string DefaultValue { get; internal set; }
        public T Value { get; internal set; }
        public bool IsDefault { get; internal set; }
        public char ShortName { get; internal set; }
        public bool Hidden { get; internal set; }
        public bool IsSet { get; set; }
        public string[] Examples { get; internal set; }
        public string[] Completions { get; internal set; }
        public string ValueName { get; internal set; }
        public CommandCategory Category { get; internal set; }
        public Action<CommandLineItem<T>> Action { get; set; }

        internal void ConvertAndSetValue(string value)
        {
            if (typeof(T) == typeof(string))
            {
                Value = (T)Convert.ChangeType(value, typeof(string));
            }
            else if (ValueType == ValueType.Bool)
            {
                if (bool.TryParse(value, out var result))
                    Value = (T)Convert.ChangeType(result, typeof(bool));
                else
                    throw new ArgumentException($"{value} is not of type bool", nameof(value));
            }
            else if (ValueType == ValueType.Duration)
            {
                if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var result))
                    Value = (T)Convert.ChangeType(result, typeof(TimeSpan));
                else
                    throw new ArgumentException($"'{value}' is not a duration (Days.Hours:Minutes:Seconds.Milli)", nameof(value));
            }
            else if (ValueType == ValueType.Enum)
            {
                try
                {
                    var resultEnum = Enum.Parse(TypeOfEnum, value);
                    Value = (T)Convert.ChangeType(resultEnum, TypeOfEnum);
                }
                catch (ArgumentException)
                {
                    var values = string.Join(",", Enum.GetNames(TypeOfEnum));
                    throw new ArgumentException($"'{value}' is not any for the values {values}", nameof(value));
                }
            }
            else if (ValueType == ValueType.Float)
            {
                if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                    Value = (T)Convert.ChangeType(result, typeof(float));
                else
                    throw new ArgumentException($"'{value}' is not a float", nameof(value));
            }
            else if (ValueType == ValueType.Int)
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                    Value = (T)Convert.ChangeType(result, typeof(int));
                else
                    throw new ArgumentException($"'{value}' is not an integer", nameof(value));
            }
            else if (ValueType == ValueType.Ip || ValueType == ValueType.Tcp || ValueType == ValueType.String)
            {
                Value = (T)Convert.ChangeType(value, typeof(string));
            }
            else if (ValueType == ValueType.Url)
            {
                if (typeof(T) == typeof(Uri))
                {
                    if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uriResult))
                        Value = (T) Convert.ChangeType(uriResult, typeof(Uri));
                    else
                        throw new ArgumentException($"'{value}' is not a Uri", nameof(value));
                }
                else
                {
                    Value = (T)Convert.ChangeType(value, typeof(string));
                }
            }
            else if (ValueType == ValueType.Date)
            {
                if (DateTime.TryParseExact(value, new[] { "yyyy.MM.dd", "yyyy-MM-dd", "yyyy/MM/dd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    Value = (T)Convert.ChangeType(result, typeof(DateTime));
                else
                    throw new ArgumentException($"'{value}' is not a date", nameof(value));
            }
            Action?.Invoke(this);
        }
        public string StringValue
        {
            get
            {
                return Value.ToString();
            }
            set
            {
                ConvertAndSetValue(value);
            }
        }
    }
}
