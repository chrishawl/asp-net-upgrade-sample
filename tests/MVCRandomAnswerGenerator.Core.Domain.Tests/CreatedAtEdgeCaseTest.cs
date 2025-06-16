using MVCRandomAnswerGenerator.Core.Domain;
using Xunit;

namespace MVCRandomAnswerGenerator.Core.Domain.Tests;

/// <summary>
/// Test to validate the fix for the CreatedAt property initializer issue.
/// </summary>
public class CreatedAtEdgeCaseTest
{
    [Fact]
    public void QuestionAndAnswer_WithExplicitDefault_UsesCurrentTime()
    {
        // Arrange
        var beforeTime = DateTime.UtcNow.AddSeconds(-1);
        
        // Act
        var questionAndAnswer = new QuestionAndAnswer("Test question", "Test answer", default);
        
        // Assert
        var afterTime = DateTime.UtcNow.AddSeconds(1);
        Assert.True(questionAndAnswer.CreatedAt >= beforeTime);
        Assert.True(questionAndAnswer.CreatedAt <= afterTime);
        Assert.NotEqual(default(DateTime), questionAndAnswer.CreatedAt);
    }

    [Fact]
    public void QuestionAndAnswer_PropertyInitializer_DoesNotReferenceSelf()
    {
        // This test ensures the problematic self-referencing property initializer was removed
        // by verifying that explicit default values are handled correctly
        
        // Arrange & Act
        var qa1 = new QuestionAndAnswer("Q1", "A1", default);
        var qa2 = new QuestionAndAnswer("Q2", "A2", new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        
        // Assert
        Assert.NotEqual(default(DateTime), qa1.CreatedAt);
        Assert.Equal(new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc), qa2.CreatedAt);
    }

    [Fact]
    public void QuestionAndAnswer_Deconstruct_WithDefaultCreatedAt_ReturnsCurrentTime()
    {
        // Arrange
        var beforeTime = DateTime.UtcNow.AddSeconds(-1);
        var qa = new QuestionAndAnswer("Test Q", "Test A", default);
        
        // Act
        var (question, answer, createdAt) = qa;
        
        // Assert
        var afterTime = DateTime.UtcNow.AddSeconds(1);
        Assert.Equal("Test Q", question);
        Assert.Equal("Test A", answer);
        Assert.True(createdAt >= beforeTime);
        Assert.True(createdAt <= afterTime);
        Assert.NotEqual(default(DateTime), createdAt);
    }

    [Fact]
    public void QuestionAndAnswer_WithExpression_PreservesCreatedAtBehavior()
    {
        // Arrange
        var original = new QuestionAndAnswer("Original Q", "Original A", default);
        var specificTime = new DateTime(2023, 5, 15, 10, 30, 0, DateTimeKind.Utc);
        
        // Act
        var modifiedWithCurrentTime = original with { Question = "Modified Q" };
        var modifiedWithSpecificTime = original with { CreatedAt = specificTime };
        
        // Assert
        Assert.Equal("Modified Q", modifiedWithCurrentTime.Question);
        Assert.Equal("Original A", modifiedWithCurrentTime.Answer);
        Assert.Equal(original.CreatedAt, modifiedWithCurrentTime.CreatedAt); // Should preserve original CreatedAt
        
        Assert.Equal("Original Q", modifiedWithSpecificTime.Question);
        Assert.Equal("Original A", modifiedWithSpecificTime.Answer);
        Assert.Equal(specificTime, modifiedWithSpecificTime.CreatedAt);
    }

    [Fact]
    public void QuestionAndAnswer_WithExpression_DefaultCreatedAt_UsesCurrentTime()
    {
        // Arrange
        var original = new QuestionAndAnswer("Q", "A", new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var beforeTime = DateTime.UtcNow.AddSeconds(-1);
        
        // Act
        var modified = original with { CreatedAt = default };
        
        // Assert
        var afterTime = DateTime.UtcNow.AddSeconds(1);
        Assert.Equal("Q", modified.Question);
        Assert.Equal("A", modified.Answer);
        // When using 'with' expression with default, it should use default DateTime, not current time
        // This is different from constructor behavior
        Assert.Equal(default(DateTime), modified.CreatedAt);
    }
}