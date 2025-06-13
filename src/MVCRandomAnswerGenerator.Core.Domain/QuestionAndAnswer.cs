using System.ComponentModel.DataAnnotations;

namespace MVCRandomAnswerGenerator.Core.Domain;

/// <summary>
/// Represents a question and its corresponding answer with creation timestamp.
/// </summary>
/// <param name="Question">The question being asked. Cannot be null or empty.</param>
/// <param name="Answer">The generated answer. Cannot be null or empty.</param>
/// <param name="CreatedAt">The timestamp when this question-answer pair was created. Defaults to current UTC time.</param>
public record QuestionAndAnswer(
    [Required] string Question,
    [Required] string Answer,
    DateTime CreatedAt = default)
{
    /// <summary>
    /// Initializes a new instance of the QuestionAndAnswer record with the current UTC time.
    /// </summary>
    /// <param name="Question">The question being asked.</param>
    /// <param name="Answer">The generated answer.</param>
    public QuestionAndAnswer(string Question, string Answer)
        : this(Question, Answer, DateTime.UtcNow)
    {
    }

    /// <summary>
    /// Gets the creation timestamp, using current UTC time if not specified.
    /// </summary>
    public DateTime CreatedAt { get; init; } = CreatedAt == default ? DateTime.UtcNow : CreatedAt;
}