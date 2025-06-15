using FsCheck;
using FsCheck.Xunit;
using MVCRandomAnswerGenerator.Core.Domain;
using FluentAssertions;

namespace MVCRandomAnswerGenerator.Core.Domain.Tests;

/// <summary>
/// Property-based tests using FsCheck to validate AnswerGenerator behavior across a wide range of inputs.
/// These tests help discover edge cases that traditional unit tests might miss.
/// </summary>
public class PropertyBasedTests
{
    private readonly IAnswerGenerator _answerGenerator = new AnswerGenerator();

    /// <summary>
    /// Property: For any non-null string, GenerateAnswer should return a non-null, non-empty string
    /// that is one of the known Magic 8-Ball answers.
    /// </summary>
    [Property]
    public bool GenerateAnswer_WithAnyValidString_ReturnsValidAnswer(string question)
    {
        // Arrange: Filter out null values as they should throw exceptions
        if (question == null)
            return true; // Skip null test cases - handled by separate tests

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
        return answer != null &&
               answer.Length > 0 &&
               expectedAnswers.Contains(answer);
    }

    /// <summary>
    /// Property: For any non-null string, calling GenerateAnswer multiple times with the same input
    /// should always return the same result (deterministic behavior).
    /// </summary>
    [Property]
    public bool GenerateAnswer_WithSameInput_ProducesDeterministicResults(string question)
    {
        // Arrange: Filter out null values
        if (question == null)
            return true;

        // Act: Generate answers multiple times
        var answers = new List<string>();
        for (int i = 0; i < 5; i++)
        {
            answers.Add(_answerGenerator.GenerateAnswer(question));
        }

        // Assert: All answers should be identical
        return answers.All(a => a == answers[0]);
    }

    /// <summary>
    /// Property: Different inputs should generally produce different outputs
    /// (though collisions are possible due to hash code limitations).
    /// </summary>
    [Property]
    public void GenerateAnswer_WithDifferentInputs_MayProduceDifferentOutputs(string question1, string question2)
    {
        // Arrange: Filter out null values and identical strings
        if (question1 == null || question2 == null || question1 == question2)
            return;

        // Act
        var answer1 = _answerGenerator.GenerateAnswer(question1);
        var answer2 = _answerGenerator.GenerateAnswer(question2);

        // Assert: Both should be valid answers (they may or may not be different)
        var expectedAnswers = new[]
        {
            "It is certain", "It is decidedly so", "Without a doubt", "Yes, definitely",
            "You may rely on it", "As I see it, yes", "Most likely", "Outlook good",
            "Yes", "Signs point to yes", "Reply hazy try again", "Ask again later",
            "Better not tell you now", "Cannot predict now", "Concentrate and ask again",
            "Don't count on it", "My reply is no", "My sources say no",
            "Outlook not so good", "Very doubtful"
        };

        answer1.Should().BeOneOf(expectedAnswers);
        answer2.Should().BeOneOf(expectedAnswers);
    }

    /// <summary>
    /// Property: The length of the input string should not affect the validity of the output
    /// (performance may vary, but output should always be valid).
    /// </summary>
    [Property]
    public bool GenerateAnswer_WithVaryingInputLengths_ReturnsValidAnswer(int length)
    {
        // Arrange: Generate string of specified length (limit to reasonable size)
        if (length < 0 || length > 10000)
            return true; // Skip unreasonable lengths

        var question = new string('a', length);

        // Act
        var answer = _answerGenerator.GenerateAnswer(question);

        // Assert
        return !string.IsNullOrEmpty(answer);
    }

    /// <summary>
    /// Property: Questions with Unicode characters should be handled correctly.
    /// </summary>
    [Property]
    public bool GenerateAnswer_WithUnicodeCharacters_ReturnsValidAnswer()
    {
        // Arrange: Test with various Unicode strings
        var unicodeQuestions = new[]
        {
            "Will 🚀 work?",
            "Что будет дальше?", // Russian
            "¿Funcionará esto?", // Spanish with special characters
            "これは動きますか？", // Japanese
            "Will émojis and accénts work?", // Accented characters
            "Testing 中文 characters?", // Mixed scripts
            "Math symbols: ∑∆∇∂ work?", // Mathematical symbols
            "🎭🎨🎪🎯 emoji test?" // Multiple emojis
        };

        foreach (var question in unicodeQuestions)
        {
            var answer = _answerGenerator.GenerateAnswer(question);
            if (string.IsNullOrEmpty(answer))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Configure FsCheck to use reasonable test case counts and sizes.
    /// </summary>
    public PropertyBasedTests()
    {
        Arb.Register<Generators>();
    }
}

/// <summary>
/// Custom generators for property-based testing to create more realistic test data.
/// </summary>
public class Generators
{
    /// <summary>
    /// Generate reasonable-length strings for testing (avoiding extremely long strings).
    /// </summary>
    public static Arbitrary<string> ReasonableStrings() =>
        Arb.Default.String().Filter(s => s == null || s.Length <= 1000);
}