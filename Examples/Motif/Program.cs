using KingpinNet;
using Microsoft.Extensions.Configuration;
using System.Reflection;
namespace TestKingpinNet
{
    class Program
    {
        static void Main(string[] args)
        {
            Kingpin.ExitOnHelp();
            Kingpin.ExitOnParsingErrors();
            Kingpin.ShowHelpOnParsingErrors();
            Kingpin.ExitOnNoArguments();
            Kingpin.ShowHelpOnNoArguments();
            Kingpin.Category("Motif", "Easy generation of entire projects and solutions using Liquid syntax");
            Kingpin.Author("Peter Andersen");
            Kingpin.Version(Assembly.GetEntryAssembly()?.GetName().Version?.ToString());
            Kingpin.Command("create", "Create directories and files based on the definition file");
            Kingpin.Command("update", "Update directories and files based on the definition file");
            Kingpin.Flag("definition", "Motif YAML definition file").Short('d').IsRequired();
            Kingpin.Flag("output", "Output folder for generated directories and files").Short('o').IsRequired();
            Kingpin.Flag("template", "Motif template folder").DirectoryExists().Short('t').IsRequired();

            var configuration = new ConfigurationBuilder().AddKingpinNetCommandLine(args).Build();
            // Kingpin.Parse(args);
            Console.ReadLine();
        }
    }
}
