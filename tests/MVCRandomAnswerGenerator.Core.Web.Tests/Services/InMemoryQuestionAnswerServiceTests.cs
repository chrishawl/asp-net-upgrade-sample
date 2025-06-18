using MVCRandomAnswerGenerator.Core.Domain;
using MVCRandomAnswerGenerator.Core.Web.Services;

namespace MVCRandomAnswerGenerator.Core.Web.Tests.Services;

public class InMemoryQuestionAnswerServiceTests
{
    private readonly InMemoryQuestionAnswerService _service;

    public InMemoryQuestionAnswerServiceTests()
    {
        _service = new InMemoryQuestionAnswerService();
    }

    [Fact]
    public void GetAll_WhenEmpty_ReturnsEmptyList()
    {
        // Act
        var result = _service.GetAll();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Add_WithValidQuestionAndAnswer_AddsToCollection()
    {
        // Arrange
        var qa = new QuestionAndAnswer("Test question?", "Test answer");

        // Act
        _service.Add(qa);
        var result = _service.GetAll();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(qa);
    }

    [Fact]
    public void Add_WithMultipleItems_AddsInCorrectOrder()
    {
        // Arrange
        var qa1 = new QuestionAndAnswer("First question?", "First answer");
        var qa2 = new QuestionAndAnswer("Second question?", "Second answer");
        var qa3 = new QuestionAndAnswer("Third question?", "Third answer");

        // Act
        _service.Add(qa1);
        _service.Add(qa2);
        _service.Add(qa3);
        var result = _service.GetAll();

        // Assert
        result.Should().HaveCount(3);
        // Should be in reverse order (most recent first)
        result[0].Should().Be(qa3);
        result[1].Should().Be(qa2);
        result[2].Should().Be(qa1);
    }

    [Fact]
    public void Add_WithNullQuestionAndAnswer_ThrowsArgumentNullException()
    {
        // Act & Assert
        var action = () => _service.Add(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Clear_RemovesAllItems()
    {
        // Arrange
        var qa1 = new QuestionAndAnswer("Test question 1?", "Test answer 1");
        var qa2 = new QuestionAndAnswer("Test question 2?", "Test answer 2");
        _service.Add(qa1);
        _service.Add(qa2);

        // Act
        _service.Clear();
        var result = _service.GetAll();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAll_ReturnsReadOnlyList()
    {
        // Arrange
        var qa = new QuestionAndAnswer("Test question?", "Test answer");
        _service.Add(qa);

        // Act
        var result = _service.GetAll();

        // Assert
        result.Should().BeAssignableTo<IReadOnlyList<QuestionAndAnswer>>();
    }

    [Fact]
    public async Task ConcurrentAccess_ThreadSafe()
    {
        // Arrange
        const int numberOfThreads = 10;
        const int itemsPerThread = 100;
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < numberOfThreads; i++)
        {
            int threadIndex = i;
            var task = Task.Run(() =>
            {
                for (int j = 0; j < itemsPerThread; j++)
                {
                    var qa = new QuestionAndAnswer($"Thread {threadIndex} Question {j}?", $"Thread {threadIndex} Answer {j}");
                    _service.Add(qa);
                }
            });
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        var result = _service.GetAll();

        // Assert
        result.Should().HaveCount(numberOfThreads * itemsPerThread);
    }
}