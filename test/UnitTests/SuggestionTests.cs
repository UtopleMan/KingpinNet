using KingpinNet;
using KingpinNet.UI;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace Tests
{
    public class SuggestionTests
    {
        private Mock<IConsole> consoleMock;

        [SetUp]
        public void Setup()
        {
            consoleMock = new Mock<IConsole>();
        }

        [Test]
        public void SuggestCommand()
        {
            // Arrange
            string[] args = new[] { "suggest", "cmd1" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1", "command1 help");
            command.Command("cmd2", "command2 help");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsSuggestion);
            Assert.AreEqual(result.Suggestions.First(), "cmd2");
        }
        [Test]
        public void SuggestNoCommand()
        {
            // Arrange
            string[] args = new[] { "suggest", "cmd1", "k" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1", "command1 help");
            command.Command("cmd2", "command2 help");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsSuggestion);
            Assert.IsFalse(result.Suggestions.Any());
        }
        [Test]
        public void SuggestCommandSubstring()
        {
            // Arrange
            string[] args = new[] { "suggest", "cmd1", "cm" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1", "command1 help");
            command.Command("cmd2", "command2 help");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsSuggestion);
            Assert.AreEqual(result.Suggestions.First(), "cmd2");
        }
        [Test]
        public void EmptySuggestCommandShowAll()
        {
            // Arrange
            string[] args = new[] { "suggest" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1");
            command.Command("cmd2");
            application.Flag("myflag");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsSuggestion);
            Assert.AreEqual(result.Suggestions[0], "cmd1");
            Assert.AreEqual(result.Suggestions[1], "--myflag");
        }
        [Test]
        public void SuggestCommandShowAll()
        {
            // Arrange
            string[] args = new[] { "suggest", "m" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1");
            command.Command("cmd2");
            application.Flag("myflag");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsSuggestion);
            Assert.AreEqual(result.Suggestions[0], "cmd1");
            Assert.AreEqual(result.Suggestions[1], "--myflag");
        }
        [Test]
        public void SuggestMinusShowFlags()
        {
            // Arrange
            string[] args = new[] { "suggest","cmd1","-" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1");
            command.Command("cmd2");
            command.Flag("myflag");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsSuggestion);
            Assert.AreEqual(result.Suggestions[0], "--myflag");
        }
        [Test]
        public void SuggestDoubleMinusShowFlags()
        {
            // Arrange
            string[] args = new[] { "suggest", "cmd1", "--" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1");
            command.Command("cmd2");
            command.Flag("myflag");

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsSuggestion);
            Assert.AreEqual(result.Suggestions[0], "--myflag");
        }
        [Test]
        public void SuggestCommandShowShortNamedFlag()
        {
            // Arrange
            string[] args = new[] { "suggest", "m" };
            var application = new KingpinApplication(consoleMock.Object);
            var command = application.Command("cmd1");
            application.Flag("myflag").Short('m');

            // Act
            var subject = new Parser(application);
            var result = subject.Parse(args);

            // Assert
            Assert.IsTrue(result.IsSuggestion);
            Assert.AreEqual(result.Suggestions[0], "cmd1");
            Assert.AreEqual(result.Suggestions[1], "--myflag");
            Assert.AreEqual(result.Suggestions[2], "-m");
        }
    }
}
