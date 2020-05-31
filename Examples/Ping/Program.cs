using KingpinNet;
using System;

namespace Ping
{
    class Program
    {
        static void Main(string[] args)
        {
            var debug = Kingpin.Flag("debug", "Enable debug mode.").IsBool();
            var timeout = Kingpin.Flag("timeout", "Timeout waiting for ping.").IsRequired().Short('t').IsDuration();
            var ip = Kingpin.Argument("ip", "IP address to ping.").IsRequired().IsIp();
            var count = Kingpin.Argument("count", "Number of packets to send").IsInt();

            Kingpin
                .Version("0.0.1")
                .Author("Joe Malone")
                .ExitOnHelp()
                .ShowHelpOnParsingErrors()
                .Parse(args);

            Console.WriteLine($"Would ping: {ip} with timeout {timeout} and count {count} with debug set to {debug}");
            Console.ReadLine();
        }
    }
}
