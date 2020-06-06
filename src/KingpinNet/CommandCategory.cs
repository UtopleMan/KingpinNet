namespace KingpinNet
{
    public class CommandCategory
    {
        public CommandCategory(KingpinApplication application, string name, string description)
        {
            this.application = application;
            this.Name = name;
            this.Description = description;
        }

        private KingpinApplication application;

        public string Name { get; }
        public string Description { get; }
        public CommandItem Command(string name, string help)
        {
            var result = new CommandItem(name, name, help, this);
            application.AddCommand(result);
            return result;
        }
    }
}
