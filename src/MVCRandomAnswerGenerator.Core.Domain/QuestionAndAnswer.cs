using System.ComponentModel.DataAnnotations;

namespace MVCRandomAnswerGenerator.Core.Domain;

/// <summary>
/// Represents a question and its corresponding answer with creation timestamp.
/// </summary>
public record QuestionAndAnswer
{
    /// <summary>
    /// Initializes a new instance of the QuestionAndAnswer record.
    /// </summary>
    /// <param name="Question">The question being asked. Cannot be null or empty.</param>
    /// <param name="Answer">The generated answer. Cannot be null or empty.</param>
    /// <param name="CreatedAt">The timestamp when this question-answer pair was created. Defaults to current UTC time.</param>
    public QuestionAndAnswer([Required] string Question, [Required] string Answer, DateTime CreatedAt = default)
    {
        this.Question = Question;
        this.Answer = Answer;
        this.CreatedAt = CreatedAt == default ? DateTime.UtcNow : CreatedAt;
    }

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
    /// The question being asked. Cannot be null or empty.
    /// </summary>
    [Required]
    public string Question { get; init; }

    /// <summary>
    /// The generated answer. Cannot be null or empty.
    /// </summary>
    [Required]
    public string Answer { get; init; }

    /// <summary>
    /// The timestamp when this question-answer pair was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Deconstructs the QuestionAndAnswer record into its component parts.
    /// </summary>
    /// <param name="Question">The question being asked.</param>
    /// <param name="Answer">The generated answer.</param>
    /// <param name="CreatedAt">The timestamp when this question-answer pair was created.</param>
    public void Deconstruct(out string Question, out string Answer, out DateTime CreatedAt)
    {
        Question = this.Question;
        Answer = this.Answer;
        CreatedAt = this.CreatedAt;
    }
}