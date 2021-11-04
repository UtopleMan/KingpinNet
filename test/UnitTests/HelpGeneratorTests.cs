using KingpinNet;
using System.IO;
using System.Text;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class WriterStub : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
    public class HelpGeneratorTests
    {
        private readonly StringWriter writer = new StringWriter();
        private readonly IConsole console;
        private readonly ITestOutputHelper output;

        public HelpGeneratorTests(ITestOutputHelper output)
        {
            this.output = output;
            var consoleMock = new Mock<IConsole>();
            consoleMock.SetupGet(x => x.Out).Returns(writer);
            console = consoleMock.Object;
        }

        [Fact]
        public void WriteEmpty()
        {
            // Arrange
            var application = new KingpinApplication(console);

            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains("usage:", result);
        }
        [Fact]
        public void ParseGlobalHelpFlag()
        {
            // Arrange
            string[] args = new[] { "--help" };
            var application = new KingpinApplication(console);
            application.Initialize();
            application.Command("run", "This is a command");

            // Act
            var result = application.Parse(args);

            // Assert
            Assert.Equal("true", result.Result["help"]);
        }
        [Fact]
        public void ParseHelpFlagOnCommand()
        {
            // Arrange
            string[] args = new[] { "run", "--help" };
            var application = new KingpinApplication(console);
            application.Initialize();
            application.Command("run", "This is a command");

            // Act
            var result = application.Parse(args);

            // Assert
            Assert.Equal("true", result.Result["run:help"]);
        }
        [Fact]
        public void ShowDetailedError()
        {
            // Arrange
            string[] args = new[] { "--integer=x1x" };

            var application = new KingpinApplication(console);
            application.Initialize();
            application.ShowHelpOnParsingErrors();
            application.Command("integer", "This is an int").IsInt();

            // Act
            try
            {
                application.Parse(args);
            }
            catch { }

            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains("Illegal flag", result);
        }


        [Fact]
        public void WriteApplictaionName()
        {
            // Arrange
            var application = new KingpinApplication(console);
            application.Help("testapp.exe");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains("testapp.exe", result);
        }
        [Fact]
        public void WriteApplictaionHelp()
        {
            // Arrange
            var application = new KingpinApplication(console);
            application.Help("This is the glorious test app");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"This is the glorious test app", result);
        }
        [Fact]
        public void WriteGlobalFlag()
        {
            // Arrange
            var application = new KingpinApplication(console);
            application.Flag("flag", "flag help");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"Flags:", result);
            Assert.Contains($"  --flag=<value>                        flag help", result);
        }

        [Fact]
        public void WriteGlobalFlagWithShortName()
        {
            // Arrange
            var application = new KingpinApplication(console);
            application.Flag("flag", "flag help").Short('f');
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"Flags:", result);
            Assert.Contains($"  -f,  --flag=<value>                   flag help", result);
        }
        [Fact]
        public void WriteGlobalFlags()
        {
            // Arrange
            var application = new KingpinApplication(console);
            application.Flag("flag1", "flag1 help").Short('f');
            application.Flag("flag2", "flag2 help").Short('g');
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage:  [<flags>]", result);
            Assert.Contains($"Flags:", result);
            Assert.Contains($"  -f,  --flag1=<value>                  flag1 help", result);
            Assert.Contains($"  -g,  --flag2=<value>                  flag2 help", result);
        }

        [Fact]
        public void WriteGlobalArgument()
        {
            // Arrange
            var application = new KingpinApplication(console);
            application.Argument("arg", "arg help");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage:  [<arg>]", result);
            Assert.Contains($"Args:", result);
            Assert.Contains($"  [<arg>]                               arg help", result);
        }

        [Fact]
        public void WriteGlobalArguments()
        {
            // Arrange
            var application = new KingpinApplication(console);
            application.Argument("arg1", "arg1 help");
            application.Argument("arg2", "arg2 help");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage:  [<args> ...]", result);
            Assert.Contains($"Args:", result);
            Assert.Contains($"  [<arg1>]                              arg1 help", result);
            Assert.Contains($"  [<arg2>]                              arg2 help", result);
        }

        [Fact]
        public void WriteGlobalCommand()
        {
            // Arrange
            var application = new KingpinApplication(console);
            application.Command("cmd", "command help");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage:  <command>", result);
            Assert.Contains($"Commands:", result);
            Assert.Contains($"  cmd", result);
            Assert.Contains($"    command help", result);
        }

        [Fact]
        public void WriteGlobalCommands()
        {
            // Arrange
            var application = new KingpinApplication(console);
            application.Command("cmd1", "command1 help");
            application.Command("cmd2", "command2 help");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage:  <command>", result);
            Assert.Contains($"Commands:", result);
            Assert.Contains($"  cmd1", result);
            Assert.Contains($"    command1 help", result);
            Assert.Contains($"  cmd2", result);
            Assert.Contains($"    command2 help", result);
        }
        [Fact]
        public void WriteNestedCommands()
        {
            // Arrange
            var application = new KingpinApplication(console);
            var command = application.Command("cmd1", "command1 help");
            command.Command("cmd2", "command2 help");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage:  <command>", result);
            Assert.Contains($"Commands:", result);
            Assert.Contains($"  cmd1 cmd2", result);
            Assert.Contains($"    command2 help", result);
        }

        [Fact]
        public void WriteGlobalCommandWithFlag()
        {
            // Arrange
            var application = new KingpinApplication(console);
            var command = application.Command("cmd", "command help");
            command.Flag("flag", "flag help");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage:  <command>", result);
            Assert.Contains($"Commands:", result);
            Assert.Contains($"  cmd --flag=<value> flag help          command help", result);
        }

        [Fact]
        public void WriteGlobalCommandWithFlags()
        {
            // Arrange
            var application = new KingpinApplication(console);
            var command = application.Command("cmd", "command help");
            command.Flag("flag1", "flag1 help");
            command.Flag("flag2", "flag2 help");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage:  <command>", result);
            Assert.Contains($"Commands:", result);
            Assert.Contains($"  cmd [<flags>]", result);
            Assert.Contains($"    command help", result);
        }

        [Fact]
        public void WriteGlobalCommandWithArguments()
        {
            // Arrange
            var application = new KingpinApplication(console);
            var command = application.Command("cmd", "command help");
            command.Argument("arg1", "arg1 help");
            command.Argument("arg2", "arg2 help");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage:  <command>", result);
            Assert.Contains($"Commands:", result);
            Assert.Contains($"  cmd <arg1> <arg2>", result);
            Assert.Contains($"    command help", result);
        }

        [Fact]
        public void WriteNestedCommandWithGlobalCommandWithAFlag()
        {
            // Arrange
            var application = new KingpinApplication(console);
            var command = application.Command("cmd1", "command1 help");
            command.Flag("flag", "flag help");
            command.Command("cmd2", "command2 help");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage:  <command>", result);
            Assert.Contains($"Commands:", result);
            Assert.Contains($"  cmd1 --flag=<value> flag help         command1 help", result);
            Assert.Contains($"  cmd1 cmd2                             command2 help", result);
        }

        [Fact]
        public void WriteHelpForNestedCommand()
        {
            // Arrange
            var application = new KingpinApplication(console);
            var command = application.Command("cmd1", "command1 help");
            command.Command("cmd2", "command2 help");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.CommandHelp.liquid");
            subject.Generate(command, writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage:  cmd1 <command>", result);
            Assert.Contains($"command1 help", result);
            Assert.Contains($"Commands:", result);
            Assert.Contains($"  cmd2                                  command2 help", result);
        }

        [Fact]
        public void WriteGlobalCommandWithFlagAndExamples()
        {
            // Arrange
            var application = new KingpinApplication(console);
            var command = application.Command("cmd", "command help");
            command.Flag("flag", "flag help").SetExamples("1", "2");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.CommandHelp.liquid");
            subject.Generate(command, writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage:  cmd [<flags>]", result);
            Assert.Contains($"  command help", result);
            Assert.Contains($"Flags:", result);
            Assert.Contains($"  --flag=<value>                        flag help (e.g. 1, 2)", result);
        }

        [Fact]
        public void WriteGlobalCommandWithFlagAndDefaultValue()
        {
            // Arrange
            var application = new KingpinApplication(console);
            var command = application.Command("cmd", "command help");
            command.Flag("flag", "flag help").Default("1234.5678");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage:  <command>", result);
            Assert.Contains($"Commands:", result);
            Assert.Contains($"  cmd --flag=<1234.5678> flag help      command help", result);
        }

        [Fact]
        public void WriteFlagWithValueName()
        {
            // Arrange
            var application = new KingpinApplication(console);
            application.ApplicationName("x");
             var command = application.Command("cmd", "command help");
            command.Flag("flag", "flag help").ValueName("!name!");
            // Act
            var subject = new HelpGenerator(application, console);
            var appText = subject.ReadResourceInExecutingAssembly("KingpinNet.Help.ApplicationHelp.liquid");
            subject.Generate(writer, appText);
            // Assert
            var result = writer.ToString();
            output.WriteLine(result);
            Assert.Contains($"usage: x <command>", result);
            Assert.Contains($"Commands:", result);
            Assert.Contains($"  cmd --flag=!name!", result);
            Assert.Contains($"    command help", result);
        }
    }
}
