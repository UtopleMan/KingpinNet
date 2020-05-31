using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace KingpinNet
{
    public static class KingpinNetCommandLineConfigurationExtensions
    {
        public static IConfigurationBuilder AddKingpinNetCommandLine(this IConfigurationBuilder configurationBuilder, string[] args,
            KingpinApplication application = null)
        {
            configurationBuilder.Add(new KingpinNetCommandLineConfigurationSource()
            {
                KingpinApplication = application,
                Args = args
            });
            return configurationBuilder;
        }
    }

    public class KingpinNetCommandLineConfigurationSource : IConfigurationSource
    {
        public IEnumerable<string> Args { get; set; }
        public KingpinApplication KingpinApplication { get; set; }
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new KingpinCommandLineConfigurationProvider(Args, KingpinApplication);
        }
    }


    public class KingpinCommandLineConfigurationProvider : ConfigurationProvider
    {
        private KingpinApplication _kingpinApplication;
        private IEnumerable<string> _args;

        public KingpinCommandLineConfigurationProvider(IEnumerable<string> args, KingpinApplication application)
        {
            _kingpinApplication = application;
            _args = args;
        }

        public override void Load()
        {
            if (_kingpinApplication == null)
                Data = Kingpin.Parse(_args).Result;
            else
                Data = _kingpinApplication.Parse(_args).Result;
        }
    }
}
