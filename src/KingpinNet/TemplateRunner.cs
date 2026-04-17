using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KingpinNet;
public interface ITemplateRunner
{
    Task<string> Run(string templateName, IDictionary<string, object> application);
}
public class TemplateRunner : ITemplateRunner
{
    private readonly IConsole console;

    public TemplateRunner(IConsole console)
    {
        this.console = console;
    }
    public async Task<string> Run(string templateName, IDictionary<string, object> application)
    {
        try
        {
            var templateContent = ReadResource(templateName);
            var template = Template.ParseLiquid(templateContent);
            if (template.HasErrors)
            {
                console.Out.WriteLine($"Syntax error in template {templateName}: {string.Join(", ", template.Messages)}");
                return await Task.FromResult("");
            }

            var appObject = new ScriptObject();
            foreach (var kvp in application)
                appObject[kvp.Key] = kvp.Value;

            var root = new ScriptObject();
            root["application"] = appObject;

            var context = new LiquidTemplateContext { MemberRenamer = member => member.Name };
            context.PushGlobal(root);
            var result = template.Render(context);
            return await Task.FromResult(result);
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
