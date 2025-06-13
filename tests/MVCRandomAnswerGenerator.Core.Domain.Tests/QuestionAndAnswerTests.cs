using MVCRandomAnswerGenerator.Core.Domain;

namespace MVCRandomAnswerGenerator.Core.Domain.Tests;

public class QuestionAndAnswerTests
{
    [Fact]
    public void QuestionAndAnswer_CanBeCreatedWithQuestionAndAnswer()
    {
        // Arrange
        const string question = "What is the meaning of life?";
        const string answer = "42";

        // Act
        var questionAndAnswer = new QuestionAndAnswer(question, answer);

        // Assert
        Assert.Equal(question, questionAndAnswer.Question);
        Assert.Equal(answer, questionAndAnswer.Answer);
        Assert.True(questionAndAnswer.CreatedAt <= DateTime.UtcNow);
        Assert.True(questionAndAnswer.CreatedAt > DateTime.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void QuestionAndAnswer_CanBeCreatedWithSpecificTimestamp()
    {
        // Arrange
        const string question = "Will this work?";
        const string answer = "Yes, definitely";
        var specificTime = new DateTime(2023, 10, 15, 14, 30, 0, DateTimeKind.Utc);

        // Act
        var questionAndAnswer = new QuestionAndAnswer(question, answer, specificTime);

        // Assert
        Assert.Equal(question, questionAndAnswer.Question);
        Assert.Equal(answer, questionAndAnswer.Answer);
        Assert.Equal(specificTime, questionAndAnswer.CreatedAt);
    }

    [Fact]
    public void QuestionAndAnswer_RecordEquality_WorksCorrectly()
    {
        // Arrange
        const string question = "Are records equal?";
        const string answer = "Yes";
        var timestamp = DateTime.UtcNow;

        var record1 = new QuestionAndAnswer(question, answer, timestamp);
        var record2 = new QuestionAndAnswer(question, answer, timestamp);
        var record3 = new QuestionAndAnswer("Different question", answer, timestamp);

        // Act & Assert
        Assert.Equal(record1, record2);
        Assert.NotEqual(record1, record3);
    }

    [Fact]
    public void QuestionAndAnswer_WithDeconstructionWorks()
    {
        // Arrange
        const string expectedQuestion = "Can I deconstruct this?";
        const string expectedAnswer = "Most likely";
        var expectedTime = DateTime.UtcNow;
        var record = new QuestionAndAnswer(expectedQuestion, expectedAnswer, expectedTime);

        // Act
        var (question, answer, createdAt) = record;

        // Assert
        Assert.Equal(expectedQuestion, question);
        Assert.Equal(expectedAnswer, answer);
        Assert.Equal(expectedTime, createdAt);
    }

    [Fact]
    public void QuestionAndAnswer_WithCloning_WorksCorrectly()
    {
        // Arrange
        var original = new QuestionAndAnswer("Original question", "Original answer");

        // Act
        var modified = original with { Answer = "Modified answer" };

        // Assert
        Assert.Equal("Original question", original.Question);
        Assert.Equal("Original answer", original.Answer);
        Assert.Equal("Original question", modified.Question);
        Assert.Equal("Modified answer", modified.Answer);
        Assert.Equal(original.CreatedAt, modified.CreatedAt);
    }
}