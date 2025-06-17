using MVCRandomAnswerGenerator.Core.Domain;

namespace MVCRandomAnswerGenerator.Core.Web.Services;

/// <summary>
/// Service for managing questions and answers in the application.
/// </summary>
public interface IQuestionAnswerService
{
    /// <summary>
    /// Gets all stored questions and answers.
    /// </summary>
    /// <returns>A list of questions and answers ordered by most recent first.</returns>
    IReadOnlyList<QuestionAndAnswer> GetAll();

    /// <summary>
    /// Adds a new question and answer to the collection.
    /// </summary>
    /// <param name="questionAndAnswer">The question and answer to add.</param>
    void Add(QuestionAndAnswer questionAndAnswer);

    /// <summary>
    /// Clears all stored questions and answers. Used primarily for testing purposes.
    /// </summary>
    void Clear();
}