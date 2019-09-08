using System;
using KingpinNet;
using Microsoft.Extensions.Configuration;

namespace Integration
{
    class Program
    {
        static void Main(string[] args)
        {
            var runCommand = Kingpin.Command("run", "run a command").IsDefault();
            var generalFlag = Kingpin.Flag("general", "a general flag").Short('g').IsInt().IsUrl().IsIp().IsEnum(typeof(EnumType)).IsDuration().IsTcp().IsFloat().Default("5s");

            var runUrlCommand = runCommand.Command("url", "Run a URL");

            var runUrlArgument = runUrlCommand.Argument("url", "the url to run").IsRequired().IsUrl().FileExists().DirectoryExists();
            var runUrlFlag = runUrlCommand.Flag("flag", "a flag").Short('f').IsInt().IsUrl().IsIp().IsEnum(typeof(EnumType)).IsDuration().IsTcp().IsFloat().Default("5s");
            var runUrlSwitch = runUrlCommand.Flag("switch", "a switch").IsBool();

            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().AddKingpinNetCommandLine(args).Build();
            if (configuration["help"] == "true")
                return;
            Console.WriteLine(configuration["flag"]);
            Console.ReadLine();
        }
    }
}
