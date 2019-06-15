using KingpinNet;
using System;

namespace Checque
{
    class Program
    {
        static void Main(string[] args)
        {
            Kingpin.ShowHelpOnParsingErrors();
            // Kingpin.ExitOnParsingErrors();
            Kingpin.Flag("registry", "The registry to pull data from");
            Kingpin.Flag("registrys", "The registrys to pull data from");
            Kingpin.Flag("connectionstring", "Database connection string");
            Kingpin.Flag("database", "The name of the database to write data to");
            Kingpin.Flag("token", "The API token to use to connect to checquehistoricaldata.com");

            var schemaCmd = Kingpin.Command("schema", "Database schema commands");
            schemaCmd.Command("create", "Create the schema on the database");

            var basedataCmd = Kingpin.Command("basedata", "Metadata commands");
            basedataCmd.Command("get", "Get basedata for registry(s)");

            var quoteCmd = Kingpin.Command("checque", "checque data commands");
            var quoteGetCmd = quoteCmd.Command("get", "Get checque data for registry(s)");
            quoteGetCmd.Flag<DateTime>("from", "From date");
            quoteGetCmd.Flag<DateTime>("to", "From date");

            var normalizeCmd = Kingpin.Command("normalize", "Normalize checque data");
            normalizeCmd.Flag("symbol", "The symbol to normalize");
            normalizeCmd.Flag("force", "Force generation of normalized data from start of time").IsBool().Short('f');
            normalizeCmd.Flag("all", "Force generation of normalized data for all checque in the registry").IsBool().Short('a');

            var calculateCmd = Kingpin.Command("calculate", "Calculate factors");
            var betasCmd = calculateCmd.Command("betas", "Calculate beta factors");
            betasCmd.Flag("symbol", "Symbol to run calculations on");
            betasCmd.Flag("force", "Force calculation from start of time").IsBool().Short('f');
            betasCmd.Flag("all", "Force generation of beta factors for all checque in the registry").IsBool().Short('a');

            var rankCmd = calculateCmd.Command("rank", "Rank checque against each other in the same registry");
            rankCmd.Flag("force", "Force calculation from start of time").IsBool().Short('f');
            rankCmd.Flag("date", "Calculate for date").IsDate();

            var result = Kingpin.Parse(args);
            Console.ReadLine();
        }
    }
}
