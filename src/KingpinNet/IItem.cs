using System;

namespace KingpinNet
{
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
        bool Hidden { get; }
        string ValueName { get; }
        string Help { get; }
        string[] Examples { get; }
        ItemType ItemType { get; }
        string[] Completions { get; }
    }
}
