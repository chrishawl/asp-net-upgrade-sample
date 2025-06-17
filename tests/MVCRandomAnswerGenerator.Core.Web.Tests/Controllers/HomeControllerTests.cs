using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;
using MVCRandomAnswerGenerator.Core.Domain;
using MVCRandomAnswerGenerator.Core.Web.Controllers;
using MVCRandomAnswerGenerator.Core.Web.Services;

namespace MVCRandomAnswerGenerator.Core.Web.Tests.Controllers;

public class HomeControllerTests
{
    private readonly Mock<IAnswerGenerator> _mockAnswerGenerator;
    private readonly Mock<IQuestionAnswerService> _mockQuestionAnswerService;
    private readonly Mock<ILogger<HomeController>> _mockLogger;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _mockAnswerGenerator = new Mock<IAnswerGenerator>();
        _mockQuestionAnswerService = new Mock<IQuestionAnswerService>();
        _mockLogger = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_mockAnswerGenerator.Object, _mockQuestionAnswerService.Object, _mockLogger.Object);
    }

    [Fact]
    public void Index_GET_ReturnsViewWithAllAnswers()
    {
        // Arrange
        var expectedAnswers = new List<QuestionAndAnswer>
        {
            new("Test question?", "Test answer")
        };
        _mockQuestionAnswerService.Setup(x => x.GetAll()).Returns(expectedAnswers);

        // Act
        var result = _controller.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().BeEquivalentTo(expectedAnswers);
        
        _mockQuestionAnswerService.Verify(x => x.GetAll(), Times.Once);
    }

    [Fact]
    public async Task Index_POST_WithValidQuestion_AddsQuestionAndRedirects()
    {
        // Arrange
        const string question = "Test question?";
        const string expectedAnswer = "Test answer";
        _mockAnswerGenerator
            .Setup(x => x.GenerateAnswerAsync(question))
            .ReturnsAsync(expectedAnswer);

        // Act
        var result = await _controller.Index(question);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        var redirectResult = result as RedirectToActionResult;
        redirectResult!.ActionName.Should().Be(nameof(HomeController.Index));

        _mockAnswerGenerator.Verify(x => x.GenerateAnswerAsync(question), Times.Once);
        _mockQuestionAnswerService.Verify(x => x.Add(It.Is<QuestionAndAnswer>(qa => 
            qa.Question == question && qa.Answer == expectedAnswer)), Times.Once);
    }

    [Fact]
    public async Task Index_POST_WithInvalidModelState_ReturnsViewWithErrors()
    {
        // Arrange
        _controller.ModelState.AddModelError("nextQuestion", "The question is required.");
        var expectedAnswers = new List<QuestionAndAnswer>
        {
            new("Previous question?", "Previous answer")
        };
        _mockQuestionAnswerService.Setup(x => x.GetAll()).Returns(expectedAnswers);

        // Act
        var result = await _controller.Index("");

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().BeEquivalentTo(expectedAnswers);
        _controller.ModelState.IsValid.Should().BeFalse();

        _mockAnswerGenerator.Verify(x => x.GenerateAnswerAsync(It.IsAny<string>()), Times.Never);
        _mockQuestionAnswerService.Verify(x => x.Add(It.IsAny<QuestionAndAnswer>()), Times.Never);
    }

    [Fact]
    public async Task Index_POST_WhenAnswerGeneratorThrows_ReturnsViewWithError()
    {
        // Arrange
        const string question = "Test question?";
        var expectedException = new InvalidOperationException("Test exception");
        _mockAnswerGenerator
            .Setup(x => x.GenerateAnswerAsync(question))
            .ThrowsAsync(expectedException);
        
        var expectedAnswers = new List<QuestionAndAnswer>
        {
            new("Previous question?", "Previous answer")
        };
        _mockQuestionAnswerService.Setup(x => x.GetAll()).Returns(expectedAnswers);

        // Act
        var result = await _controller.Index(question);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().BeEquivalentTo(expectedAnswers);
        _controller.ModelState.IsValid.Should().BeFalse();
        _controller.ModelState[""].Errors.Should().HaveCount(1);
        _controller.ModelState[""].Errors[0].ErrorMessage.Should().Contain("error occurred");

        _mockQuestionAnswerService.Verify(x => x.Add(It.IsAny<QuestionAndAnswer>()), Times.Never);
    }

    [Fact]
    public void About_ReturnsViewWithMessage()
    {
        // Act
        var result = _controller.About();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.ViewData["Message"].Should().Be("The ASP.NET Core Random Answer Generator");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Index_POST_WithInvalidQuestion_DoesNotCallServices(string? invalidQuestion)
    {
        // Arrange
        if (string.IsNullOrWhiteSpace(invalidQuestion))
        {
            _controller.ModelState.AddModelError("nextQuestion", "The question is required.");
        }
        
        _mockQuestionAnswerService.Setup(x => x.GetAll()).Returns(new List<QuestionAndAnswer>());

        // Act
        var result = await _controller.Index(invalidQuestion);

        // Assert
        result.Should().BeOfType<ViewResult>();
        _mockAnswerGenerator.Verify(x => x.GenerateAnswerAsync(It.IsAny<string>()), Times.Never);
        _mockQuestionAnswerService.Verify(x => x.Add(It.IsAny<QuestionAndAnswer>()), Times.Never);
    }
}