using System;
using System.IO;
using KingpinNet;
using Microsoft.Extensions.Configuration;

namespace TestKingpinNet
{
    public enum EnumType
    {
        Option1,
        Option2
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("oooo    oooo  o8o                                     o8o                   ooooo      ooo oooooooooooo ooooooooooooo");
            Console.WriteLine("`888   .8P'   `\"'                                     `\"'                   `888b.     `8' `888'     `8 8'   888   `8");
            Console.WriteLine(" 888  d8'    oooo  ooo. .oo.    .oooooooo oo.ooooo.  oooo  ooo. .oo.         8 `88b.    8   888              888     ");
            Console.WriteLine(" 88888[      `888  `888P\"Y88b  888' `88b   888' `88b `888  `888P\"Y88b        8   `88b.  8   888oooo8         888     ");
            Console.WriteLine(" 888`88b.     888   888   888  888   888   888   888  888   888   888        8     `88b.8   888    \"         888     ");
            Console.WriteLine(" 888  `88b.   888   888   888  `88bod8P'   888   888  888   888   888        8       `888   888       o      888     ");
            Console.WriteLine("o888o  o888o o888o o888o o888o `8oooooo.   888bod8P' o888o o888o o888o      o8o        `8  o888ooooood8     o888o    ");
            Console.WriteLine("                               d\"     YD   888                                                                       ");
            Console.WriteLine("                               \"Y88888P'  o888o                                                                      ");


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
