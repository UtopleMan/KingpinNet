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
        public void ParseHelpFlag()
        {
            // Arrange
            string[] args = new[] { "--help" };
            var application = new KingpinApplication();
            application.Initialize();
            application.Commands.Add(new CommandItem("run", "This is a command"));

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.AreEqual(result["help"], "true");
        }

        [Test]
        public void ShowDetailedError()
        {
            // Arrange
            string[] args = new[] { "--integer=x1x" };
            Kingpin.ShowHelpOnParsingErrors();

            Kingpin.Command("integer", "This is an int").IsInt();

            StringWriter sw = new StringWriter();
            Console.SetOut(sw);
            // Act
            try
            {
                Kingpin.Parse(args);
            }
            catch { }
            finally
            {
                Console.Out.Close();
            }

            // Assert
            Assert.IsTrue(sw.ToString().Contains("Illegal flag"));
        }


        [Test]
        public void WriteApplictaionName()
        {
            // Arrange
            var application = new KingpinApplication();
            application.ApplicationHelp("testapp.exe");
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains("testapp.exe"));
        }
        [Test]
        public void WriteApplictaionHelp()
        {
            // Arrange
            var application = new KingpinApplication();
            application.ApplicationHelp("This is the glorious test app");
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            TestContext.Out.WriteLine(result);
            Assert.IsTrue(result.Contains($"This is the glorious test app"));
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
            Assert.IsTrue(result.Contains($"Flags:"));
            Assert.IsTrue(result.Contains($"      --flag   flag help"));
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
            Assert.IsTrue(result.Contains($"Flags:"));
            Assert.IsTrue(result.Contains($"  -f, --flag   flag help"));
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
            Assert.IsTrue(result.Contains($"usage: [<flags>]"));
            Assert.IsTrue(result.Contains($"Flags:"));
            Assert.IsTrue(result.Contains($"  -f, --flag1   flag1 help"));
            Assert.IsTrue(result.Contains($"  -g, --flag2   flag2 help"));
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
            Assert.IsTrue(result.Contains($"usage: [<arg>]"));
            Assert.IsTrue(result.Contains($"Args:"));
            Assert.IsTrue(result.Contains($"  [<arg>]   arg help"));
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
            Assert.IsTrue(result.Contains($"usage: [<args> ...]"));
            Assert.IsTrue(result.Contains($"Args:"));
            Assert.IsTrue(result.Contains($"  [<arg1>]   arg1 help"));
            Assert.IsTrue(result.Contains($"  [<arg2>]   arg2 help"));
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
            Assert.IsTrue(result.Contains($"usage: <command>"));
            Assert.IsTrue(result.Contains($"Commands:"));
            Assert.IsTrue(result.Contains($"  cmd"));
            Assert.IsTrue(result.Contains($"    command help"));
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
            Assert.IsTrue(result.Contains($"usage: <command>"));
            Assert.IsTrue(result.Contains($"Commands:"));
            Assert.IsTrue(result.Contains($"  cmd1"));
            Assert.IsTrue(result.Contains($"    command1 help"));
            Assert.IsTrue(result.Contains($"  cmd2"));
            Assert.IsTrue(result.Contains($"    command2 help"));
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
            Assert.IsTrue(result.Contains($"usage: <command>"));
            Assert.IsTrue(result.Contains($"Commands:"));
            Assert.IsTrue(result.Contains($"  cmd1 cmd2"));
            Assert.IsTrue(result.Contains($"    command2 help"));
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
            Assert.IsTrue(result.Contains($"usage: <command>"));
            Assert.IsTrue(result.Contains($"Commands:"));
            Assert.IsTrue(result.Contains($"  cmd --flag=<Flag>"));
            Assert.IsTrue(result.Contains($"    command help"));
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
            Assert.IsTrue(result.Contains($"usage: <command>"));
            Assert.IsTrue(result.Contains($"Commands:"));
            Assert.IsTrue(result.Contains($"  cmd [<flags>]"));
            Assert.IsTrue(result.Contains($"    command help"));
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
            Assert.IsTrue(result.Contains($"usage: <command>"));
            Assert.IsTrue(result.Contains($"Commands:"));
            Assert.IsTrue(result.Contains($"  cmd <arg1> <arg2>"));
            Assert.IsTrue(result.Contains($"    command help"));
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
            Assert.IsTrue(result.Contains($"usage: <command>"));
            Assert.IsTrue(result.Contains($"Commands:"));
            Assert.IsTrue(result.Contains($"  cmd1 --flag=<Flag>"));
            Assert.IsTrue(result.Contains($"    command1 help"));
            Assert.IsTrue(result.Contains($"  cmd1 cmd2"));
            Assert.IsTrue(result.Contains($"    command2 help"));
        }

        [Test]
        public void WriteHelpForNestedCommand()
        {
            // Arrange
            var application = new KingpinApplication();
            var command = new CommandItem("cmd1", "command1 help");
            command.Command("cmd2", "command2 help");
            application.Commands.Add(command);
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(command, writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains($"usage: cmd1 <command>"));
            Assert.IsTrue(result.Contains($"command1 help"));
            Assert.IsTrue(result.Contains($"Commands:"));
            Assert.IsTrue(result.Contains($"  cmd2"));
            Assert.IsTrue(result.Contains($"    command2 help"));
        }

        [Test]
        public void WriteGlobalCommandWithFlagAndExamples()
        {
            // Arrange
            var application = new KingpinApplication();
            var command = new CommandItem("cmd", "command help");
            command.Flag("flag", "flag help").Examples("1", "2");
            application.Commands.Add(command);
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(command, writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains($"usage: cmd [<flags>]"));
            Assert.IsTrue(result.Contains($"command help"));
            Assert.IsTrue(result.Contains($"Flags:"));
            Assert.IsTrue(result.Contains($"      --flag   flag help (e.g. 1, 2)"));
        }

        [Test]
        public void WriteGlobalCommandWithFlagAndDefaultValue()
        {
            // Arrange
            var application = new KingpinApplication();
            var command = new CommandItem("cmd", "command help");
            command.Flag("flag", "flag help").Default("1234.5678");
            application.Commands.Add(command);
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains($"usage: <command>"));
            Assert.IsTrue(result.Contains($"Commands:"));
            Assert.IsTrue(result.Contains($"  cmd --flag=<1234.5678>"));
            Assert.IsTrue(result.Contains($"    command help"));
        }

        [Test]
        public void WriteFlagWithValueName()
        {
            // Arrange
            var application = new KingpinApplication();
            var command = new CommandItem("cmd", "command help");
            command.Flag("flag", "flag help").ValueName("!name!");
            application.Commands.Add(command);
            // Act
            var subject = new HelpGenerator(application);
            var writer = new StringWriter();
            subject.Generate(writer);
            // Assert
            var result = writer.ToString();
            Assert.IsTrue(result.Contains($"usage: <command>"));
            Assert.IsTrue(result.Contains($"Commands:"));
            Assert.IsTrue(result.Contains($"  cmd --flag=!name!"));
            Assert.IsTrue(result.Contains($"    command help"));
        }
    }
}
