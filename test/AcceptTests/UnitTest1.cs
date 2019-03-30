using KingpinNet;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ParseSimpleCommand()
        {
            string[] args = new[] { "run" };
            var command = Kingpin.Command("run", "This is a command");

            var result = Kingpin.Parse(
                new List<CommandBuilder>(new[] { command }),
                new List<FlagBuilder>(),
                new List<ArgumentBuilder>(),
                args);
            Assert.AreEqual(result["command"], "run");
        }
        [Test]
        public void ParseSimpleCommandWithArgument()
        {
            string[] args = new[] { "run", "an_argument" };
            var command = Kingpin.Command("run", "This is a command");
            var argument = command.Argument("argument", "This is an argument");

            var result = Kingpin.Parse(
                new List<CommandBuilder>(new[] { command }),
                new List<FlagBuilder>(),
                new List<ArgumentBuilder>(),
                args);
            Assert.AreEqual(result["command"], "run");
            Assert.AreEqual(result["argument"], "an_argument");
        }
        [Test]
        public void ParseSimpleCommandWithFlag()
        {
            string[] args = new[] { "run", "--myflag=danish" };
            var command = Kingpin.Command("run", "This is a command");
            var flag = command.Flag("myflag", "This is the flag of the person");

            var result = Kingpin.Parse(
                new List<CommandBuilder>(new[] { command }),
                new List<FlagBuilder>(),
                new List<ArgumentBuilder>(),
                args);
            Assert.AreEqual(result["command"], "run");
            Assert.AreEqual(result["myflag"], "danish");
        }

        [Test]
        public void ParseGlobalFlag()
        {
            string[] args = new[] { "--myflag=danish" };
            var flag = Kingpin.Flag("myflag", "This is the flag of the person");

            var result = Kingpin.Parse(
                new List<CommandBuilder>(),
                new List<FlagBuilder>(new[] { flag }),
                new List<ArgumentBuilder>(),
                args);
            Assert.AreEqual(result["myflag"], "danish");
        }

        [Test]
        public void ParseGlobalArgument()
        {
            string[] args = new[] { "hurray" };
            var argument = Kingpin.Argument("argument", "This is an argument");

            var result = Kingpin.Parse(
                new List<CommandBuilder>(),
                new List<FlagBuilder>(),
                new List<ArgumentBuilder>(new[] { argument }),
                args);
            Assert.AreEqual(result["argument"], "hurray");
        }

        [Test]
        public void ParseGlobalShortFlag()
        {
            string[] args = new[] { "-h=help" };
            var flag = Kingpin.Flag("help", "This is the help").Short('h');

            var result = Kingpin.Parse(
                new List<CommandBuilder>(),
                new List<FlagBuilder>(new[] { flag }),
                new List<ArgumentBuilder>(),
                args);
            Assert.AreEqual(result["help"], "help");
        }
    }
}
