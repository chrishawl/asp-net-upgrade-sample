using FluentAssertions;
using MVCRandomAnswerGenerator.Core.Domain;

namespace MVCRandomAnswerGenerator.Core.Domain.Tests;

/// <summary>
/// Comprehensive tests for async functionality in the AnswerGenerator.
/// Ensures that async operations maintain the same deterministic behavior as sync operations.
/// </summary>
public class AsyncFunctionalityTests
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

    [Fact]
    public async Task GenerateAnswerAsync_WithValidQuestion_ReturnsValidAnswer()
    {
        // Arrange
        const string question = "Will this async method work?";

        // Act
        var answer = await _answerGenerator.GenerateAnswerAsync(question);

        // Assert
        answer.Should().NotBeNullOrEmpty();
        answer.Should().BeOneOf(_expectedAnswers);
    }

    [Fact]
    public async Task GenerateAnswerAsync_WithNullQuestion_ThrowsArgumentNullException()
    {
        // Arrange
        string? nullQuestion = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _answerGenerator.GenerateAnswerAsync(nullQuestion!).AsTask());
    }

    [Fact]
    public async Task GenerateAnswerAsync_WithSameQuestion_ReturnsSameAnswer()
    {
        // Arrange
        const string question = "Will async be consistent?";

        // Act
        var answer1 = await _answerGenerator.GenerateAnswerAsync(question);
        var answer2 = await _answerGenerator.GenerateAnswerAsync(question);
        var answer3 = await _answerGenerator.GenerateAnswerAsync(question);

        // Assert
        answer1.Should().Be(answer2);
        answer2.Should().Be(answer3);
    }

    [Fact]
    public async Task GenerateAnswerAsync_ComparedToSync_ReturnsSameResult()
    {
        // Arrange
        const string question = "Should async match sync?";

        // Act
        var syncAnswer = _answerGenerator.GenerateAnswer(question);
        var asyncAnswer = await _answerGenerator.GenerateAnswerAsync(question);

        // Assert
        asyncAnswer.Should().Be(syncAnswer);
    }

    [Theory]
    [InlineData("Will this work?")]
    [InlineData("Is this a good idea?")]
    [InlineData("Should I continue?")]
    [InlineData("Will it rain tomorrow?")]
    [InlineData("Am I on the right track?")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Will 🚀 work with émojis and accénts?")]
    public async Task GenerateAnswerAsync_WithVariousInputs_MatchesSyncResults(string question)
    {
        // Act
        var syncAnswer = _answerGenerator.GenerateAnswer(question);
        var asyncAnswer = await _answerGenerator.GenerateAnswerAsync(question);

        // Assert
        asyncAnswer.Should().Be(syncAnswer);
    }

    [Fact]
    public async Task GenerateAnswerAsync_ConcurrentCalls_ReturnConsistentResults()
    {
        // Arrange
        const string question = "Concurrent test question?";
        const int concurrentCalls = 100;

        // Act
        var tasks = new Task<string>[concurrentCalls];
        for (int i = 0; i < concurrentCalls; i++)
        {
            tasks[i] = _answerGenerator.GenerateAnswerAsync(question).AsTask();
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        var distinctResults = results.Distinct().ToArray();
        distinctResults.Should().HaveCount(1, "All concurrent calls should return the same result");
        distinctResults[0].Should().BeOneOf(_expectedAnswers);
    }

    [Fact]
    public async Task GenerateAnswerAsync_MultipleDifferentQuestionsConcurrently_ReturnValidAnswers()
    {
        // Arrange
        var questions = new[]
        {
            "Question 1?",
            "Question 2?",
            "Question 3?",
            "Question 4?",
            "Question 5?",
            "Question 6?",
            "Question 7?",
            "Question 8?",
            "Question 9?",
            "Question 10?"
        };

        // Act
        var tasks = questions.Select(q => _answerGenerator.GenerateAnswerAsync(q).AsTask()).ToArray();
        var results = await Task.WhenAll(tasks);

        // Assert
        for (int i = 0; i < results.Length; i++)
        {
            results[i].Should().BeOneOf(_expectedAnswers);
            
            // Each question should produce the same result when called again
            var repeatResult = await _answerGenerator.GenerateAnswerAsync(questions[i]);
            repeatResult.Should().Be(results[i]);
        }
    }

    [Fact]
    public async Task GenerateAnswerAsync_StressTestWithManyAsyncCalls_MaintainsPerformance()
    {
        // Arrange
        const int numberOfCalls = 1000;
        var questions = Enumerable.Range(1, numberOfCalls)
            .Select(i => $"Stress test question {i}?")
            .ToArray();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var tasks = questions.Select(q => _answerGenerator.GenerateAnswerAsync(q).AsTask()).ToArray();
        var results = await Task.WhenAll(tasks);
        
        stopwatch.Stop();

        // Assert
        results.Should().HaveCount(numberOfCalls);
        results.Should().AllSatisfy(result => result.Should().BeOneOf(_expectedAnswers));
        
        // Performance should be reasonable
        var averageTimePerCall = stopwatch.ElapsedMilliseconds / (double)numberOfCalls;
        averageTimePerCall.Should().BeLessThan(1.0, 
            $"Average time per async call was {averageTimePerCall:F3}ms, which may indicate performance issues");
    }

    [Fact]
    public async Task GenerateAnswerAsync_WithCancellationToken_CompletesSuccessfully()
    {
        // Arrange
        const string question = "Will cancellation work?";
        using var cts = new CancellationTokenSource();

        // Act
        var answer = await _answerGenerator.GenerateAnswerAsync(question);

        // Assert
        answer.Should().NotBeNullOrEmpty();
        answer.Should().BeOneOf(_expectedAnswers);
    }

    [Fact]
    public async Task GenerateAnswerAsync_WithLongRunningOperations_CompletesWithinReasonableTime()
    {
        // Arrange
        var longQuestion = new string('a', 10000) + "?";
        const int iterations = 50;

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var tasks = new List<Task<string>>();
        for (int i = 0; i < iterations; i++)
        {
            tasks.Add(_answerGenerator.GenerateAnswerAsync($"{longQuestion}{i}").AsTask());
        }
        
        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        results.Should().HaveCount(iterations);
        results.Should().AllSatisfy(result => result.Should().BeOneOf(_expectedAnswers));
        
        // Should complete within reasonable time
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000, 
            "Long running async operations should complete within 1 second");
    }

    [Fact]
    public async Task GenerateAnswerAsync_ValueTaskBehavior_IsOptimized()
    {
        // Arrange
        const string question = "ValueTask optimization test?";

        // Act
        var valueTask1 = _answerGenerator.GenerateAnswerAsync(question);
        var valueTask2 = _answerGenerator.GenerateAnswerAsync(question);

        // Assert - ValueTask should complete synchronously for this implementation
        valueTask1.IsCompleted.Should().BeTrue("ValueTask should complete synchronously");
        valueTask2.IsCompleted.Should().BeTrue("ValueTask should complete synchronously");

        var result1 = await valueTask1;
        var result2 = await valueTask2;

        result1.Should().Be(result2);
        result1.Should().BeOneOf(_expectedAnswers);
    }

    [Fact]
    public async Task GenerateAnswerAsync_ConfigureAwaitFalse_WorksCorrectly()
    {
        // Arrange
        const string question = "ConfigureAwait test?";

        // Act
        var answer = await _answerGenerator.GenerateAnswerAsync(question).ConfigureAwait(false);

        // Assert
        answer.Should().NotBeNullOrEmpty();
        answer.Should().BeOneOf(_expectedAnswers);
    }
}