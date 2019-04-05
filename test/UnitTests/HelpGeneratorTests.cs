using System;
using KingpinNet;
using NUnit.Framework;
using System.IO;
using System.Text;

namespace Tests
{
    public class WriterStub : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
    public class HelpGeneratorTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void WriteEmpty()
        {
            // Arrange
            var application = new KingpinApplication();
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("usage:"));
        }

        [Test]
        public void WriteApplictaionName()
        {
            // Arrange
            var application = new KingpinApplication();
            application.Name = "testapp.exe";
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("usage: testapp.exe"));
        }
        [Test]
        public void WriteApplictaionHelp()
        {
            // Arrange
            var application = new KingpinApplication();
            application.Help = "This is the glorious test app";
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            TestContext.Out.WriteLine(result);
            Assert.IsTrue(result.Contains(Environment.NewLine + Environment.NewLine + "This is the glorious test app"+ Environment.NewLine + Environment.NewLine));
        }
        [Test]
        public void WriteGlobalFlag()
        {
            // Arrange
            var application = new KingpinApplication();
            application.Flags.Add(new FlagItem("flag", "flag help"));
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("Flags:\r\n      --flag   flag help\r\n"));
        }

        [Test]
        public void WriteGlobalFlagWithShortName()
        {
            // Arrange
            var application = new KingpinApplication();
            application.Flags.Add(new FlagItem("flag", "flag help").Short('f'));
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("Flags:\r\n  -f, --flag   flag help\r\n"));
        }
        [Test]
        public void WriteGlobalFlags()
        {
            // Arrange
            var application = new KingpinApplication();
            application.Flags.Add(new FlagItem("flag1", "flag1 help").Short('f'));
            application.Flags.Add(new FlagItem("flag2", "flag2 help").Short('g'));
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("usage: [<flags>] \r\n\r\n"));
            Assert.IsTrue(result.Contains("Flags:\r\n  -f, --flag1   flag1 help\r\n  -g, --flag2   flag2 help\r\n"));
        }

        [Test]
        public void WriteGlobalArgument()
        {
            // Arrange
            var application = new KingpinApplication();
            application.Arguments.Add(new ArgumentItem("arg", "arg help"));
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("usage: [<arg>]\r\n\r\n"));
            Assert.IsTrue(result.Contains("Args:\r\n  [<arg>]   arg help\r\n"));
        }

        [Test]
        public void WriteGlobalArguments()
        {
            // Arrange
            var application = new KingpinApplication();
            application.Arguments.Add(new ArgumentItem("arg1", "arg1 help"));
            application.Arguments.Add(new ArgumentItem("arg2", "arg2 help"));
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("usage: [<args> ...]\r\n\r\n"));
            Assert.IsTrue(result.Contains("Args:\r\n  [<arg1>]   arg1 help\r\n  [<arg2>]   arg2 help\r\n"));
        }

        [Test]
        public void WriteGlobalCommand()
        {
            // Arrange
            var application = new KingpinApplication();
            application.Commands.Add(new CommandItem("cmd", "command help"));
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("usage: <command> \r\n\r\n"));
            Assert.IsTrue(result.Contains("Commands:\r\n  cmd \r\n    command help\r\n"));
        }

        [Test]
        public void WriteGlobalCommands()
        {
            // Arrange
            var application = new KingpinApplication();
            application.Commands.Add(new CommandItem("cmd1", "command1 help"));
            application.Commands.Add(new CommandItem("cmd2", "command2 help"));
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("usage: <command> \r\n\r\n"));
            Assert.IsTrue(result.Contains("Commands:\r\n  cmd1 \r\n    command1 help\r\n"));
            Assert.IsTrue(result.Contains(             "  cmd2 \r\n    command2 help\r\n"));
        }
        [Test]
        public void WriteNestedCommands()
        {
            // Arrange
            var application = new KingpinApplication();
            var command = new CommandItem("cmd1", "command1 help");
            command.Command("cmd2", "command2 help");
            application.Commands.Add(command);
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("usage: <command> \r\n\r\n"));
            Assert.IsTrue(result.Contains("Commands:\r\n  cmd1 cmd2 \r\n    command2 help\r\n\r\n"));
        }

        [Test]
        public void WriteGlobalCommandWithFlag()
        {
            // Arrange
            var application = new KingpinApplication();
            var command = new CommandItem("cmd", "command help");
            command.Flag("flag", "flag help");
            application.Commands.Add(command);
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("usage: <command> \r\n\r\n"));
            Assert.IsTrue(result.Contains("Commands:\r\n  cmd --flag=<Flag> \r\n    command help\r\n\r\n"));
        }

        [Test]
        public void WriteGlobalCommandWithFlags()
        {
            // Arrange
            var application = new KingpinApplication();
            var command = new CommandItem("cmd", "command help");
            command.Flag("flag1", "flag1 help");
            command.Flag("flag2", "flag2 help");
            application.Commands.Add(command);
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("usage: <command> \r\n\r\n"));
            Assert.IsTrue(result.Contains("Commands:\r\n  cmd [<flags>] \r\n    command help\r\n\r\n"));
        }

        [Test]
        public void WriteGlobalCommandWithArguments()
        {
            // Arrange
            var application = new KingpinApplication();
            var command = new CommandItem("cmd", "command help");
            command.Argument("arg1", "arg1 help");
            command.Argument("arg2", "arg2 help");
            application.Commands.Add(command);
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("usage: <command> \r\n\r\n"));
            Assert.IsTrue(result.Contains("Commands:\r\n  cmd <arg1> <arg2> \r\n    command help\r\n\r\n"));
        }

        [Test]
        public void WriteNestedCommandWithGlobalCommandWithAFlag()
        {
            // Arrange
            var application = new KingpinApplication();
            var command = new CommandItem("cmd1", "command1 help");
            command.Flag("flag", "flag help");
            command.Command("cmd2", "command2 help");
            application.Commands.Add(command);
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("usage: <command> \r\n\r\n"));
            Assert.IsTrue(result.Contains("Commands:\r\n  cmd1 --flag=<Flag> \r\n    command1 help\r\n\r\n"));
            Assert.IsTrue(result.Contains("  cmd1 cmd2 \r\n    command2 help\r\n\r\n"));
        }
    }
}
