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
        private static readonly string Nl = Environment.NewLine;

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
            Assert.IsTrue(result.Contains($"Flags:{Nl}      --flag   flag help{Nl}"));
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
            Assert.IsTrue(result.Contains($"Flags:{Nl}  -f, --flag   flag help{Nl}"));
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
            Assert.IsTrue(result.Contains($"usage: [<flags>] {Nl}{Nl}"));
            Assert.IsTrue(result.Contains($"Flags:{Nl}  -f, --flag1   flag1 help{Nl}  -g, --flag2   flag2 help{Nl}"));
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
            Assert.IsTrue(result.Contains($"usage: [<arg>]{Nl}{Nl}"));
            Assert.IsTrue(result.Contains($"Args:{Nl}  [<arg>]   arg help{Nl}"));
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
            Assert.IsTrue(result.Contains($"usage: [<args> ...]{Nl}{Nl}"));
            Assert.IsTrue(result.Contains($"Args:{Nl}  [<arg1>]   arg1 help{Nl}  [<arg2>]   arg2 help{Nl}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Nl}{Nl}"));
            Assert.IsTrue(result.Contains($"Commands:{Nl}  cmd {Nl}    command help{Nl}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Nl}{Nl}"));
            Assert.IsTrue(result.Contains($"Commands:{Nl}  cmd1 {Nl}    command1 help{Nl}"));
            Assert.IsTrue(result.Contains($"  cmd2 {Nl}    command2 help{Nl}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Nl}{Nl}"));
            Assert.IsTrue(result.Contains($"Commands:{Nl}  cmd1 cmd2 {Nl}    command2 help{Nl}{Nl}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Nl}{Nl}"));
            Assert.IsTrue(result.Contains($"Commands:{Nl}  cmd --flag=<Flag> {Nl}    command help{Nl}{Nl}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Nl}{Nl}"));
            Assert.IsTrue(result.Contains($"Commands:{Nl}  cmd [<flags>] {Nl}    command help{Nl}{Nl}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Nl}{Nl}"));
            Assert.IsTrue(result.Contains($"Commands:{Nl}  cmd <arg1> <arg2> {Nl}    command help{Nl}{Nl}"));
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
            Assert.IsTrue(result.Contains($"usage: <command> {Nl}{Nl}"));
            Assert.IsTrue(result.Contains($"Commands:{Nl}  cmd1 --flag=<Flag> {Nl}    command1 help{Nl}{Nl}"));
            Assert.IsTrue(result.Contains($"  cmd1 cmd2 {Nl}    command2 help{Nl}{Nl}"));
        }
    }
}
