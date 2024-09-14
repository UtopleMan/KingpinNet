//
using System;

namespace KingpinNet;

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
    public ArgumentItem<T> SetSuggestions(params string[] suggestions)
    {
        Item.Suggestions = suggestions;
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

    public ArgumentItem<T> IsLong()
    {
        Item.ValueType = ValueType.Long;
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
