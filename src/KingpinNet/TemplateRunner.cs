using DotLiquid;

namespace KingpinNet
{
    public interface ITemplateRunner
    {
        Task<string> Run(string templateName, dynamic application);
    }
    public class TemplateRunner : ITemplateRunner
    {
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
                Console.WriteLine($"Syntax error in template {templateName}: {exception.Message}");
                return await Task.FromResult("");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
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
}
