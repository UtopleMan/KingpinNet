namespace KingpinNet;

public interface IHelpTemplate
{
    KingpinApplication Application { get; set; }
    CommandItem Command { get; set; }
    string TransformText();
}
