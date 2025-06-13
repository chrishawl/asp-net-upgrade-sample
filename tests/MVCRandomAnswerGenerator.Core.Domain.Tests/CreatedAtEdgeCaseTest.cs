using MVCRandomAnswerGenerator.Core.Domain;

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
}