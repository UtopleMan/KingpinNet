using System;
using System.Collections.Generic;

namespace KingpinNet
{
    public class Kingpin
    {
        private static readonly KingpinApplication Application = new KingpinApplication();

        static Kingpin()
        {
            Application.ApplicationName(AppDomain.CurrentDomain.FriendlyName);
            Application.ApplicationHelp("");
            Application.Initialize();
            //Flag("completion-script-bash", "Generate completion script for bash.").IsHidden().Action(a.generateBashCompletionScript).Bool()
            //Flag("completion-script-zsh", "Generate completion script for ZSH.").IsHidden().Action(a.generateZSHCompletionScript).Bool()
        }

        public static KingpinApplication ShowHelpOnParsingErrors()
        {
            return Application.ShowHelpOnParsingErrors();
        }

        public static KingpinApplication ExitOnParsingErrors()
        {
            return Application.ExitOnParsingErrors();
        }

        public static KingpinApplication ExitOnHelp()
        {
            return Application.ExitOnHelp();
        }

        public static KingpinApplication Author(string author)
        {
            return Application.Author(author);
        }

        public static KingpinApplication Version(string version)
        {
            return Application.Version(version);
        }

        public static CommandItem Command(string name, string help)
        {
            return Application.Command(name, help);
        }

        public static FlagItem<string> Flag(string name, string help)
        {
            return Application.Flag(name, help);
        }

        public static ArgumentItem<string> Argument(string name, string help)
        {
            return Application.Argument(name, help);
        }

        public static FlagItem<T> Flag<T>(string name, string help)
        {
            return Application.Flag<T>(name, help);
        }

        public static ArgumentItem<T> Argument<T>(string name, string help)
        {
            return Application.Argument<T>(name, help);
        }

        public static IDictionary<string, string> Parse(IEnumerable<string> args)
        {
            var parser = new Parser(Application);
            Application.AddCommandHelpOnAllCommands();
            try
            {
                return parser.Parse(args);
            }
            catch (ParseException exception)
            {
                if (Application.HelpShownOnParsingErrors)
                {
                    Console.WriteLine(exception.Message);
                    foreach (var error in exception.Errors)
                        Console.WriteLine($"   {error}");
                    Application.GenerateHelp();
                }
                throw;
            }
        }

        //public static IDictionary<string, string> Parse(List<CommandBuilder> commands, List<FlagItemBuilder> flags,
        //    List<ArgumentItemBuilder> arguments, IEnumerable<string> args)
        //{
        //    var parser = new Parser(Application);
        //    return parser.Parse(args);
        //}

    }
}
