using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using MVCRandomAnswerGenerator.Core.Domain;

namespace MVCRandomAnswerGenerator.Core.Domain.Tests;

/// <summary>
/// Performance benchmarks for the AnswerGenerator to establish baseline metrics
/// and ensure no performance regression compared to the .NET Framework version.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RunStrategy.Monitoring, iterationCount: 100)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class PerformanceBenchmarks
{
    private readonly IAnswerGenerator _answerGenerator = new AnswerGenerator();
    private readonly string[] _testQuestions;
    private readonly string _longQuestion;

    public PerformanceBenchmarks()
    {
        // Prepare test data
        _testQuestions = [
            "Will this work?",
            "Is this a good idea?",
            "Should I continue?",
            "Will it rain tomorrow?",
            "Am I on the right track?",
            "What does the future hold?",
            "Is today a good day?",
            "Will my tests pass?",
            "Should I refactor this code?",
            "Is performance important?"
        ];
        
        _longQuestion = new string('a', 1000) + "?";
    }

    [Benchmark(Baseline = true)]
    public string GenerateAnswer_SingleQuestion()
    {
        return _answerGenerator.GenerateAnswer("Will this work?");
    }

    [Benchmark]
    public string GenerateAnswer_EmptyString()
    {
        return _answerGenerator.GenerateAnswer("");
    }

    [Benchmark]
    public string GenerateAnswer_LongQuestion()
    {
        return _answerGenerator.GenerateAnswer(_longQuestion);
    }

    [Benchmark]
    public string GenerateAnswer_UnicodeQuestion()
    {
        return _answerGenerator.GenerateAnswer("Will 🚀 work with émojis and accénts?");
    }

    [Benchmark]
    public List<string> GenerateAnswers_MultipleQuestions()
    {
        var results = new List<string>();
        foreach (var question in _testQuestions)
        {
            results.Add(_answerGenerator.GenerateAnswer(question));
        }
        return results;
    }

    [Benchmark]
    public HashSet<string> GenerateAnswers_UniquenessTest()
    {
        var results = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            var question = $"Test question {i}?";
            results.Add(_answerGenerator.GenerateAnswer(question));
        }
        return results;
    }

    [Benchmark]
    public List<string> GenerateAnswers_ConsistencyTest()
    {
        var results = new List<string>();
        const string question = "Consistency test question?";
        
        // Generate same answer multiple times to test deterministic behavior
        for (int i = 0; i < 10; i++)
        {
            results.Add(_answerGenerator.GenerateAnswer(question));
        }
        return results;
    }
}