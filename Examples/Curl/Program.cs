using System;
using KingpinNet;
using Microsoft.Extensions.Configuration;

namespace Curl
{
    class Program
    {
        static void Main(string[] args)
        {
            Kingpin
                .Version("1.0")
                .Author("Peter Andersen")
                .ApplicationName("curl")
                .ApplicationHelp("An example implementation of curl.")
                .ShowHelpOnParsingErrors();

            var timeout = Kingpin.Flag("timeout", "Set connection timeout.").Short('t').Default("5s"); // .Duration()
            var headers = Kingpin.Flag("headers", "Add HTTP headers to the request.").Short('H'); // .PlaceHolder("HEADER=VALUE");
            var get = Kingpin.Command("get", "GET a resource.").IsDefault();
            var getFlag = get.Flag("test", "Test flag").IsBool();
            var getUrl = get.Command("url", "Retrieve a URL.").IsDefault();
            var getUrlUrl = getUrl.Argument("url", "URL to GET.").IsRequired().IsUrl();
            var getFile = get.Command("file", "Retrieve a file.");
            var getFileFile = getFile.Argument("file", "File to retrieve.").IsRequired(); // .ExistingFile()
            var post = Kingpin.Command("post", "POST a resource.");
            var postData = post.Flag("data", "Key-value data to POST").Short('d'); // .PlaceHolder("KEY:VALUE").StringMap()
            var postBinaryFile = post.Flag("data-binary", "File with binary data to POST."); // .IsFile();
            var postUrl = post.Argument("url", "URL to POST to.").IsRequired().IsUrl();


            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().AddKingpinNetCommandLine(args).Build();

            switch (configuration["command"])
            {
                case "get:url":
                    Console.WriteLine($"Getting URL {configuration["get:url:url"]}");
                    break;

                case "post":
                    Console.WriteLine($"Posting to URL {configuration["post:url"]}");
                    break;
            }

            Console.ReadLine();
        }
    }
}
