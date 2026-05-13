using System.Collections.Generic;
using System.IO;
using System.Linq;
using KingpinNet;
using KingpinNet.AiHelp;
using Xunit;
using Xunit.Abstractions;
using YamlDotNet.Serialization;

namespace Tests;

public class AiHelpYamlTests
{
    private readonly ITestOutputHelper _output;

    public AiHelpYamlTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private static string Emit(KingpinApplication app, CommandItem scope = null)
    {
        var sw = new StringWriter();
        var writer = new KingpinNet.AiHelp.AiHelpYamlWriter(sw);
        writer.Write(app, scope);
        return sw.ToString();
    }

    private static object ParseYaml(string yaml)
    {
        var deserializer = new DeserializerBuilder().Build();
        using var reader = new StringReader(yaml);
        return deserializer.Deserialize(reader);
    }

    private static KingpinApplication NewApp()
    {
        var app = new KingpinApplication();
        app.ApplicationName("execute").Help("Send ECHO_REQUEST packets to a host");
        return app;
    }

    [Fact]
    public void EmptyApp_EmitsCommandSummaryAndConventions()
    {
        var app = NewApp();
        var yaml = Emit(app);
        _output.WriteLine(yaml);

        Assert.Contains("command: execute", yaml);
        Assert.Contains("summary: Send ECHO_REQUEST packets to a host", yaml);
        Assert.Contains("conventions:", yaml);
        // No global_flags / commands / sections when empty
        Assert.DoesNotContain("global_flags:", yaml);
        Assert.DoesNotContain("commands:", yaml);
    }

    [Fact]
    public void OutputAlwaysParsesAsValidYaml()
    {
        var app = NewApp();
        app.Flag("timeout", "Exit after the given timeout").Short('t').IsInt().Unit("seconds").Default("5");
        var run = app.Command("run", "Run a session");
        run.Flag("count", "Stop after N packets").Short('c').IsInt().Unit("packets");
        var now = run.Command("now", "Send immediately");
        now.Argument("destination", "Host to ping").IsRequired();
        app.ExitCode(0, "OK").ExitCode(1, "No reply");
        app.Example("Ping once", "execute run now example.com");
        app.Note("Requires raw-socket privileges");
        app.Prefer("Use --count to bound runs", "any scripted run", "agents hang otherwise");
        app.Avoid("Don't flood", "explicit load test", "can saturate links");

        var yaml = Emit(app);
        _output.WriteLine(yaml);

        var parsed = ParseYaml(yaml);
        Assert.NotNull(parsed);
        Assert.IsAssignableFrom<IDictionary<object, object>>(parsed);
        var dict = (IDictionary<object, object>)parsed;
        Assert.Equal("execute", dict["command"]);
        Assert.True(dict.ContainsKey("global_flags"));
        Assert.True(dict.ContainsKey("commands"));
        Assert.True(dict.ContainsKey("exit_codes"));
        Assert.True(dict.ContainsKey("examples"));
        Assert.True(dict.ContainsKey("notes"));
        Assert.True(dict.ContainsKey("prefer"));
        Assert.True(dict.ContainsKey("avoid"));
        Assert.True(dict.ContainsKey("conventions"));
    }

    [Fact]
    public void Quoting_ColonInHelp_RoundTripsExact()
    {
        var app = NewApp();
        app.Flag("foo", "Format: key:value pairs separated by ':'");
        var yaml = Emit(app);
        _output.WriteLine(yaml);

        var dict = (IDictionary<object, object>)ParseYaml(yaml);
        var globalFlags = (IList<object>)dict["global_flags"];
        var foo = (IDictionary<object, object>)globalFlags[0];
        Assert.Equal("Format: key:value pairs separated by ':'", foo["help"]);
    }

    [Fact]
    public void Quoting_HashInHelp_RoundTrips()
    {
        var app = NewApp();
        app.Flag("color", "ANSI color, e.g. #FF00AA");
        var yaml = Emit(app);
        _output.WriteLine(yaml);
        var dict = (IDictionary<object, object>)ParseYaml(yaml);
        var foo = (IDictionary<object, object>)((IList<object>)dict["global_flags"])[0];
        Assert.Equal("ANSI color, e.g. #FF00AA", foo["help"]);
    }

    [Fact]
    public void Quoting_LeadingDashInHelp_RoundTrips()
    {
        var app = NewApp();
        app.Flag("path", "- start of a list, awkward");
        var yaml = Emit(app);
        var dict = (IDictionary<object, object>)ParseYaml(yaml);
        var foo = (IDictionary<object, object>)((IList<object>)dict["global_flags"])[0];
        Assert.Equal("- start of a list, awkward", foo["help"]);
    }

    [Fact]
    public void Quoting_NewlineInHelp_UsesBlockScalarAndRoundTrips()
    {
        var app = NewApp();
        app.Flag("multi", "Line one\nLine two\nLine three");
        var yaml = Emit(app);
        _output.WriteLine(yaml);

        Assert.Contains("help: |", yaml);
        var dict = (IDictionary<object, object>)ParseYaml(yaml);
        var foo = (IDictionary<object, object>)((IList<object>)dict["global_flags"])[0];
        // YAML block scalars trailing-newline differs across emitters; check the body.
        var help = (string)foo["help"];
        Assert.Contains("Line one", help);
        Assert.Contains("Line two", help);
        Assert.Contains("Line three", help);
    }

    [Fact]
    public void OptionalFieldsOmittedWhenUnset()
    {
        var app = NewApp();
        app.Flag("plain", "");
        var yaml = Emit(app);
        _output.WriteLine(yaml);

        Assert.DoesNotContain("short:", yaml);
        Assert.DoesNotContain("unit:", yaml);
        Assert.DoesNotContain("default:", yaml);
        Assert.DoesNotContain("caution:", yaml);
    }

    [Fact]
    public void TakesValue_IsFalse_OnlyForBool()
    {
        var app = NewApp();
        app.Flag("verbose", "Verbose mode").IsBool();
        app.Flag("count", "Number of packets").IsInt();

        var yaml = Emit(app);
        var dict = (IDictionary<object, object>)ParseYaml(yaml);
        var flags = (IList<object>)dict["global_flags"];

        var verbose = (IDictionary<object, object>)flags[0];
        var count = (IDictionary<object, object>)flags[1];
        Assert.Equal("false", verbose["takes_value"]);
        Assert.Equal("true", count["takes_value"]);
    }

    [Fact]
    public void Required_EmittedExplicitly()
    {
        var app = NewApp();
        app.Argument("dest", "Destination").IsRequired();
        // (Note: arguments live on root in this synthetic app.)
        var yaml = Emit(app);
        _output.WriteLine(yaml);
        Assert.Contains("required: true", yaml);
    }

    [Fact]
    public void Caution_EmittedWhenSet()
    {
        var app = NewApp();
        app.Flag("force", "Force operation").IsBool().Caution("Destructive; requires root.");
        var yaml = Emit(app);
        Assert.Contains("caution: Destructive; requires root.", yaml.Replace("\"", ""));
        // Just confirm round-trip too.
        var dict = (IDictionary<object, object>)ParseYaml(yaml);
        var flag = (IDictionary<object, object>)((IList<object>)dict["global_flags"])[0];
        Assert.Equal("Destructive; requires root.", flag["caution"]);
    }

    [Fact]
    public void NestedCommands_HavePathAsArray()
    {
        var app = NewApp();
        var run = app.Command("run", "Run a session");
        var now = run.Command("now", "Run immediately");

        var yaml = Emit(app);
        _output.WriteLine(yaml);

        Assert.Contains("- path: [run]", yaml);
        Assert.Contains("- path: [run, now]", yaml);
    }

    [Fact]
    public void ScopedEmission_RestrictsToSubtree()
    {
        var app = NewApp();
        var run = app.Command("run", "Run a session");
        var now = run.Command("now", "Run immediately");
        var unrelated = app.Command("other", "Some other command");

        var yaml = Emit(app, scope: run);
        _output.WriteLine(yaml);

        Assert.Contains("command: execute run", yaml);
        Assert.Contains("- path: [run, now]", yaml);
        Assert.DoesNotContain("other", yaml);
    }

    [Fact]
    public void EmptyGlobalSections_AreNotEmitted()
    {
        var app = NewApp();
        var yaml = Emit(app);
        Assert.DoesNotContain("exit_codes:", yaml);
        Assert.DoesNotContain("examples:", yaml);
        Assert.DoesNotContain("notes:", yaml);
        Assert.DoesNotContain("prefer:", yaml);
        Assert.DoesNotContain("avoid:", yaml);
    }

    [Fact]
    public void ConventionsBlock_AlwaysEmitted()
    {
        var app = NewApp();
        var yaml = Emit(app);
        Assert.Contains("conventions:", yaml);
        Assert.Contains("flag_form:", yaml);
        Assert.Contains("inheritance:", yaml);
    }

    [Fact]
    public void HiddenFlagsAndCommands_AreOmitted()
    {
        var app = NewApp();
        app.Flag("internal", "Hidden flag").IsHidden();
        var hidden = app.Command("secret", "Hidden command").IsHidden();
        hidden.Argument("arg", "");
        var yaml = Emit(app);
        Assert.DoesNotContain("--internal", yaml);
        Assert.DoesNotContain("secret", yaml);
    }

    [Fact]
    public void ExitCodesEmitted_AsMapKeyedByInteger()
    {
        var app = NewApp();
        app.ExitCode(0, "OK").ExitCode(2, "DNS error");
        var yaml = Emit(app);
        var dict = (IDictionary<object, object>)ParseYaml(yaml);
        var exit = (IDictionary<object, object>)dict["exit_codes"];
        Assert.Equal("OK", exit["0"]);
        Assert.Equal("DNS error", exit["2"]);
    }

    [Fact]
    public void Examples_HaveIntentAndCommand()
    {
        var app = NewApp();
        app.Example("Send 5 packets", "execute --count=5 example.com");
        var yaml = Emit(app);
        _output.WriteLine(yaml);

        var dict = (IDictionary<object, object>)ParseYaml(yaml);
        var examples = (IList<object>)dict["examples"];
        var first = (IDictionary<object, object>)examples[0];
        Assert.Equal("Send 5 packets", first["intent"]);
        Assert.Equal("execute --count=5 example.com", first["command"]);
    }

    [Fact]
    public void ExamplesAreParseable_AgainstTheirOwnApp()
    {
        // Build a small app, declare an example invocation, and assert the parser
        // accepts each example so authored examples can't silently drift.
        var app = new KingpinApplication();
        app.ApplicationName("execute");
        app.Initialize();
        var run = app.Command("run", "Run a session");
        var now = run.Command("now", "Run immediately");
        now.Argument("destination", "Where to ping").IsRequired();
        now.Flag("count", "Stop after N packets").Short('c').IsInt().Unit("packets");

        app.Example("Ping example.com once", "run now example.com");
        app.Example("Ping with bounded count", "run now --count=5 example.com");

        foreach (var ex in app.Examples)
        {
            // Tokenise on whitespace (good enough for these examples).
            var argv = ex.Command.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            var result = app.Parse(argv);
            Assert.False(result.ParsingFailed, $"Example failed to parse: {ex.Command}\n{result.ErrorMessage}");
        }
    }

    [Fact]
    public void GoldenSnapshot_RepresentativeApp_StructureIsStable()
    {
        var app = new KingpinApplication();
        app.ApplicationName("execute").Help("Send ICMP ECHO_REQUEST packets to a host");

        app.Flag("interval", "Seconds between packets")
            .Short('i').IsFloat().Unit("seconds").Default("1.0");
        app.Flag("timeout", "Exit after the given timeout")
            .Short('t').IsInt().Unit("seconds");

        var run = app.Command("run", "Run an execute session");
        run.Flag("interval", "Seconds between packets")
            .Short('i').IsFloat().Unit("seconds").Default("1.0");
        var now = run.Command("now", "Send packets immediately without scheduling");
        now.Argument("count", "Number of packets to send")
            .IsInt().Unit("packets").IsRequired();
        now.Flag("count", "Stop after sending the given number of packets")
            .Short('c').IsInt().Unit("packets");

        app.ExitCode(0, "At least one reply received");
        app.ExitCode(1, "No replies received");
        app.ExitCode(2, "Error (DNS failure, invalid argument, permission denied)");

        app.Example("Send exactly 5 packets then stop", "execute --count=5 google.com");
        app.Note("Flag semantics differ across platforms; this schema reflects BSD/macOS.");
        app.Prefer("Use --count to bound runs",
            "Any non-interactive or scripted invocation",
            "Without --count, execute runs until interrupted; agents hang.");
        app.Avoid("Don't use --flood",
            "User explicitly asked for stress/load testing",
            "Requires root, can saturate links, may trigger IDS.");

        var yaml = Emit(app);
        _output.WriteLine(yaml);

        // Parse and assert key structural invariants instead of byte-for-byte snapshot.
        var dict = (IDictionary<object, object>)ParseYaml(yaml);
        Assert.Equal("execute", dict["command"]);

        var globalFlags = (IList<object>)dict["global_flags"];
        Assert.Equal(2, globalFlags.Count);

        var commands = (IList<object>)dict["commands"];
        Assert.Single(commands);
        var runCmd = (IDictionary<object, object>)commands[0];
        var runPath = (IList<object>)runCmd["path"];
        Assert.Equal(new object[] { "run" }, runPath.ToArray());

        var runChildren = (IList<object>)runCmd["commands"];
        Assert.Single(runChildren);
        var nowCmd = (IDictionary<object, object>)runChildren[0];
        var nowPath = (IList<object>)nowCmd["path"];
        Assert.Equal(new object[] { "run", "now" }, nowPath.ToArray());

        var nowArgs = (IList<object>)nowCmd["argument"];
        var arg = (IDictionary<object, object>)nowArgs[0];
        Assert.Equal("int", arg["type"]);
        Assert.Equal("packets", arg["unit"]);
        Assert.Equal("true", arg["required"]);

        var nowFlags = (IList<object>)nowCmd["flags"];
        var countFlag = (IDictionary<object, object>)nowFlags[0];
        Assert.Equal("--count", countFlag["long"]);
        Assert.Equal("-c", countFlag["short"]);
        Assert.Equal("packets", countFlag["unit"]);

        var prefer = (IList<object>)dict["prefer"];
        var preferFirst = (IDictionary<object, object>)prefer[0];
        Assert.Equal("Use --count to bound runs", preferFirst["rule"]);

        var avoid = (IList<object>)dict["avoid"];
        var avoidFirst = (IDictionary<object, object>)avoid[0];
        Assert.Equal("Don't use --flood", avoidFirst["rule"]);
    }
}
