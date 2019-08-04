namespace KingpinNet
{
    public interface IHelpTemplate
    {
        string TransformText();
        KingpinApplication Application { get; set; }
        CommandItem Command { get; set; }
    }
}

