namespace MVCRandomAnswerGenerator.Core.Domain;

/// <summary>
/// Provides deterministic Magic 8-Ball style answers to questions.
/// This implementation uses the question's hash code to ensure the same question always returns the same answer.
/// </summary>
public sealed class AnswerGenerator : IAnswerGenerator
{
    /// <summary>
    /// The collection of possible Magic 8-Ball answers.
    /// </summary>
    private static readonly string[] Answers = [
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

    /// <summary>
    /// Generates a deterministic answer to the specified question.
    /// The same question will always produce the same answer by using the question's hash code as a seed.
    /// </summary>
    /// <param name="question">The question to answer. Cannot be null.</param>
    /// <returns>A Magic 8-Ball style answer from the predefined set of answers.</returns>
    /// <exception cref="ArgumentNullException">Thrown when question is null.</exception>
    public string GenerateAnswer(string question)
    {
        ArgumentNullException.ThrowIfNull(question);
        
        var random = new Random(question.GetHashCode());
        var index = random.Next(Answers.Length);
        return Answers[index];
    }

    /// <summary>
    /// Asynchronously generates a deterministic answer to the specified question.
    /// This method completes synchronously but returns a ValueTask for API consistency.
    /// </summary>
    /// <param name="question">The question to answer. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation, containing a Magic 8-Ball style answer.</returns>
    /// <exception cref="ArgumentNullException">Thrown when question is null.</exception>
    public ValueTask<string> GenerateAnswerAsync(string question)
    {
        var answer = GenerateAnswer(question);
        return ValueTask.FromResult(answer);
    }
}