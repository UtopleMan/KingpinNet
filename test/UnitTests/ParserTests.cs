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
            application.Command("run", "This is a command");

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
            var command = application.Command("cmd1", "command1 help");
            command.Command("cmd2", "command2 help");

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
            var command = application.Command("run", "This is a command");
            command.Argument("argument", "This is an argument");

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
            var command = application.Command("run", "This is a command");
            command.Flag("myflag", "This is the flag of the person");

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
            application.Flag("myflag", "This is the flag of the person");

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
            application.Flag("myflag", "This is the flag of the person").IsBool();

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
            var flag = application.Flag("myflag", "This is the flag of the person");

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
            application.Argument("argument", "This is an argument");

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
            application.Flag("help", "This is the help").Short('h');

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
            var flag = application.Flag("required", "This is the required flag").IsRequired();

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }

        [Test]
        public void ParseStrongTypedBoolSuccess()
        {
            // Arrange
            string[] args = new[] { "--bool=true" };
            var application = new KingpinApplication();
            var flag = application.Flag<bool>("bool", "");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(flag.Value, true);
        }

        [Test]
        public void ParseBoolSuccess()
        {
            // Arrange
            string[] args = new[] { "--bool=true" };
            var application = new KingpinApplication();
            application.Flag("bool", "").IsBool();

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
            var flag = application.Flag("bool", "").IsBool();

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }

        [Test]
        public void ParseStrongTypedDurationSuccess()
        {
            // Arrange
            string[] args = new[] { "--time=1:00:00" };
            var application = new KingpinApplication();
            var flag = application.Flag<TimeSpan>("time", "");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(TimeSpan.FromHours(1), flag.Value);
        }

        [Test]
        public void ParseDurationSuccess()
        {
            // Arrange
            string[] args = new[] { "--time=1:00:00" };
            var application = new KingpinApplication();
            application.Flag("time", "").IsDuration();

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual("1:00:00", result["time"]);
        }

        [Test]
        public void ParseDurationSuccessWithDays()
        {
            // Arrange
            string[] args = new[] { "--time=1.1:00:00" };
            var application = new KingpinApplication();
            application.Flag("time", "").IsDuration();

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual("1.1:00:00", result["time"]);
        }


        [Test]
        public void ParseDurationFail()
        {
            // Arrange
            string[] args = new[] { "--time=notduration" };
            var application = new KingpinApplication();
            var flag = application.Flag("time", "").IsDuration();

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
        public void ParseStrongTypedEnumSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=NotSoChecked" };
            var application = new KingpinApplication();
            var flag = application.Flag<CheckMe>("flag", "");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(CheckMe.NotSoChecked, flag.Value);
        }
        [Test]
        public void ParseEnumSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=NotSoChecked" };
            var application = new KingpinApplication();
            application.Flag("flag", "").IsEnum(typeof(CheckMe));

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
            var flag = application.Flag("flag", "").IsEnum(typeof(CheckMe));

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }

        [Test]
        public void ParseStrongTypedFloatSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=1.000" };
            var application = new KingpinApplication();
            var flag = application.Flag<float>("flag", "");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(1.000f, flag.Value);
        }

        [Test]
        public void ParseFloatSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=1.000" };
            var application = new KingpinApplication();
            application.Flag("flag", "").IsFloat();

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
            var flag = application.Flag("flag", "").IsFloat();

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }

        [Test]
        public void ParseStrongTypedIntSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=1" };
            var application = new KingpinApplication();
            var flag = application.Flag<int>("flag", "");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(1, flag.Value);
        }

        [Test]
        public void ParseIntSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=1" };
            var application = new KingpinApplication();
            application.Flag("flag", "").IsInt();

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
            var flag = application.Flag("flag", "").IsInt();

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }

        [Test]
        public void ParseIntReturnsRichErrorMessage()
        {
            // Arrange
            string[] args = new[] { "--flag=x1x" };
            var application = new KingpinApplication();
            application.Flag("flag", "").IsInt();

            // Act
            var subject = new Parser(application);

            try
            {
                subject.Parse(args);
            }
            catch (ParseException exception)
            {
                Assert.IsTrue(exception.Errors.Count == 1);
                Assert.AreEqual("'x1x' is not an integer", exception.Errors[0]);
            }
        }

        [Test]
        public void ParseStrongTypedUriSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=http://www.google.com" };
            var application = new KingpinApplication();
            var flag = application.Flag<Uri>("flag", "");

            // Act
            var subject = new Parser(application);
            subject.Parse(args);

            // Assert
            Assert.AreEqual(new Uri("http://www.google.com", UriKind.RelativeOrAbsolute).AbsolutePath,
                flag.Value.AbsolutePath);
        }

        [Test]
        public void ParseUrlSuccess()
        {
            // Arrange
            string[] args = new[] { "--flag=http://www.google.com" };
            var application = new KingpinApplication();
            application.Flag("flag", "").IsUrl();

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
            var flag = application.Flag("flag", "").IsUrl();

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
            application.Flag("flag", "").IsIp();

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
            var flag = application.Flag("flag", "").IsIp();

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
            application.Flag("flag", "").IsTcp();

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual("myfancyhostname123:1234", result["flag"]);
        }
        [Test]
        public void ParseTcpFail()
        {
            // Arrange
            string[] args = new[] { "--flag=xyz:" };
            var application = new KingpinApplication();
            var flag = application.Flag("flag", "").IsTcp();

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }
        [Test]
        public void ParseDefaultFlagSuccess()
        {
            // Arrange
            string[] args = new string[0];

            var application = new KingpinApplication();
            application.Flag("flag", "").IsTcp().Default("myfancyhostname123:1234");

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
            var command = application.Command("cmd1", "command1 help");
            var cmd2 = command.Command("cmd2", "command2 help");
            cmd2.Flag("flg", "").Default("1234");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["cmd1:cmd2:flg"], "1234");
        }


        
        [Test]
        public void ParseDirectoryShouldExistSuccess()
        {
            // Arrange
            string directory = Path.GetTempPath();
            string[] args = new[] { directory };
            var application = new KingpinApplication();
            application.Argument("directory", "").DirectoryExists();

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
            string directory = Path.GetTempPath();
            string[] args = new[] { directory +"\\notgonnaexist" };
            var application = new KingpinApplication();
            var arg = application.Argument("directory", "").DirectoryExists();

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }
        [Test]
        public void ParseFileShouldExistSuccess()
        {
            // Arrange
            string fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
            File.WriteAllText(fileName, "hej");
            string[] args = new[] { fileName };
            var application = new KingpinApplication();
            application.Argument("file", "").FileExists();

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
            string fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
            var application = new KingpinApplication();
            string[] args = new[] { fileName };
            var arg = application.Argument("file", "").FileExists();

            // Act
            var subject = new Parser(application);
            Assert.Throws<ParseException>(() => subject.Parse(args));
        }
    }
}
