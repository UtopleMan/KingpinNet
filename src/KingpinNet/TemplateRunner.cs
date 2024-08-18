using DotLiquid;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KingpinNet;
public interface ITemplateRunner
{
    Task<string> Run(string templateName, dynamic application);
}
public class TemplateRunner : ITemplateRunner
{
    private readonly IConsole console;

    public TemplateRunner(IConsole console)
    {
        this.console = console;
    }
    public async Task<string> Run(string templateName, dynamic application)
    {

        try
        {
            var templateContent = ReadResource(templateName);
            var template = Template.Parse(templateContent);
            var result = template.Render(Hash.FromAnonymousObject(new { application = new LiquidDynamic(application) }));
            return await Task.FromResult(result);
        }
        catch (DotLiquid.Exceptions.SyntaxException exception)
        {
            console.Out.WriteLine($"Syntax error in template {templateName}: {exception.Message}");
            return await Task.FromResult("");
        }
        catch (Exception exception)
        {
            console.Out.WriteLine(exception.ToString());
            return await Task.FromResult("");
        }
    }

    private string ReadResource(string resourceName)
    {
        var assembly = typeof(TemplateRunner).Assembly;
        using Stream stream = assembly.GetManifestResourceStream(resourceName);
        using StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
