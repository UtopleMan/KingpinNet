namespace KingpinNet
{
    public class CommandBuilder
    {
        private CommandLineItemBuilder<string> _internalBuilder;

        public CommandLineItem<string> Item => _internalBuilder.Item;

        public CommandBuilder(string name, string help)
        {
            _internalBuilder = new CommandLineItemBuilder<string>(name, help);
        }
        public CommandBuilder HintOptions(params string[] hints)
        {
            Item.HintOptions = hints;
            return this;
        }
        public CommandLineItemBuilder<string> Flag(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help);
            result.Item.ItemType = ItemType.Flag;
            Item.Flags.Add(result);
            return result;
        }
        public CommandLineItemBuilder<string> Argument(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help);
            result.Item.ItemType = ItemType.Argument;
            Item.Arguments.Add(result);
            return result;
        }
        public CommandLineItemBuilder<string> Flag<T>(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help);
            result.Item.ItemType = ItemType.Flag;
            result.Item.ValueType = ValueTypeConverter.Convert(typeof(T));
            Item.Flags.Add(result);
            return result;
        }
        public CommandLineItemBuilder<string> Argument<T>(string name, string help)
        {
            var result = new CommandLineItemBuilder<string>(name, help);
            result.Item.ItemType = ItemType.Argument;
            result.Item.ValueType = ValueTypeConverter.Convert(typeof(T));
            Item.Arguments.Add(result);
            return result;
        }

        public CommandBuilder Command(string name, string help)
        {
            var result = new CommandBuilder(name, help);
            Item.Commands.Add(result);
            return result;
        }

        public CommandBuilder IsDefault()
        {
            Item.IsDefault = true;
            return this;
        }
    }
}
