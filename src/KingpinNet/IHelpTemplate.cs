namespace KingpinNet
{
    public interface IHelpTemplate
    {
        string TransformText();
        KingpinApplication Application { get; set; }
        CommandBuilder Command { get; set; }
    }
}

