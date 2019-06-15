using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class CommandLineItem<T>
    {
        public readonly List<CommandBuilder> Commands = new List<CommandBuilder>();
        public readonly List<CommandLineItemBuilder<string>> Flags = new List<CommandLineItemBuilder<string>>();
        public readonly List<CommandLineItemBuilder<string>> Arguments = new List<CommandLineItemBuilder<string>>();
        public ItemType ItemType = ItemType.None;
        public ValueType ValueType = ValueType.String;
        public string Name;
        public string Help;
        public bool IsRequired;
        public bool FileShouldExist;
        public bool DirectoryShouldExist;
        public Type TypeOfEnum;
        public string DefaultValue = "";
        public T Value;
        public string[] HintOptions;
        public bool IsDefault;
        public char ShortName;
        public Action<string> Action;
        public bool IsHidden;
        public bool IsSet;
        public string[] Examples;
    }
}
