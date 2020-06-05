using KingpinNet;
using NUnit.Framework;

namespace Tests
{
    public class TokenizerTests
    {
        [Test]
        public void ParseTwoItems()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("word1 word2");

            // Assert
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[0] == "word1");
            Assert.IsTrue(result[1] == "word2");
        }

        [Test]
        public void ParseOneItem()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("word1");

            // Assert
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0] == "word1");
        }
        [Test]
        public void IgnoreSpaces1()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("   word1   ");

            // Assert
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0] == "word1");
        }
        [Test]
        public void IgnoreSpaces2()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("   word1   word2    ");

            // Assert
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[0] == "word1");
            Assert.IsTrue(result[1] == "word2");
        }
        [Test]
        public void RespectQuotes()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("   word1 \"  word2   \" ");

            // Assert
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[0] == "word1");
            Assert.IsTrue(result[1] == "  word2   ");
        }
        [Test]
        public void TreatTabsLikeSpaces()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("word1\tword2");

            // Assert
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[0] == "word1");
            Assert.IsTrue(result[1] == "word2");
        }
        [Test]
        public void TreatNewlineLikeSpaces()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("word1\nword2");

            // Assert
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[0] == "word1");
            Assert.IsTrue(result[1] == "word2");
        }
        [Test]
        public void TreatCarriageReturnNewLineLikeSpaces()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("word1\r\nword2");

            // Assert
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[0] == "word1");
            Assert.IsTrue(result[1] == "word2");
        }
        [Test]
        public void ParseZeroString()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("");

            // Assert
            Assert.IsTrue(result.Count == 0);
        }
        [Test]
        public void ParseEmptySpaces()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("         ");

            // Assert
            Assert.IsTrue(result.Count == 0);
        }

        [Test]
        public void TreatCarriageReturnNewlineAsSpace()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("   word1 \r\n  word2   ");

            // Assert
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[0] == "word1");
            Assert.IsTrue(result[1] == "word2");
        }

        [Test]
        public void TreatWeirdScenario1()
        {
            // Arrange
            var subject = new CommandLineTokenizer();

            // Act
            var result = subject.ToTokens("   word1\"\"word2");

            // Assert
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0] == "word1word2");
        }
    }
}
