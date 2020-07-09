using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using KingpinNet;
using KingpinNet.Help;
using Microsoft.Extensions.Configuration;

namespace Curl
{
    public class Options
    {
        [Flag("Set connection timeout."), Short('t'), Default("5s")]
        public string Timeout { get; set; }
        [Flag("Set connection timeout."), Short('t')]
        public string Headers { get; set; }
        [Command("GET a resource.")]
        public GetOption Get { get; set; }
    }

    public class GetOption
    {
        [Flag("Test flag")]
        public bool Test { get; set; }
        [Command("Retrieve a URL.")]
        public UrlOption Url { get; set; }
        [Command("Retrieve a file")]
        public FileOption File { get; set; }
    }

    public class FileOption
    {
        [Argument("File to GET."), File(true)]
        public string File { get; set; }
    }

    public class UrlOption
    {
        [Argument("URL to GET."), Url(), Completions("http://localhost", "http://0.0.0.0")]
        public string Url { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Kingpin
                .Version("1.0")
                .Author("Peter Andersen")
                .ApplicationName("curl")
                .ApplicationHelp("An example implementation of curl.")
                .ExitOnHelp()
                .ShowHelpOnParsingErrors()
                .Template(new DockerApplicationHelp(), new DockerCommandHelp())
                .Log((serverity, message, exception) => {
                    //Console.WriteLine($"[{serverity}]\t{message}");
                    //if (exception != null)
                    //{
                    //    Console.WriteLine($"\t{exception}");
                    //}
                })
                .Options(typeof(Options));

            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddKingpinNetCommandLine(args)
                .Build();

            switch (configuration["command"])
            {
                case "get:url":
                    Console.WriteLine($"Getting URL {configuration["get:url:url"]}");
                    break;

                case "post":
                    Console.WriteLine($"Posting to URL {configuration["post:url"]}");
                    break;
            }
        }
    }
}
