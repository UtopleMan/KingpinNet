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
            Assert.IsTrue(result.Contains($"Flags:{Environment.NewLine}      --flag   flag help{Environment.NewLine}"));
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
            Assert.IsTrue(result.Contains($"Flags:{Environment.NewLine}  -f, --flag   flag help{Environment.NewLine}"));
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
            Assert.IsTrue(result.Contains($"usage: [<flags>] {Environment.NewLine}{Environment.NewLine}"));
            Assert.IsTrue(result.Contains($"Flags:{Environment.NewLine}  -f, --flag1   flag1 help{Environment.NewLine}  -g, --flag2   flag2 help{Environment.NewLine}"));
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
            Assert.IsTrue(result.Contains($"usage: [<arg>]{Environment.NewLine}{Environment.NewLine}"));
            Assert.IsTrue(result.Contains($"Args:{Environment.NewLine}  [<arg>]   arg help{Environment.NewLine}"));
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
            Assert.IsTrue(result.Contains($"usage: [<args> ...]{Environment.NewLine}{Environment.NewLine}"));
            Assert.IsTrue(result.Contains($"Args:{Environment.NewLine}  [<arg1>]   arg1 help{Environment.NewLine}  [<arg2>]   arg2 help{Environment.NewLine}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Environment.NewLine}{Environment.NewLine}"));
            Assert.IsTrue(result.Contains($"Commands:{Environment.NewLine}  cmd {Environment.NewLine}    command help{Environment.NewLine}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Environment.NewLine}{Environment.NewLine}"));
            Assert.IsTrue(result.Contains($"Commands:{Environment.NewLine}  cmd1 {Environment.NewLine}    command1 help{Environment.NewLine}"));
            Assert.IsTrue(result.Contains($"  cmd2 {Environment.NewLine}    command2 help{Environment.NewLine}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Environment.NewLine}{Environment.NewLine}"));
            Assert.IsTrue(result.Contains($"Commands:{Environment.NewLine}  cmd1 cmd2 {Environment.NewLine}    command2 help{Environment.NewLine}{Environment.NewLine}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Environment.NewLine}{Environment.NewLine}"));
            Assert.IsTrue(result.Contains($"Commands:{Environment.NewLine}  cmd --flag=<Flag> {Environment.NewLine}    command help{Environment.NewLine}{Environment.NewLine}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Environment.NewLine}{Environment.NewLine}"));
            Assert.IsTrue(result.Contains($"Commands:{Environment.NewLine}  cmd [<flags>] {Environment.NewLine}    command help{Environment.NewLine}{Environment.NewLine}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Environment.NewLine}{Environment.NewLine}"));
            Assert.IsTrue(result.Contains($"Commands:{Environment.NewLine}  cmd <arg1> <arg2> {Environment.NewLine}    command help{Environment.NewLine}{Environment.NewLine}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Environment.NewLine}{Environment.NewLine}"));
            Assert.IsTrue(result.Contains($"Commands:{Environment.NewLine}  cmd1 --flag=<Flag> {Environment.NewLine}    command1 help{Environment.NewLine}{Environment.NewLine}"));
            Assert.IsTrue(result.Contains($"  cmd1 cmd2 {Environment.NewLine}    command2 help{Environment.NewLine}{Environment.NewLine}"));
        }
    }
}
