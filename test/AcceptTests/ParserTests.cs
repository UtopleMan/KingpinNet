using KingpinNet;
using NUnit.Framework;

namespace Tests
{

    public class ParserTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ParseSimpleCommand()
        {
            // Arrange
            string[] args = new[] { "run" };
            var application = new KingpinApplication();
            application.Commands.Add(new CommandBuilder("run", "This is a command"));

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["command"], "run");
        }

        [Test]
        public void ParseNestedCommand()
        {
            // Arrange
            string[] args = new[] { "cmd1", "cmd2" };
            var application = new KingpinApplication();
            var command = new CommandBuilder("cmd1", "command1 help");
            command.Command("cmd2", "command2 help");
            application.Commands.Add(command);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["command"], "cmd1-cmd2");
        }

        [Test]
        public void ParseSimpleCommandWithArgument()
        {
            // Arrange
            string[] args = new[] { "run", "an_argument" };
            var application = new KingpinApplication();
            var command = new CommandBuilder("run", "This is a command");
            application.Commands.Add(command);
            var argument = command.Argument("argument", "This is an argument");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["command"], "run");
            Assert.AreEqual(result["argument"], "an_argument");
        }
        [Test]
        public void ParseSimpleCommandWithFlag()
        {
            // Arrange
            string[] args = new[] { "run", "--myflag=danish" };
            var application = new KingpinApplication();
            var command = Kingpin.Command("run", "This is a command");
            var flag = command.Flag("myflag", "This is the flag of the person");
            application.Commands.Add(command);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["command"], "run");
            Assert.AreEqual(result["myflag"], "danish");
        }

        [Test]
        public void ParseGlobalFlag()
        {

            // Arrange
            string[] args = new[] { "--myflag=danish" };
            var application = new KingpinApplication();
            var flag = new FlagBuilder("myflag", "This is the flag of the person");
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["myflag"], "danish");
        }

        [Test]
        public void ParseGlobalBoolFlag()
        {

            // Arrange
            string[] args = new[] { "--myflag" };
            var application = new KingpinApplication();
            var flag = new FlagBuilder("myflag", "This is the flag of the person").IsBool();
            application.Flags.Add((FlagBuilder)flag);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["myflag"], "true");
        }

        [Test]
        public void ParseGlobalNonBoolFlagThrowsException()
        {

            // Arrange
            string[] args = new[] { "--myflag" };
            var application = new KingpinApplication();
            var flag = new FlagBuilder("myflag", "This is the flag of the person");
            application.Flags.Add((FlagBuilder)flag);

            // Act
            var subject = new Parser(application);
            // Assert
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }

        [Test]
        public void ParseGlobalArgument()
        {
            // Arrange
            string[] args = new[] { "hurray" };
            var application = new KingpinApplication();
            var argument = new ArgumentBuilder("argument", "This is an argument");
            application.Arguments.Add(argument);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["argument"], "hurray");
        }

        [Test]
        public void ParseGlobalShortFlag()
        {
            // Arrange
            string[] args = new[] { "-h=help" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("help", "This is the help").Short('h');
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["help"], "help");
        }
    }
}
