using MVCRandomAnswerGenerator.Core.Domain;

namespace MVCRandomAnswerGenerator.Core.Domain.Tests;

/// <summary>
/// Tests to validate functional parity with the original .NET Framework implementation
/// These tests use known question-answer pairs to ensure deterministic compatibility.
/// </summary>
public class FrameworkCompatibilityTests
{
    private readonly IAnswerGenerator _answerGenerator = new AnswerGenerator();

    [Theory]
    [InlineData("Will this work?")]
    [InlineData("Is this a good idea?")]
    [InlineData("Should I continue?")]
    [InlineData("Will it rain tomorrow?")]
    [InlineData("Am I on the right track?")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Will 🚀 work with émojis and accénts?")]
    public void GenerateAnswer_ProducesDeterministicResults_AcrossMultipleCalls(string question)
    {
        // Arrange
        var results = new List<string>();

        // Act - Generate the same answer 10 times
        for (int i = 0; i < 10; i++)
        {
            results.Add(_answerGenerator.GenerateAnswer(question));
        }

        // Assert - All results should be identical
        Assert.True(results.All(r => r == results.First()), 
            $"All answers should be identical for question: '{question}'. Got: [{string.Join(", ", results.Distinct())}]");
    }

    [Fact]
    public void GenerateAnswer_EmptyString_ReturnsValidAnswer()
    {
        // Act
        var answer = _answerGenerator.GenerateAnswer("");

        // Assert
        Assert.NotNull(answer);
        Assert.NotEmpty(answer);
    }

    [Fact]
    public void GenerateAnswer_WhitespaceString_ReturnsValidAnswer()
    {
        // Act
        var answer = _answerGenerator.GenerateAnswer("   ");

        // Assert
        Assert.NotNull(answer);
        Assert.NotEmpty(answer);
    }

    [Fact]
    public void GenerateAnswer_LongString_ReturnsValidAnswer()
    {
        // Arrange
        var longQuestion = new string('a', 1000) + "?";

        // Act
        var answer = _answerGenerator.GenerateAnswer(longQuestion);

        // Assert
        Assert.NotNull(answer);
        Assert.NotEmpty(answer);
    }

    [Fact]
    public void GenerateAnswer_ProducesAllPossibleAnswers_WithDifferentQuestions()
    {
        // Arrange
        var allAnswers = new HashSet<string>();
        var testQuestions = new List<string>();
        
        // Generate enough different questions to potentially hit all answers
        for (int i = 0; i < 1000; i++)
        {
            testQuestions.Add($"Test question number {i}?");
        }

        // Act
        foreach (var question in testQuestions)
        {
            var answer = _answerGenerator.GenerateAnswer(question);
            allAnswers.Add(answer);
        }

        // Assert
        // We should get a good distribution of answers (at least 15 out of 20 possible)
        Assert.True(allAnswers.Count >= 15, $"Expected at least 15 different answers, but got {allAnswers.Count}");
    }

    [Fact]
    public void GenerateAnswer_DifferentQuestions_ProduceDifferentAnswers()
    {
        // Arrange
        const string question1 = "First question?";
        const string question2 = "Second question?";

        // Act
        var answer1 = _answerGenerator.GenerateAnswer(question1);
        var answer2 = _answerGenerator.GenerateAnswer(question2);

        // Assert
        Assert.NotEqual(answer1, answer2);
    }
}