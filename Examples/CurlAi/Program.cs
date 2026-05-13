using System;
using KingpinNet;
using Microsoft.Extensions.Configuration;

namespace CurlAi;

class Program
{
    static void Main(string[] args)
    {
        Kingpin
            .Version("1.0")
            .Author("Peter Andersen")
            .ApplicationName("curl-ai")
            .Help("An example implementation of curl exercising the AI-friendly help surface.")
            .ExitOnHelp()
            .ExitOnParseErrors()
            .ShowHelpOnParsingErrors();

        // Global flags — each value-taking flag declares a `unit`.
        Kingpin.Flag("timeout", "Set connection timeout.")
            .Short('t').IsInt().Unit("seconds").Default("5");
        Kingpin.Flag("headers", "Add HTTP headers to the request.")
            .Short('H').Unit("header");
        Kingpin.Flag("verbose", "Verbose request/response logging.")
            .Short('v').IsBool();
        // Destructive flag carries a `caution` describing the risk and root requirement.
        Kingpin.Flag("insecure", "Skip TLS certificate verification.")
            .Short('k').IsBool()
            .Caution("Disables TLS verification; only use against trusted hosts you control.");

        var getCategory = Kingpin.Category("Get", "Category for all the GET commands:");
        var get = getCategory.Command("get", "GET a resource.").IsDefault();
        var getUrl = get.Command("url", "Retrieve a URL.").IsDefault();
        // Required positional argument — exercises required: true in the YAML.
        getUrl.Argument("url", "URL to GET.").IsRequired().IsUrl();

        var getFile = get.Command("file", "Retrieve a file.");
        getFile.Argument("file", "File to retrieve.").IsRequired();

        var post = getCategory.Command("post", "POST a resource.");
        post.Flag("data", "Key-value data to POST.").Short('d');
        post.Flag("retries", "Number of retries on failure.")
            .IsInt().Unit("count").Default("3");
        post.Argument("url", "URL to POST to.").IsRequired().IsUrl();

        var list = Kingpin.Command("list", "LIST a resource.");
        list.Flag("interval", "Polling interval between LIST calls.")
            .IsFloat().Unit("seconds").Default("1.0");
        list.Argument("url", "URL to LIST.").IsRequired().IsUrl();

        // Global AI-help sections (root-only by design).
        Kingpin
            .ExitCode(0, "Success")
            .ExitCode(1, "Network or protocol error")
            .ExitCode(2, "Invalid argument or unknown URL scheme");

        Kingpin
            .Example("Fetch a URL", "curl-ai get url https://example.com")
            .Example("POST JSON to an endpoint",
                "curl-ai post --data=@payload.json https://api.example.com/v1/items")
            .Example("Poll a list endpoint every 2s",
                "curl-ai list --interval=2.0 https://example.com/items");

        Kingpin
            .Note("All commands accept --timeout to bound network operations.")
            .Note("HTTP/HTTPS are supported; other schemes are rejected at parse time.");

        Kingpin
            .Prefer("Always pass --timeout explicitly",
                "Non-interactive or scripted invocations",
                "Without --timeout, curl-ai will block indefinitely on dead peers and stall calling agents.")
            .Prefer("Quote URLs that contain & or ?",
                "Any invocation involving a query string",
                "Bare query strings are interpreted by the shell, not curl-ai.");

        Kingpin
            .Avoid("Don't use --insecure",
                "Hitting a trusted endpoint with a known-bad certificate during development",
                "Disables TLS verification globally; opens the request to MITM attacks.")
            .Avoid("Don't pass shell-expanded glob patterns as URLs",
                "Genuinely fetching multiple URLs",
                "curl-ai accepts exactly one URL per invocation; globs become noise.");

        ParseResult result;
        try
        {
            result = Kingpin.Parse(args);
        }
        catch (ParseException)
        {
            return;
        }

        if (result.ParsingFailed)
            return;

        var configuration = new ConfigurationBuilder().AddKingpinNetCommandLine(args).Build();
        switch (configuration["command"])
        {
            case "get:url":
                Console.WriteLine($"Getting URL {configuration["get:url:url"]}");
                break;
            case "post":
                Console.WriteLine($"Posting to URL {configuration["post:url"]}");
                break;
            case "list":
                Console.WriteLine($"Listing URL {configuration["list:url"]}");
                break;
            default:
                Console.WriteLine("Use --help or --help-ai to see available commands.");
                break;
        }
    }
}
