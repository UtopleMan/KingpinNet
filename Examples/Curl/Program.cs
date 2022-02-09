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
                .Help("An example implementation of curl.")
                .ExitOnHelp()
                .ShowHelpOnParsingErrors()
                .Log((serverity, message, exception) => {
                    Console.WriteLine($"[{serverity}]\t{message}");
                    if (exception != null)
                    {
                        Console.WriteLine($"\t{exception}");
                    }
                });

            var timeout = Kingpin.Flag("timeout", "Set connection timeout.").Short('t').Default("5s"); // .Duration()
            var headers = Kingpin.Flag("headers", "Add HTTP headers to the request.").Short('H'); // .PlaceHolder("HEADER=VALUE");

            var getCategory = Kingpin.Category("Get", "Category for all the GET commands:");

            var get = getCategory.Command("get", "GET a resource.").IsDefault();
            var getFlag = get.Flag("test", "Test flag").IsBool();
            var getUrl = get.Command("url", "Retrieve a URL.").IsDefault();
            var getUrlUrl = getUrl.Argument("url", "URL to GET.").IsRequired().IsUrl().SetSuggestions("http://localhost", "http://192.168.1.1", "http://0.0.0.0");
            var getFile = get.Command("file", "Retrieve a file.");
            var getFileFile = getFile.Argument("file", "File to retrieve.").IsRequired(); // .ExistingFile()

            var post = getCategory.Command("post", "POST a resource.");
            var postData = post.Flag("data", "Key-value data to POST").Short('d'); // .PlaceHolder("KEY:VALUE").StringMap()
            var postBinaryFile = post.Flag("data-binary", "File with binary data to POST."); // .IsFile();
            var postUrl = post.Argument("url", "URL to POST to.").IsRequired().IsUrl().SetSuggestions("http://localhost", "http://192.168.1.1", "http://0.0.0.0");

            var list = Kingpin.Command("list", "LIST a resource.");
            var listData = list.Flag("data", "Key-value data to LIST").Short('d'); // .PlaceHolder("KEY:VALUE").StringMap()
            var listBinaryFile = list.Flag("data-binary", "File with binary data to LIST."); // .IsFile();
            var listUrl = list.Argument("url", "URL to LIST to.").IsRequired().IsUrl().SetSuggestions("http://localhost", "http://192.168.1.1", "http://0.0.0.0");

            var configuration = new ConfigurationBuilder().AddKingpinNetCommandLine(args).Build();

            switch (configuration["command"])
            {
                case "get:url":
                    Console.WriteLine($"Getting URL {configuration["get:url:url"]}");
                    break;

                case "post":
                    Console.WriteLine($"Posting to URL {configuration["post:url"]}");
                    break;
                default:
                    throw new Exception("Didn't understand commandline");
            }
        }
    }
}
