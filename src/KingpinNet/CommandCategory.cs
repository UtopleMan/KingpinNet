namespace KingpinNet;

public class CommandCategory(KingpinApplication application, string name, string description)
{
    public string Name { get; } = name;
    public string Description { get; } = description;

    public CommandItem Command(string name, string help)
    {
        var result = new CommandItem(name, name, help, this);
        application.AddCommand(result);
        return result;
    }
}
