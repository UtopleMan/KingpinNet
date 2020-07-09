using KingpinNet;
using KingpinNet.UI;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace Tests
{
    public class AutoCompletionTests
    {
        private Mock<IConsole> consoleMock;

        [SetUp]
        public void Setup()
        {
            consoleMock = new Mock<IConsole>();
        }

        [Test]
        public void CompleteCommand()
        {
            // Arrange
            string[] args = new[] { "complete", "cmd1" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1", "command1 help");
            command.Command("cmd2", "command2 help");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsCompletion);
            Assert.AreEqual(result.Completions.First(), "cmd2");
        }
        [Test]
        public void CompleteNoCommand()
        {
            // Arrange
            string[] args = new[] { "complete", "cmd1", "k" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1", "command1 help");
            command.Command("cmd2", "command2 help");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsCompletion);
            Assert.IsFalse(result.Completions.Any());
        }
        [Test]
        public void CompleteCommandSubstring()
        {
            // Arrange
            string[] args = new[] { "complete", "cmd1", "cm" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1", "command1 help");
            command.Command("cmd2", "command2 help");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsCompletion);
            Assert.AreEqual(result.Completions.First(), "cmd2");
        }
        [Test]
        public void EmptyCompleteCommandShowAll()
        {
            // Arrange
            string[] args = new[] { "complete" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1");
            command.Command("cmd2");
            application.Flag("myflag");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsCompletion);
            Assert.AreEqual(result.Completions[0], "cmd1");
            Assert.AreEqual(result.Completions[1], "--myflag");
        }
        [Test]
        public void CompleteCommandShowAll()
        {
            // Arrange
            string[] args = new[] { "complete", "m" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1");
            command.Command("cmd2");
            application.Flag("myflag");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsCompletion);
            Assert.AreEqual(result.Completions[0], "cmd1");
            Assert.AreEqual(result.Completions[1], "--myflag");
        }
        [Test]
        public void CompleteMinusShowFlags()
        {
            // Arrange
            string[] args = new[] { "complete", "cmd1","-" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1");
            command.Command("cmd2");
            command.Flag("myflag");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsCompletion);
            Assert.AreEqual(result.Completions[0], "--myflag");
        }
        [Test]
        public void CompleteDoubleMinusShowFlags()
        {
            // Arrange
            string[] args = new[] { "complete", "cmd1", "--" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1");
            command.Command("cmd2");
            command.Flag("myflag");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsCompletion);
            Assert.AreEqual(result.Completions[0], "--myflag");
        }
        [Test]
        public void CompleteCommandShowShortNamedFlag()
        {
            // Arrange
            string[] args = new[] { "complete", "m" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1");
            application.Flag("myflag").Short('m');

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsCompletion);
            Assert.AreEqual(result.Completions[0], "cmd1");
            Assert.AreEqual(result.Completions[1], "--myflag");
            Assert.AreEqual(result.Completions[2], "-m");
        }
    }
}
