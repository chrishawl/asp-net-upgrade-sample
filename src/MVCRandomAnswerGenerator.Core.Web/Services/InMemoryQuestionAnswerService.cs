using MVCRandomAnswerGenerator.Core.Domain;

namespace MVCRandomAnswerGenerator.Core.Web.Services;

/// <summary>
/// In-memory implementation of the question and answer service.
/// This is a singleton service that maintains state across requests.
/// </summary>  
public sealed class InMemoryQuestionAnswerService : IQuestionAnswerService
{
    private readonly List<QuestionAndAnswer> _allAnswers = [];
    private readonly object _lock = new();

    /// <summary>
    /// Gets all stored questions and answers.
    /// </summary>
    /// <returns>A list of questions and answers ordered by most recent first.</returns>
    public IReadOnlyList<QuestionAndAnswer> GetAll()
    {
        lock (_lock)
        {
            return _allAnswers.ToList().AsReadOnly();
        }
    }

    /// <summary>
    /// Adds a new question and answer to the collection.
    /// </summary>
    /// <param name="questionAndAnswer">The question and answer to add.</param>
    public void Add(QuestionAndAnswer questionAndAnswer)
    {
        ArgumentNullException.ThrowIfNull(questionAndAnswer);
        lock (_lock)
        {
            _allAnswers.Insert(0, questionAndAnswer);
        }
    }

    /// <summary>
    /// Clears all stored questions and answers. Used primarily for testing purposes.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _allAnswers.Clear();
        }
    }
}