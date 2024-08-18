using System;

namespace KingpinNet;


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
    string[] Suggestions { get; }
}

public class BaseItem<T> : IItem
{
    protected CommandLineItem<T> Item { get; set; }

    internal BaseItem(string path, string name, string help, ItemType itemType, ValueType valueType = ValueType.String)
    {
        Item = new CommandLineItem<T>(path, name, help, itemType, valueType, null);
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

    public string[] Suggestions => Item.Suggestions;

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
