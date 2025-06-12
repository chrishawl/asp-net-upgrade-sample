using System;
using MVCRandomAnswerGenerator.Models;
using Xunit;

namespace MVCRandomAnswerGenerator.Framework.Tests
{
    public class QuestionAndAnswerTests
    {
        [Fact]
        public void QuestionAndAnswer_CanBeInstantiated()
        {
            // Act
            var questionAndAnswer = new QuestionAndAnswer();

            // Assert
            Assert.NotNull(questionAndAnswer);
        }

        [Fact]
        public void Question_CanBeSetAndRetrieved()
        {
            // Arrange
            var questionAndAnswer = new QuestionAndAnswer();
            const string expectedQuestion = "What is the meaning of life?";

            // Act
            questionAndAnswer.Question = expectedQuestion;

            // Assert
            Assert.Equal(expectedQuestion, questionAndAnswer.Question);
        }

        [Fact]
        public void Answer_CanBeSetAndRetrieved()
        {
            // Arrange
            var questionAndAnswer = new QuestionAndAnswer();
            const string expectedAnswer = "42";

            // Act
            questionAndAnswer.Answer = expectedAnswer;

            // Assert
            Assert.Equal(expectedAnswer, questionAndAnswer.Answer);
        }

        [Fact]
        public void QuestionAndAnswer_PropertiesInitializeToNull()
        {
            // Act
            var questionAndAnswer = new QuestionAndAnswer();

            // Assert
            Assert.Null(questionAndAnswer.Question);
            Assert.Null(questionAndAnswer.Answer);
        }

        [Fact]
        public void QuestionAndAnswer_CanBeInitializedWithObjectInitializer()
        {
            // Arrange
            const string expectedQuestion = "Will this work?";
            const string expectedAnswer = "Yes, definitely";

            // Act
            var questionAndAnswer = new QuestionAndAnswer
            {
                Question = expectedQuestion,
                Answer = expectedAnswer
            };

            // Assert
            Assert.Equal(expectedQuestion, questionAndAnswer.Question);
            Assert.Equal(expectedAnswer, questionAndAnswer.Answer);
        }

        [Fact]
        public void Question_CanBeSetToNull()
        {
            // Arrange
            var questionAndAnswer = new QuestionAndAnswer
            {
                Question = "Initial question"
            };

            // Act
            questionAndAnswer.Question = null;

            // Assert
            Assert.Null(questionAndAnswer.Question);
        }

        [Fact]
        public void Answer_CanBeSetToNull()
        {
            // Arrange
            var questionAndAnswer = new QuestionAndAnswer
            {
                Answer = "Initial answer"
            };

            // Act
            questionAndAnswer.Answer = null;

            // Assert
            Assert.Null(questionAndAnswer.Answer);
        }

        [Fact]
        public void Question_CanBeSetToEmptyString()
        {
            // Arrange
            var questionAndAnswer = new QuestionAndAnswer();

            // Act
            questionAndAnswer.Question = "";

            // Assert
            Assert.Equal("", questionAndAnswer.Question);
        }

        [Fact]
        public void Answer_CanBeSetToEmptyString()
        {
            // Arrange
            var questionAndAnswer = new QuestionAndAnswer();

            // Act
            questionAndAnswer.Answer = "";

            // Assert
            Assert.Equal("", questionAndAnswer.Answer);
        }

        [Fact]
        public void Question_SupportsUnicodeCharacters()
        {
            // Arrange
            var questionAndAnswer = new QuestionAndAnswer();
            const string unicodeQuestion = "Will 🚀 émojis and accénts work?";

            // Act
            questionAndAnswer.Question = unicodeQuestion;

            // Assert
            Assert.Equal(unicodeQuestion, questionAndAnswer.Question);
        }

        [Fact]
        public void Answer_SupportsUnicodeCharacters()
        {
            // Arrange
            var questionAndAnswer = new QuestionAndAnswer();
            const string unicodeAnswer = "Sí, definitely! 🎉 It's très bien!";

            // Act
            questionAndAnswer.Answer = unicodeAnswer;

            // Assert
            Assert.Equal(unicodeAnswer, questionAndAnswer.Answer);
        }

        [Fact]
        public void Question_SupportsLongStrings()
        {
            // Arrange
            var questionAndAnswer = new QuestionAndAnswer();
            var longQuestion = new string('Q', 10000) + "?";

            // Act
            questionAndAnswer.Question = longQuestion;

            // Assert
            Assert.Equal(longQuestion, questionAndAnswer.Question);
        }

        [Fact]
        public void Answer_SupportsLongStrings()
        {
            // Arrange
            var questionAndAnswer = new QuestionAndAnswer();
            var longAnswer = new string('A', 10000);

            // Act
            questionAndAnswer.Answer = longAnswer;

            // Assert
            Assert.Equal(longAnswer, questionAndAnswer.Answer);
        }

        [Fact]
        public void QuestionAndAnswer_PropertiesAreIndependent()
        {
            // Arrange
            var questionAndAnswer = new QuestionAndAnswer();
            const string question = "Test question?";
            const string answer = "Test answer";

            // Act
            questionAndAnswer.Question = question;
            questionAndAnswer.Answer = answer;

            // Modify one property
            questionAndAnswer.Question = "Modified question?";

            // Assert
            Assert.Equal("Modified question?", questionAndAnswer.Question);
            Assert.Equal(answer, questionAndAnswer.Answer); // Answer should remain unchanged
        }

        [Fact]
        public void QuestionAndAnswer_SupportsWhitespaceValues()
        {
            // Arrange
            var questionAndAnswer = new QuestionAndAnswer();
            const string whitespaceQuestion = "   ";
            const string whitespaceAnswer = "\t\n\r ";

            // Act
            questionAndAnswer.Question = whitespaceQuestion;
            questionAndAnswer.Answer = whitespaceAnswer;

            // Assert
            Assert.Equal(whitespaceQuestion, questionAndAnswer.Question);
            Assert.Equal(whitespaceAnswer, questionAndAnswer.Answer);
        }
    }
}