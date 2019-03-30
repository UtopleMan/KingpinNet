using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace KingpinNet
{
    public static class KingpinNetCommandLineConfigurationExtensions
    {
        public static IConfigurationBuilder AddKingpinNetCommandLine(this IConfigurationBuilder configurationBuilder, string[] args)
        {
            configurationBuilder.Add(new KingpinNetCommandLineConfigurationSource()
            {
                Args = args
            });
            return configurationBuilder;
        }
    }

    public class KingpinNetCommandLineConfigurationSource : IConfigurationSource
    {
        public IEnumerable<string> Args { get; set; }
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new KingpinCommandLineConfigurationProvider(Args);
        }
    }


    public class KingpinCommandLineConfigurationProvider : ConfigurationProvider
    {
        private IEnumerable<string> _args;

        public KingpinCommandLineConfigurationProvider(IEnumerable<string> args)
        {
            _args = args;
        }

        public override void Load()
        {
            Data = Kingpin.Parse(_args);
        }
    }
}
