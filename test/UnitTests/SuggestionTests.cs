using KingpinNet;
using Moq;
using System.Linq;
using Xunit;

namespace Tests
{
    public class SuggestionTests
    {
        private Mock<IConsole> consoleMock;
        public SuggestionTests()
        {
            consoleMock = new Mock<IConsole>();
        }

        [Fact]
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
            Assert.True(result.IsSuggestion);
            Assert.Equal("cmd2", result.Suggestions.First());
        }
        [Fact]
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
            Assert.True(result.IsSuggestion);
            Assert.False(result.Suggestions.Any());
        }
        [Fact]
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
            Assert.True(result.IsSuggestion);
            Assert.Equal("cmd2", result.Suggestions.First());
        }
        [Fact]
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
            Assert.True(result.IsSuggestion);
            Assert.Equal("cmd1", result.Suggestions[0]);
            Assert.Equal("--myflag", result.Suggestions[1]);
        }
        [Fact]
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
            Assert.True(result.IsSuggestion);
            Assert.Equal("cmd1", result.Suggestions[0]);
            Assert.Equal("--myflag", result.Suggestions[1]);
        }
        [Fact]
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
            Assert.True(result.IsSuggestion);
            Assert.Equal("--myflag", result.Suggestions[0]);
        }
        [Fact]
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
            Assert.True(result.IsSuggestion);
            Assert.Equal("--myflag", result.Suggestions[0]);
        }
        [Fact]
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
            Assert.True(result.IsSuggestion);
            Assert.Equal("cmd1", result.Suggestions[0]);
            Assert.Equal("--myflag", result.Suggestions[1]);
            Assert.Equal("-m", result.Suggestions[2]);
        }
    }
}
