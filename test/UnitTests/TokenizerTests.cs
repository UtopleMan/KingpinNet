using KingpinNet;
using Xunit;

namespace Tests
{
    public class TokenizerTests
    {
        [Fact]
        public void ParseTwoItems()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("word1 word2");

            // Assert
            Assert.True(result.Count == 2);
            Assert.True(result[0] == "word1");
            Assert.True(result[1] == "word2");
        }

        [Fact]
        public void ParseOneItem()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("word1");

            // Assert
            Assert.True(result.Count == 1);
            Assert.True(result[0] == "word1");
        }
        [Fact]
        public void IgnoreSpaces1()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("   word1   ");

            // Assert
            Assert.True(result.Count == 1);
            Assert.True(result[0] == "word1");
        }
        [Fact]
        public void IgnoreSpaces2()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("   word1   word2    ");

            // Assert
            Assert.True(result.Count == 2);
            Assert.True(result[0] == "word1");
            Assert.True(result[1] == "word2");
        }
        [Fact]
        public void RespectQuotes()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("   word1 \"  word2   \" ");

            // Assert
            Assert.True(result.Count == 2);
            Assert.True(result[0] == "word1");
            Assert.True(result[1] == "  word2   ");
        }
        [Fact]
        public void TreatTabsLikeSpaces()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("word1\tword2");

            // Assert
            Assert.True(result.Count == 2);
            Assert.True(result[0] == "word1");
            Assert.True(result[1] == "word2");
        }
        [Fact]
        public void TreatNewlineLikeSpaces()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("word1\nword2");

            // Assert
            Assert.True(result.Count == 2);
            Assert.True(result[0] == "word1");
            Assert.True(result[1] == "word2");
        }
        [Fact]
        public void TreatCarriageReturnNewLineLikeSpaces()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("word1\r\nword2");

            // Assert
            Assert.True(result.Count == 2);
            Assert.True(result[0] == "word1");
            Assert.True(result[1] == "word2");
        }
        [Fact]
        public void ParseZeroString()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("");

            // Assert
            Assert.True(result.Count == 0);
        }
        [Fact]
        public void ParseEmptySpaces()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("         ");

            // Assert
            Assert.True(result.Count == 0);
        }

        [Fact]
        public void TreatCarriageReturnNewlineAsSpace()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("   word1 \r\n  word2   ");

            // Assert
            Assert.True(result.Count == 2);
            Assert.True(result[0] == "word1");
            Assert.True(result[1] == "word2");
        }

        [Fact]
        public void TreatWeirdScenario1()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("   word1\"\"word2");

            // Assert
            Assert.True(result.Count == 1);
            Assert.True(result[0] == "word1word2");
        }
    }
}
