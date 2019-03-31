using KingpinNet;
using System;

namespace Ping
{
    class Program
    {
        static Flag debug = Kingpin.Flag("debug", "Enable debug mode.").IsBool();
        static Flag timeout = Kingpin.Flag("timeout", "Timeout waiting for ping.").IsRequired().Short('t').IsDuration();
        static Argument ip = Kingpin.Argument("ip", "IP address to ping.").IsRequired().IsIp();
        static Argument count = Kingpin.Argument("count", "Number of packets to send").IsInt();

        static void Main(string[] args)
        {
            Kingpin.Version("0.0.1");
            Kingpin.Author("Joe Malone");
            var result = Kingpin.Parse(args);
            Console.WriteLine($"Would ping: {ip} with timeout {timeout} and count {count}");
            Console.ReadLine();
        }
    }
}
