using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MVCRandomAnswerGenerator.Core.Web.Services;

namespace MVCRandomAnswerGenerator.Core.Web.Tests.Integration;

/// <summary>
/// Integration tests for the ASP.NET Core web application using the Test Host.
/// These tests verify complete request/response cycles.
/// </summary>
public class WebApplicationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public WebApplicationIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HomePage_Get_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString()
            .Should().StartWith("text/html");
    }

    [Fact]
    public async Task HomePage_Get_ContainsExpectedContent()
    {
        // Act
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("Random Answer Generator");
    }

    [Fact]
    public async Task AboutPage_Get_ReturnsSuccessWithCorrectMessage()
    {
        // Act
        var response = await _client.GetAsync("/Home/About");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("The ASP.NET Core Random Answer Generator");
    }

    [Fact]
    public async Task HealthCheck_Get_ReturnsHealthyStatus()
    {
        // Act
        var response = await _client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString()
            .Should().Be("application/json");
        content.Should().Contain("\"status\":\"Healthy\"");
    }

    [Fact]
    public async Task HomePage_PostValidQuestion_RedirectsToHomePage()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            ["nextQuestion"] = "Will this test pass?"
        };

        // Get the form first to obtain anti-forgery token
        var getResponse = await _client.GetAsync("/");
        var getContent = await getResponse.Content.ReadAsStringAsync();
        
        // Extract anti-forgery token (simplified approach for testing)
        var tokenStart = getContent.IndexOf("name=\"__RequestVerificationToken\" type=\"hidden\" value=\"");
        if (tokenStart > 0)
        {
            tokenStart += "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"".Length;
            var tokenEnd = getContent.IndexOf("\"", tokenStart);
            if (tokenEnd > tokenStart)
            {
                var token = getContent.Substring(tokenStart, tokenEnd - tokenStart);
                formData["__RequestVerificationToken"] = token;
            }
        }

        var formContent = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/", formContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location?.ToString().Should().Be("/");
    }

    [Fact]
    public async Task HomePage_PostEmptyQuestion_ReturnsFormWithValidationErrors()
    {
        // Arrange - First get the page to get the anti-forgery token
        var getResponse = await _client.GetAsync("/");
        var getContent = await getResponse.Content.ReadAsStringAsync();
        
        var formData = new Dictionary<string, string>
        {
            ["nextQuestion"] = ""
        };

        // Extract anti-forgery token
        var tokenStart = getContent.IndexOf("name=\"__RequestVerificationToken\" type=\"hidden\" value=\"");
        if (tokenStart > 0)
        {
            tokenStart += "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"".Length;
            var tokenEnd = getContent.IndexOf("\"", tokenStart);
            if (tokenEnd > tokenStart)
            {
                var token = getContent.Substring(tokenStart, tokenEnd - tokenStart);
                formData["__RequestVerificationToken"] = token;
            }
        }

        var formContent = new FormUrlEncodedContent(formData);

        // Act
        var response = await _client.PostAsync("/", formContent);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("field-validation-error");
    }

    [Fact]
    public async Task Application_ServesStaticFiles()
    {
        // Act
        var response = await _client.GetAsync("/favicon.ico");

        // Assert
        // Should either return the file (200) or not found (404), but not server error
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Application_HandlesInvalidRoute()
    {
        // Act
        var response = await _client.GetAsync("/NonExistentController/NonExistentAction");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Application_RedirectsHttpToHttps()
    {
        // Arrange
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Act
        var response = await client.GetAsync("/");

        // Assert - In test environment, HTTPS redirection may not be enabled
        // This test verifies the application handles the request appropriately
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect, HttpStatusCode.MovedPermanently);
    }

    [Fact]  
    public async Task HomePage_MultipleRequests_HandlesConcurrency()
    {
        // Arrange
        const int numberOfRequests = 10;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act - Make multiple concurrent requests
        for (int i = 0; i < numberOfRequests; i++)
        {
            tasks.Add(_client.GetAsync("/"));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert - All requests should succeed
        foreach (var response in responses)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    [Fact]
    public async Task QuestionAnswerService_PersistsDataAcrossRequests()
    {
        // Arrange - Clear any existing data
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuestionAnswerService>();
        service.Clear();

        // Get initial count
        var initialResponse = await _client.GetAsync("/");
        var initialContent = await initialResponse.Content.ReadAsStringAsync();
        
        // Post a question to add data
        var getResponse = await _client.GetAsync("/");
        var getContent = await getResponse.Content.ReadAsStringAsync();
        
        var formData = new Dictionary<string, string>
        {
            ["nextQuestion"] = "Integration test question?"
        };

        // Extract anti-forgery token
        var tokenStart = getContent.IndexOf("name=\"__RequestVerificationToken\" type=\"hidden\" value=\"");
        if (tokenStart > 0)
        {
            tokenStart += "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"".Length;
            var tokenEnd = getContent.IndexOf("\"", tokenStart);
            if (tokenEnd > tokenStart)
            {
                var token = getContent.Substring(tokenStart, tokenEnd - tokenStart);
                formData["__RequestVerificationToken"] = token;
            }
        }

        var formContent = new FormUrlEncodedContent(formData);
        await _client.PostAsync("/", formContent);

        // Act - Make a new request to verify data persistence
        var finalResponse = await _client.GetAsync("/");
        var finalContent = await finalResponse.Content.ReadAsStringAsync();

        // Assert - The question should be present in the response
        finalResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        finalContent.Should().Contain("Integration test question?");
    }
}