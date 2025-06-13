using MVCRandomAnswerGenerator.Core.Domain;

namespace MVCRandomAnswerGenerator.Core.Domain.Tests;

public class AnswerGeneratorTests
{
    private readonly IAnswerGenerator _answerGenerator = new AnswerGenerator();

    [Fact]
    public void GenerateAnswer_WithSameQuestion_ReturnsSameAnswer()
    {
        // Arrange
        const string question = "Will this work?";

        // Act
        var answer1 = _answerGenerator.GenerateAnswer(question);
        var answer2 = _answerGenerator.GenerateAnswer(question);

        // Assert
        Assert.Equal(answer1, answer2);
    }

    [Fact]
    public void GenerateAnswer_WithNullQuestion_ThrowsArgumentNullException()
    {
        // Arrange
        string? nullQuestion = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _answerGenerator.GenerateAnswer(nullQuestion!));
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
        var answer = _answerGenerator.GenerateAnswer(question);

        // Assert
        Assert.Contains(answer, expectedAnswers);
    }
}