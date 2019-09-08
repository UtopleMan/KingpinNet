using KingpinNet;
using NUnit.Framework;
using System;
using System.IO;

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
            application.Commands.Add(new CommandItem("run", "This is a command"));

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
            var command = new CommandItem("cmd1", "command1 help");
            command.Command("cmd2", "command2 help");
            application.Commands.Add(command);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["command"], "cmd1:cmd2");
        }

        [Test]
        public void ParseSimpleCommandWithArgument()
        {
            // Arrange
            string[] args = new[] { "run", "an_argument" };
            var application = new KingpinApplication();
            var command = new CommandItem("run", "This is a command");
            application.Commands.Add(command);
            var argument = command.Argument("argument", "This is an argument");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["command"], "run");
            Assert.AreEqual(result["run:argument"], "an_argument");
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
            Assert.AreEqual(result["run:myflag"], "danish");
        }

        [Test]
        public void ParseGlobalFlag()
        {

            // Arrange
            string[] args = new[] { "--myflag=danish" };
            var application = new KingpinApplication();
            var flag = new FlagItem("myflag", "This is the flag of the person");
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
            var flag = new FlagItem("myflag", "This is the flag of the person").IsBool();
            application.Flags.Add((FlagItem)flag);

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
            var flag = new FlagItem("myflag", "This is the flag of the person");
            application.Flags.Add((FlagItem)flag);

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
            var argument = new ArgumentItem("argument", "This is an argument");
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

        [Test]
        public void IsRequiredShouldFailWhenNoArgumentIsGiven()
        {
            // Arrange
            string[] args = new string[0];
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("required", "This is the required flag").IsRequired();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }

        [Test]
        public void ParseBoolSuccess()
        {
            // Arrange
            string[] args = new[] { "--bool=true" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("bool", "").IsBool();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["bool"], "true");
        }

        [Test]
        public void ParseBoolFail()
        {
            // Arrange
            string[] args = new[] { "--bool=notbool" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("bool", "").IsBool();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }

        [Test]
        public void ParseDurationSuccess()
        {
            // Arrange
            string[] args = new[] { "--time=1:00:00" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("time", "").IsDuration();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual("1:00:00", result["time"]);
        }

        [Test]
        public void ParseDurationFail()
        {
            // Arrange
            string[] args = new[] { "--time=notduration" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("time", "").IsDuration();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }

        public enum CheckMe
        {
            Checked,
            NotSoChecked
        }

        [Test]
        public void ParseEnumSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=NotSoChecked" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("flag", "").IsEnum(typeof(CheckMe));
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual("NotSoChecked", result["flag"]);
        }

        [Test]
        public void ParseEnumFail()
        {
            // Arrange
            string[] args = new[] { "--flag=CheckedXXX" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("flag", "").IsEnum(typeof(CheckMe));
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }

        [Test]
        public void ParseFloatSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=1.000" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("flag", "").IsFloat();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual("1.000", result["flag"]);
        }

        [Test]
        public void ParseFloatFail()
        {
            // Arrange
            string[] args = new[] { "--flag=x1.0x" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("flag", "").IsFloat();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }

        [Test]
        public void ParseIntSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=1" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("flag", "").IsInt();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual("1", result["flag"]);
        }

        [Test]
        public void ParseIntFail()
        {
            // Arrange
            string[] args = new[] { "--flag=x1x" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("flag", "").IsInt();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }

        [Test]
        public void ParseUrlSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=http://www.google.com" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("flag", "").IsUrl();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual("http://www.google.com", result["flag"]);
        }

        [Test]
        public void ParseUrlFail()
        {
            // Arrange
            string[] args = new[] { "--flag=http::\\www.google.com_" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("flag", "").IsUrl();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }

        [Test]
        public void ParseIpSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=123.123.123.123" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("flag", "").IsIp();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual("123.123.123.123", result["flag"]);
        }

        [Test]
        public void ParseIpFail()
        {
            // Arrange
            string[] args = new[] { "--flag=123.123.123.xyz" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("flag", "").IsIp();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }
        [Test]
        public void ParseTcpSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=myfancyhostname123:1234" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("flag", "").IsTcp();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual("myfancyhostname123:1234", result["flag"]);
        }

        [Test]
        public void ParseDefaultFlagSuccess()
        {
            // Arrange
            string[] args = new string[0];

            var application = new KingpinApplication();
            var flag = Kingpin.Flag("flag", "").IsTcp().Default("myfancyhostname123:1234");
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual("myfancyhostname123:1234", result["flag"]);
        }

        [Test]
        public void ParseNestedDefaultFlag()
        {
            // Arrange
            string[] args = {"cmd1", "cmd2"};
            var application = new KingpinApplication();
            var command = new CommandItem("cmd1", "command1 help");
            var cmd2 = command.Command("cmd2", "command2 help");
            cmd2.Flag("flg", "").Default("1234");
            application.Commands.Add(command);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["cmd1:cmd2:flg"], "1234");
        }

        [Test]
        public void ParseTcpFail()
        {
            // Arrange
            string[] args = new[] { "--flag=xyz:" };
            var application = new KingpinApplication();
            var flag = Kingpin.Flag("flag", "").IsTcp();
            application.Flags.Add(flag);

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }
        
        [Test]
        public void ParseDirectoryShouldExistSuccess()
        {
            // Arrange
            string directory = System.IO.Path.GetTempPath();
            string[] args = new[] { directory };
            var application = new KingpinApplication();
            var arg = Kingpin.Argument("directory", "").DirectoryExists();
            application.Arguments.Add(arg);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(directory, result["directory"]);
        }

        [Test]
        public void ParseDirectoryShouldExistFail()
        {
            // Arrange
            string directory = System.IO.Path.GetTempPath();
            string[] args = new[] { directory +"\\notgonnaexist" };
            var application = new KingpinApplication();
            var arg = Kingpin.Argument("directory", "").DirectoryExists();
            application.Arguments.Add(arg);

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }
        [Test]
        public void ParseFileShouldExistSuccess()
        {
            // Arrange
            string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
            File.WriteAllText(fileName, "hej");
            string[] args = new[] { fileName };
            var application = new KingpinApplication();
            var arg = Kingpin.Argument("file", "").FileExists();
            application.Arguments.Add(arg);

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(fileName, result["file"]);
        }

        [Test]
        public void ParseFileShouldExistFail()
        {
            // Arrange
            string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
            var application = new KingpinApplication();
            string[] args = new[] { fileName };
            var arg = Kingpin.Argument("file", "").FileExists();
            application.Arguments.Add(arg);

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }
    }
}
