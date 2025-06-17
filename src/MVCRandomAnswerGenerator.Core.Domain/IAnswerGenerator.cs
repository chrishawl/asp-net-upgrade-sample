namespace MVCRandomAnswerGenerator.Core.Domain;

/// <summary>
/// Provides methods to generate answers to questions using a Magic 8-Ball style approach.
/// </summary>
public interface IAnswerGenerator
{
    /// <summary>
    /// Generates a deterministic answer to the specified question.
    /// </summary>
    /// <param name="question">The question to answer. Cannot be null.</param>
    /// <returns>A Magic 8-Ball style answer.</returns>
    /// <exception cref="ArgumentNullException">Thrown when question is null.</exception>
    string GenerateAnswer(string question);

    /// <summary>
    /// Generates a deterministic answer to the specified question asynchronously.
    /// </summary>
    /// <param name="question">The question to answer. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation containing a Magic 8-Ball style answer.</returns>
    /// <exception cref="ArgumentNullException">Thrown when question is null.</exception>
    Task<string> GenerateAnswerAsync(string question);
}