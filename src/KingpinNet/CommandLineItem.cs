using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KingpinNet;

public class CommandLineItem<T>
{
    internal readonly List<IItem> arguments = [];

    internal readonly List<CommandItem> commands = [];
    internal readonly List<IItem> flags = [];
    public ItemType ItemType = ItemType.None;
    public ValueType ValueType = ValueType.String;

    public CommandLineItem(string path, string name, string help, ItemType itemType) :
        this(path, name, help, itemType, ValueType.String, null)
    {
    }

    public CommandLineItem(string path, string name, string help, ItemType itemType, ValueType valueType,
        CommandCategory category)
    {
        Path = path;
        Name = name;
        Help = help;
        ItemType = itemType;
        ValueType = valueType;
        DefaultValue = string.Empty;
        if (typeof(T).IsEnum)
            TypeOfEnum = typeof(T);
        Suggestions = [];
        Examples = [];
        Category = category;
    }

    public IEnumerable<CommandItem> Commands => commands;
    public IEnumerable<IItem> Flags => flags;
    public IEnumerable<IItem> Arguments => arguments;
    public string Path { get; internal set; }
    public string Name { get; internal set; }
    public string Help { get; internal set; }
    public bool IsRequired { get; internal set; }
    public bool FileShouldExist { get; internal set; }
    public bool DirectoryShouldExist { get; internal set; }
    public Type TypeOfEnum { get; internal set; }
    public string DefaultValue { get; internal set; }
    public T Value { get; internal set; }
    public bool IsDefault { get; internal set; }
    public char ShortName { get; internal set; }
    public Action<T> Action { get; internal set; }
    public bool IsHidden { get; internal set; }
    public bool IsSet { get; internal set; }
    public string[] Examples { get; internal set; }
    public string[] Suggestions { get; internal set; }
    public string ValueName { get; internal set; }
    public string Unit { get; internal set; }
    public string Caution { get; internal set; }
    public CommandCategory Category { get; internal set; }

    internal void ConvertAndSetValue(string value)
    {
        Value = ConvertToType(value);
        Action?.Invoke(Value);
    }

    internal void ConvertAndSetValues(List<string> value)
    {
        Value = ConvertToType(value);
        Action?.Invoke(Value);
    }

    internal T ConvertToType(List<string> values)
    {
        if (ValueType == ValueType.ListOfString) return (T)Convert.ChangeType(values, typeof(List<string>));

        throw new ArgumentException($"'{ValueType}' is not allowed", nameof(values));
    }

    internal T ConvertToType(string value)
    {
        if (typeof(T) == typeof(string)) return (T)Convert.ChangeType(value, typeof(string));

        if (ValueType == ValueType.Bool)
        {
            if (bool.TryParse(value, out var result))
                return (T)Convert.ChangeType(result, typeof(bool));
            throw new ArgumentException($"{value} is not of type bool", nameof(value));
        }

        if (ValueType == ValueType.Duration)
        {
            if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var result))
                return (T)Convert.ChangeType(result, typeof(TimeSpan));
            throw new ArgumentException($"'{value}' is not a duration (Days.Hours:Minutes:Seconds.Milli)",
                nameof(value));
        }

        if (ValueType == ValueType.Enum)
            try
            {
                var resultEnum = Enum.Parse(TypeOfEnum, value);
                return (T)Convert.ChangeType(resultEnum, TypeOfEnum);
            }
            catch (ArgumentException)
            {
                var values = string.Join(",", Enum.GetNames(TypeOfEnum));
                throw new ArgumentException($"'{value}' is not any for the values {values}", nameof(value));
            }

        if (ValueType == ValueType.Float)
        {
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                return (T)Convert.ChangeType(result, typeof(float));
            throw new ArgumentException($"'{value}' is not a float", nameof(value));
        }

        if (ValueType == ValueType.Int)
        {
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                return (T)Convert.ChangeType(result, typeof(int));
            throw new ArgumentException($"'{value}' is not an integer", nameof(value));
        }

        if (ValueType == ValueType.Long)
        {
            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                return (T)Convert.ChangeType(result, typeof(long));
            throw new ArgumentException($"'{value}' is not a long", nameof(value));
        }

        if (ValueType == ValueType.Ip || ValueType == ValueType.Tcp || ValueType == ValueType.String)
            return (T)Convert.ChangeType(value, typeof(string));

        if (ValueType == ValueType.Url)
        {
            if (typeof(T) == typeof(Uri))
            {
                if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uriResult))
                    return (T)Convert.ChangeType(uriResult, typeof(Uri));
                throw new ArgumentException($"'{value}' is not a Uri", nameof(value));
            }

            return (T)Convert.ChangeType(value, typeof(string));
        }

        if (ValueType == ValueType.Date)
        {
            if (typeof(T) == typeof(DateOnly) && DateOnly.TryParseExact(value,
                    new[] { "yyyy.MM.dd", "yyyy-MM-dd", "yyyy/MM/dd" }, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var resultDate))
                return (T)Convert.ChangeType(resultDate, typeof(T));
            if (DateTime.TryParseExact(value,
                    new[]
                    {
                        "yyyy.MM.dd", "yyyy-MM-dd", "yyyy/MM/dd", "yyyy/MM/ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss",
                        "yyyy.MM.ddTHH:mm:ss"
                    }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return (T)Convert.ChangeType(result, typeof(T));
            throw new ArgumentException($"'{value}' is not a date", nameof(value));
        }

        if (ValueType == ValueType.ListOfString)
            return (T)Convert.ChangeType(value.Split(",").ToList(), typeof(List<string>));

        throw new ArgumentException($"'{ValueType}' is not allowed", nameof(value));
    }
}
