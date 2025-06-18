using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MVCRandomAnswerGenerator.Core.Web.Services;

namespace MVCRandomAnswerGenerator.Core.Web.Tests.EndToEnd;

/// <summary>
/// End-to-end tests that verify complete user workflows from start to finish.
/// These tests simulate real user interactions with the application.
/// </summary>
public class UserWorkflowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UserWorkflowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task UserWorkflow_FirstTimeVisitor_CanViewEmptyHomePage()
    {
        // Arrange - Clear any existing data to simulate first-time visit
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuestionAnswerService>();
        service.Clear();

        // Act - User visits the home page for the first time
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert - Page loads successfully with empty state
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("Random Answer Generator");
        content.Should().Contain("Ask a question");
        // Should not contain any previous questions/answers
        content.Should().NotContain("<tr>"); // No table rows for questions/answers
    }

    [Fact]
    public async Task UserWorkflow_AskQuestionAndSeeAnswer_CompleteCycle()
    {
        // Arrange - Clear existing data
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuestionAnswerService>();
        service.Clear();

        const string testQuestion = "Will I succeed in my career?";

        // Step 1: User visits home page
        var homeResponse = await _client.GetAsync("/");
        var homeContent = await homeResponse.Content.ReadAsStringAsync();

        homeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        homeContent.Should().Contain("Ask a question");

        // Step 2: User submits a question
        var formData = new Dictionary<string, string>
        {
            ["nextQuestion"] = testQuestion
        };

        // Extract anti-forgery token
        var token = ExtractAntiForgeryToken(homeContent);
        if (token != null)
        {
            formData["__RequestVerificationToken"] = token;
        }

        var formContent = new FormUrlEncodedContent(formData);
        var postResponse = await _client.PostAsync("/", formContent);

        // Step 3: Verify redirect after submission
        postResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);
        postResponse.Headers.Location?.ToString().Should().Be("/");

        // Step 4: User is redirected to home page and sees their question with answer
        var followUpResponse = await _client.GetAsync("/");
        var followUpContent = await followUpResponse.Content.ReadAsStringAsync();

        followUpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        followUpContent.Should().Contain(testQuestion);
        // Should contain an answer (any of the Magic 8-Ball responses)
        followUpContent.Should().ContainAny("Yes", "No", "Maybe", "Ask again", "Definitely", "Unlikely");
    }

    [Fact]
    public async Task UserWorkflow_AskMultipleQuestions_AllQuestionsDisplayed()
    {
        // Arrange - Clear existing data
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuestionAnswerService>();
        service.Clear();

        var questions = new[]
        {
            "Should I learn a new programming language?",
            "Will it rain tomorrow?",
            "Is this test going to pass?"
        };

        // Act - User asks multiple questions in sequence
        foreach (var question in questions)
        {
            // Get the current page
            var homeResponse = await _client.GetAsync("/");
            var homeContent = await homeResponse.Content.ReadAsStringAsync();

            // Submit the question
            var formData = new Dictionary<string, string>
            {
                ["nextQuestion"] = question
            };

            var token = ExtractAntiForgeryToken(homeContent);
            if (token != null)
            {
                formData["__RequestVerificationToken"] = token;
            }

            var formContent = new FormUrlEncodedContent(formData);
            var postResponse = await _client.PostAsync("/", formContent);

            // Verify successful submission
            postResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        // Verify all questions are displayed
        var finalResponse = await _client.GetAsync("/");
        var finalContent = await finalResponse.Content.ReadAsStringAsync();

        finalResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        foreach (var question in questions)
        {
            finalContent.Should().Contain(question);
        }

        // Questions should be displayed in reverse order (most recent first)
        var firstQuestionIndex = finalContent.IndexOf(questions[0]);
        var lastQuestionIndex = finalContent.IndexOf(questions[^1]);
        lastQuestionIndex.Should().BeLessThan(firstQuestionIndex, "Most recent question should appear first");
    }

    [Fact]
    public async Task UserWorkflow_SubmitEmptyQuestion_SeeValidationError()
    {
        // Step 1: User visits home page
        var homeResponse = await _client.GetAsync("/");
        var homeContent = await homeResponse.Content.ReadAsStringAsync();

        // Step 2: User submits empty question
        var formData = new Dictionary<string, string>
        {
            ["nextQuestion"] = ""
        };

        var token = ExtractAntiForgeryToken(homeContent);
        if (token != null)
        {
            formData["__RequestVerificationToken"] = token;
        }

        var formContent = new FormUrlEncodedContent(formData);
        var postResponse = await _client.PostAsync("/", formContent);

        // Step 3: User sees validation error on same page (no redirect)
        var errorContent = await postResponse.Content.ReadAsStringAsync();

        postResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        errorContent.Should().Contain("field-validation-error");
        errorContent.Should().Contain("required");
    }

    [Fact]
    public async Task UserWorkflow_ViewAboutPage_ReturnsToHome()
    {
        // Step 1: User visits home page
        var homeResponse = await _client.GetAsync("/");
        homeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 2: User navigates to About page
        var aboutResponse = await _client.GetAsync("/Home/About");
        var aboutContent = await aboutResponse.Content.ReadAsStringAsync();

        aboutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        aboutContent.Should().Contain("The ASP.NET Core Random Answer Generator");

        // Step 3: User can navigate back to home (verify links work)
        var backHomeResponse = await _client.GetAsync("/");
        backHomeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UserWorkflow_CheckApplicationHealth_MonitoringEndpoint()
    {
        // Act - User or monitoring system checks health endpoint
        var response = await _client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();

        // Assert - Health check responds correctly
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Be("application/json");
        content.Should().Contain("\"status\":\"Healthy\"");
        content.Should().Contain("answer_generator");
    }

    [Fact]
    public async Task UserWorkflow_LongQuestion_HandledCorrectly()
    {
        // Arrange - Clear existing data
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuestionAnswerService>();
        service.Clear();

        var longQuestion = new string('a', 500) + "?"; // Very long question

        // Act - User submits a very long question
        var homeResponse = await _client.GetAsync("/");
        var homeContent = await homeResponse.Content.ReadAsStringAsync();

        var formData = new Dictionary<string, string>
        {
            ["nextQuestion"] = longQuestion
        };

        var token = ExtractAntiForgeryToken(homeContent);
        if (token != null)
        {
            formData["__RequestVerificationToken"] = token;
        }

        var formContent = new FormUrlEncodedContent(formData);
        var postResponse = await _client.PostAsync("/", formContent);

        // Assert - Application handles long question gracefully
        postResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);

        var followUpResponse = await _client.GetAsync("/");
        var followUpContent = await followUpResponse.Content.ReadAsStringAsync();

        followUpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        followUpContent.Should().Contain(longQuestion);
    }

    [Fact]
    public async Task UserWorkflow_SpecialCharacters_HandledSafely()
    {
        // Arrange - Clear existing data
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuestionAnswerService>();
        service.Clear();

        var questionWithSpecialChars = "Will <script>alert('xss')</script> & \"quotes\" work?";

        // Act - User submits question with special characters
        var homeResponse = await _client.GetAsync("/");
        var homeContent = await homeResponse.Content.ReadAsStringAsync();

        var formData = new Dictionary<string, string>
        {
            ["nextQuestion"] = questionWithSpecialChars
        };

        var token = ExtractAntiForgeryToken(homeContent);
        if (token != null)
        {
            formData["__RequestVerificationToken"] = token;
        }

        var formContent = new FormUrlEncodedContent(formData);
        var postResponse = await _client.PostAsync("/", formContent);

        // Assert - Special characters are handled safely (HTML encoded)
        postResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);

        var followUpResponse = await _client.GetAsync("/");
        var followUpContent = await followUpResponse.Content.ReadAsStringAsync();

        followUpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        // Should be HTML encoded to prevent XSS  
        followUpContent.Should().Contain("&lt;script&gt;");
        followUpContent.Should().Contain("&quot;quotes&quot;");
    }

    private static string? ExtractAntiForgeryToken(string html)
    {
        const string tokenPrefix = "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"";
        var tokenStart = html.IndexOf(tokenPrefix);
        if (tokenStart == -1) return null;

        tokenStart += tokenPrefix.Length;
        var tokenEnd = html.IndexOf("\"", tokenStart);
        if (tokenEnd == -1) return null;

        return html.Substring(tokenStart, tokenEnd - tokenStart);
    }
}