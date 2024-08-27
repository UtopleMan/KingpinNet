using System;

namespace KingpinNet;
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

    public FlagItem<T> IsLong()
    {
        Item.ValueType = ValueType.Long;
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
