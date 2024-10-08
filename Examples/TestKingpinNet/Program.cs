﻿using System;
using System.Collections.Generic;
using KingpinNet;
using Microsoft.Extensions.Configuration;
namespace TestKingpinNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var runCommand = Kingpin.Command("run", "run a command").IsDefault();
            var runUrlCommand = runCommand.Command("url", "Run a URL");
            var runUrlsCommand = runCommand.Command("urls", "Run URLs");
            var runUrlArgument = runUrlCommand.Argument("url", "the url to run").IsRequired().IsUrl();
            var runUrlsArgument = runUrlsCommand.Argument<List<string>>("urls", "the url to run").IsRequired();
            Kingpin.Parse(args);
            var configuration = new ConfigurationBuilder().AddKingpinNetCommandLine(args).Build();
            if (configuration["Command"] == runCommand.Path)
            {
                // Only run command specified on command line
            }

            if (configuration["Command"] == runCommand.Path)
            {
                // run url https://something specified on commandline
            }

            if (runUrlCommand.IsSet)
            {
                // run url https://something specified on commandline
                var url = runUrlArgument.Value;
            }
            Console.ReadLine();
        }
    }
}
