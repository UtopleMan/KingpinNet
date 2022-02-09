using KingpinNet;
using System;

namespace Motif
{
    internal class Program
    {
        public static FlagItem<string> MicroServiceName { get; private set; }
        public static FlagItem<string> MicroServiceNamespace { get; private set; }
        public static CommandItem CreateCommand { get; private set; }
        public static CommandItem UpdateCommand { get; private set; }
        public static FlagItem<string> DefinitionFile { get; private set; }
        public static FlagItem<string> OutputPath { get; private set; }
        public static FlagItem<string> TemplatePath { get; private set; }
        static void Main(string[] args)
        {
            Kingpin.ExitOnHelp();
            Kingpin.ExitOnParsingErrors();
            Kingpin.ShowHelpOnParsingErrors();
            Kingpin.ExitOnNoArguments();
            Kingpin.ShowHelpOnNoArguments();
            Kingpin.Category("Motif", "Easy generation of entire projects and solutions using Liquid syntax");
            Kingpin.Author("Peter Andersen");
            Kingpin.Version("1.0.0");
            CreateCommand = Kingpin.Command("create", "Create directories and files based on the definition file");
            UpdateCommand = Kingpin.Command("update", "Update directories and files based on the definition file");
            DefinitionFile = Kingpin.Flag("definition", "Motif YAML definition file").Short('d').IsRequired();
            OutputPath = Kingpin.Flag("output", "Output folder for generated directories and files").Short('o').IsRequired();
            TemplatePath = Kingpin.Flag("template", "Motif template folder").DirectoryExists().Short('t').IsRequired();

            Kingpin.Parse(args);
        }
    }
}
