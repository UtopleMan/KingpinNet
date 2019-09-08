using KingpinNet;
using System;

namespace Ping
{
    class Program
    {
        static void Main(string[] args)
        {
            FlagItem debug = Kingpin.Flag("debug", "Enable debug mode.").IsBool();
            FlagItem timeout = Kingpin.Flag("timeout", "Timeout waiting for ping.").IsRequired().Short('t').IsDuration();
            ArgumentItem ip = Kingpin.Argument("ip", "IP address to ping.").IsRequired().IsIp();
            ArgumentItem count = Kingpin.Argument("count", "Number of packets to send").IsInt();

            Kingpin.Version("0.0.1");
            Kingpin.Author("Joe Malone");
            var result = Kingpin.Parse(args);
            Console.WriteLine($"Would ping: {ip} with timeout {timeout} and count {count} with debug set to {debug}");
            Console.ReadLine();
        }
    }
}
