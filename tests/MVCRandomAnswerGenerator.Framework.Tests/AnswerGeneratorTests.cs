using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MVCRandomAnswerGenerator.Framework.Tests
{
    public class AnswerGeneratorTests
    {
        [Fact]
        public void GenerateAnswer_WithSameQuestion_ReturnsSameAnswer()
        {
            // Arrange
            const string question = "Will this work?";

            // Act
            var answer1 = AnswerGenerator.GenerateAnswer(question);
            var answer2 = AnswerGenerator.GenerateAnswer(question);

            // Assert
            Assert.Equal(answer1, answer2);
        }

        [Fact]
        public void GenerateAnswer_WithDifferentQuestions_ReturnsDifferentAnswers()
        {
            // Arrange
            const string question1 = "First question?";
            const string question2 = "Second question?";

            // Act
            var answer1 = AnswerGenerator.GenerateAnswer(question1);
            var answer2 = AnswerGenerator.GenerateAnswer(question2);

            // Assert
            Assert.NotEqual(answer1, answer2);
        }

        [Theory]
        [InlineData("Will this work?")]
        [InlineData("Is this a good idea?")]
        [InlineData("Should I continue?")]
        [InlineData("Will it rain tomorrow?")]
        [InlineData("Am I on the right track?")]
        public void GenerateAnswer_WithValidQuestion_ReturnsKnownAnswer(string question)
        {
            // Arrange
            var expectedAnswers = new[]
            {
                "It is certain",
                "It is decidedly so",
                "Without a doubt",
                "Yes, definitely",
                "You may rely on it",
                "As I see it, yes",
                "Most likely",
                "Outlook good",
                "Yes",
                "Signs point to yes",
                "Reply hazy try again",
                "Ask again later",
                "Better not tell you now",
                "Cannot predict now",
                "Concentrate and ask again",
                "Don't count on it",
                "My reply is no",
                "My sources say no",
                "Outlook not so good",
                "Very doubtful"
            };

            // Act
            var answer = AnswerGenerator.GenerateAnswer(question);

            // Assert
            Assert.Contains(answer, expectedAnswers);
        }

        [Fact]
        public void GenerateAnswer_WithNullQuestion_ThrowsArgumentNullException()
        {
            // Arrange
            string nullQuestion = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => AnswerGenerator.GenerateAnswer(nullQuestion));
        }

        [Fact]
        public void GenerateAnswer_WithEmptyQuestion_ReturnsValidAnswer()
        {
            // Arrange
            const string emptyQuestion = "";

            // Act
            var answer = AnswerGenerator.GenerateAnswer(emptyQuestion);

            // Assert
            Assert.NotNull(answer);
            Assert.NotEmpty(answer);
        }

        [Fact]
        public void GenerateAnswer_WithWhitespaceQuestion_ReturnsValidAnswer()
        {
            // Arrange
            const string whitespaceQuestion = "   ";

            // Act
            var answer = AnswerGenerator.GenerateAnswer(whitespaceQuestion);

            // Assert
            Assert.NotNull(answer);
            Assert.NotEmpty(answer);
        }

        [Fact]
        public void GenerateAnswer_IsDeterministic_SameInputAlwaysGivesSameOutput()
        {
            // Arrange
            const string question = "Deterministic test question";
            var answers = new List<string>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                answers.Add(AnswerGenerator.GenerateAnswer(question));
            }

            // Assert
            Assert.True(answers.All(a => a == answers.First()), "All answers should be identical for the same question");
        }

        [Fact]
        public void GenerateAnswer_WithUnicodeCharacters_ReturnsValidAnswer()
        {
            // Arrange
            const string unicodeQuestion = "Will 🚀 work with émojis and accénts?";

            // Act
            var answer = AnswerGenerator.GenerateAnswer(unicodeQuestion);

            // Assert
            Assert.NotNull(answer);
            Assert.NotEmpty(answer);
        }

        [Fact]
        public void GenerateAnswer_WithLongQuestion_ReturnsValidAnswer()
        {
            // Arrange
            var longQuestion = new string('a', 1000) + "?";

            // Act
            var answer = AnswerGenerator.GenerateAnswer(longQuestion);

            // Assert
            Assert.NotNull(answer);
            Assert.NotEmpty(answer);
        }

        [Fact]
        public void GenerateAnswer_ProducesAllPossibleAnswers()
        {
            // Arrange
            var allAnswers = new HashSet<string>();
            var testQuestions = new List<string>();
            
            // Generate enough different questions to potentially hit all answers
            for (int i = 0; i < 10000; i++)
            {
                testQuestions.Add($"Test question number {i}?");
            }

            // Act
            foreach (var question in testQuestions)
            {
                var answer = AnswerGenerator.GenerateAnswer(question);
                allAnswers.Add(answer);
            }

            // Assert
            // We should get a good distribution of answers (at least 15 out of 20 possible)
            Assert.True(allAnswers.Count >= 15, $"Expected at least 15 different answers, but got {allAnswers.Count}");
        }
    }
}