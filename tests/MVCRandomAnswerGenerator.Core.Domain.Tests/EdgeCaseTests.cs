using FluentAssertions;
using MVCRandomAnswerGenerator.Core.Domain;

namespace MVCRandomAnswerGenerator.Core.Domain.Tests;

/// <summary>
/// Comprehensive edge case tests to ensure robust behavior of the AnswerGenerator
/// under various boundary conditions and unusual inputs.
/// </summary>
public class EdgeCaseTests
{
    private readonly IAnswerGenerator _answerGenerator = new AnswerGenerator();
    private readonly string[] _expectedAnswers = [
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
    ];

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("   \t  \n  ")]
    public void GenerateAnswer_WithWhitespaceStrings_ReturnsValidAnswer(string question)
    {
        // Act
        var answer = _answerGenerator.GenerateAnswer(question);

        // Assert
        answer.Should().NotBeNullOrEmpty();
        answer.Should().BeOneOf(_expectedAnswers);
    }

    [Theory]
    [InlineData("🚀")]
    [InlineData("🎭🎨🎪")]
    [InlineData("Will 🚀 work?")]
    [InlineData("Testing 🔥 performance!")]
    [InlineData("🎯🎲🎰🎱")]
    public void GenerateAnswer_WithEmojiCharacters_ReturnsValidAnswer(string question)
    {
        // Act
        var answer = _answerGenerator.GenerateAnswer(question);

        // Assert
        answer.Should().NotBeNullOrEmpty();
        answer.Should().BeOneOf(_expectedAnswers);
    }

    [Theory]
    [InlineData("café")]
    [InlineData("naïve")]
    [InlineData("résumé")]
    [InlineData("Will accénted characters work?")]
    [InlineData("Ñoño")]
    [InlineData("Düsseldorf")]
    public void GenerateAnswer_WithAccentedCharacters_ReturnsValidAnswer(string question)
    {
        // Act
        var answer = _answerGenerator.GenerateAnswer(question);

        // Assert
        answer.Should().NotBeNullOrEmpty();
        answer.Should().BeOneOf(_expectedAnswers);
    }

    [Theory]
    [InlineData("Что будет дальше?")] // Russian
    [InlineData("¿Funcionará esto?")] // Spanish
    [InlineData("これは動きますか？")] // Japanese
    [InlineData("這會起作用嗎？")] // Chinese Traditional
    [InlineData("هل سيعمل هذا؟")] // Arabic
    [InlineData("Будет ли это работать?")] // Russian
    public void GenerateAnswer_WithNonLatinScripts_ReturnsValidAnswer(string question)
    {
        // Act
        var answer = _answerGenerator.GenerateAnswer(question);

        // Assert
        answer.Should().NotBeNullOrEmpty();
        answer.Should().BeOneOf(_expectedAnswers);
    }

    [Theory]
    [InlineData("∑∆∇∂")] // Mathematical symbols
    [InlineData("α β γ δ")] // Greek letters
    [InlineData("℮ ℯ ℰ")] // Mathematical constants
    [InlineData("Will ∞ work?")] // Infinity symbol
    [InlineData("Testing √ symbols?")] // Square root
    public void GenerateAnswer_WithSpecialSymbols_ReturnsValidAnswer(string question)
    {
        // Act
        var answer = _answerGenerator.GenerateAnswer(question);

        // Assert
        answer.Should().NotBeNullOrEmpty();
        answer.Should().BeOneOf(_expectedAnswers);
    }

    [Fact]
    public void GenerateAnswer_WithVeryLongString_ReturnsValidAnswer()
    {
        // Arrange
        var longQuestion = new string('a', 100000) + "?";

        // Act
        var answer = _answerGenerator.GenerateAnswer(longQuestion);

        // Assert
        answer.Should().NotBeNullOrEmpty();
        answer.Should().BeOneOf(_expectedAnswers);
    }

    [Fact]
    public void GenerateAnswer_WithRepeatedCharacters_ReturnsValidAnswer()
    {
        // Arrange
        var questions = new[]
        {
            new string('?', 100),
            new string('a', 1000),
            new string(' ', 50),
            "aaaaaaaaaaaaaaaaaaaaaaaa?",
            "!!!!!!!!!!!!!!!!!!!!!!!!!",
            "........................."
        };

        foreach (var question in questions)
        {
            // Act
            var answer = _answerGenerator.GenerateAnswer(question);

            // Assert
            answer.Should().NotBeNullOrEmpty();
            answer.Should().BeOneOf(_expectedAnswers);
        }
    }

    [Theory]
    [InlineData("\0")] // Null character
    [InlineData("test\0question")] // Embedded null
    [InlineData("\u0001\u0002\u0003")] // Control characters
    [InlineData("test\u007Fquestion")] // DEL character
    public void GenerateAnswer_WithControlCharacters_ReturnsValidAnswer(string question)
    {
        // Act
        var answer = _answerGenerator.GenerateAnswer(question);

        // Assert
        answer.Should().NotBeNullOrEmpty();
        answer.Should().BeOneOf(_expectedAnswers);
    }

    [Fact]
    public void GenerateAnswer_WithHashCodeCollisionInputs_ProducesConsistentResults()
    {
        // Arrange: Find two different strings that might have hash code collisions
        // This is probabilistic, but testing the behavior is important
        var testQuestions = new List<string>();
        for (int i = 0; i < 1000; i++)
        {
            testQuestions.Add($"Question {i}");
        }

        // Act & Assert: Each question should consistently produce the same answer
        foreach (var question in testQuestions)
        {
            var answer1 = _answerGenerator.GenerateAnswer(question);
            var answer2 = _answerGenerator.GenerateAnswer(question);
            var answer3 = _answerGenerator.GenerateAnswer(question);

            answer1.Should().Be(answer2);
            answer2.Should().Be(answer3);
            answer1.Should().BeOneOf(_expectedAnswers);
        }
    }

    [Fact]
    public void GenerateAnswer_WithBoundaryLengthStrings_ReturnsValidAnswers()
    {
        // Arrange: Test strings of various boundary lengths
        var lengths = new[] { 0, 1, 2, 255, 256, 1023, 1024, 4095, 4096, 65535, 65536 };

        foreach (var length in lengths)
        {
            var question = length == 0 ? "" : new string('x', length);

            // Act
            var answer = _answerGenerator.GenerateAnswer(question);

            // Assert
            answer.Should().NotBeNullOrEmpty();
            answer.Should().BeOneOf(_expectedAnswers);
        }
    }

    [Fact]
    public void GenerateAnswer_WithMixedContentTypes_ReturnsValidAnswers()
    {
        // Arrange: Mixed content questions
        var mixedQuestions = new[]
        {
            "English 中文 🚀 café ∞?",
            "123 ABC abc !@# $%^ &*() test?",
            "Line1\nLine2\rLine3\r\nLine4",
            "Tab\tSeparated\tValues",
            "Quote\"Marks'AndApostrophes",
            "Backslash\\Forward/Slash",
            "HTML<tags>and&entities;",
            "JSON{\"key\":\"value\"}style",
            "XML<?xml version=\"1.0\"?><test/>",
            "URL https://example.com/path?param=value"
        };

        foreach (var question in mixedQuestions)
        {
            // Act
            var answer = _answerGenerator.GenerateAnswer(question);

            // Assert
            answer.Should().NotBeNullOrEmpty();
            answer.Should().BeOneOf(_expectedAnswers);
        }
    }

    [Fact]
    public void GenerateAnswer_StressTest_MaintainsPerformanceAndConsistency()
    {
        // Arrange
        const int iterations = 10000;
        var baseQuestion = "Stress test question";
        var results = new Dictionary<string, string>();

        // Act: Generate many answers quickly
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            var question = $"{baseQuestion} {i}";
            var answer = _answerGenerator.GenerateAnswer(question);
            
            // Store first result for consistency check
            if (!results.ContainsKey(question))
            {
                results[question] = answer;
            }
            
            // Verify answer is valid
            answer.Should().BeOneOf(_expectedAnswers);
            
            // Re-test a few random questions for consistency
            if (i % 100 == 0 && i > 0)
            {
                var randomIndex = Random.Shared.Next(i);
                var randomQuestion = $"{baseQuestion} {randomIndex}";
                var reAnswer = _answerGenerator.GenerateAnswer(randomQuestion);
                reAnswer.Should().Be(results[randomQuestion]);
            }
        }
        
        stopwatch.Stop();

        // Assert: Performance should be reasonable (less than 1ms per call on average)
        var averageTimePerCall = stopwatch.ElapsedMilliseconds / (double)iterations;
        averageTimePerCall.Should().BeLessThan(1.0, 
            $"Average time per call was {averageTimePerCall:F3}ms, which may indicate performance issues");
    }
}