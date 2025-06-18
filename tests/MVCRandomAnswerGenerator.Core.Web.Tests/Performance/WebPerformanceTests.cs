using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MVCRandomAnswerGenerator.Core.Web.Services;

namespace MVCRandomAnswerGenerator.Core.Web.Tests.Performance;

/// <summary>
/// Performance tests for web endpoints to establish baseline metrics.
/// These tests help ensure no performance regression during development.
/// </summary>
public class WebPerformanceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WebPerformanceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HomePage_LoadTest_MeetsPerformanceBaseline()
    {
        // Arrange
        using var client = _factory.CreateClient();
        const int numberOfRequests = 50;
        const int maxExpectedTimeMs = 5000; // 5 seconds for 50 requests
        
        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;
        var failureCount = 0;
        var responseTimes = new List<long>();

        // Act - Make concurrent requests
        var tasks = Enumerable.Range(0, numberOfRequests)
            .Select(async _ =>
            {
                var requestStopwatch = Stopwatch.StartNew();
                try
                {
                    var response = await client.GetAsync("/");
                    requestStopwatch.Stop();
                    responseTimes.Add(requestStopwatch.ElapsedMilliseconds);
                    
                    if (response.IsSuccessStatusCode)
                        Interlocked.Increment(ref successCount);
                    else
                        Interlocked.Increment(ref failureCount);
                }
                catch
                {
                    requestStopwatch.Stop();
                    Interlocked.Increment(ref failureCount);
                }
            });

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        successCount.Should().BeGreaterThan(45); // At least 90% success rate
        failureCount.Should().BeLessThan(5); // Less than 10% failure rate
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(maxExpectedTimeMs);
        
        if (responseTimes.Any())
        {
            var averageResponseTime = responseTimes.Average();
            averageResponseTime.Should().BeLessThan(500); // Average response time < 500ms
            
            // 95th percentile
            var sortedTimes = responseTimes.OrderBy(x => x).ToList();
            var p95Index = (int)Math.Ceiling(sortedTimes.Count * 0.95) - 1;
            var p95ResponseTime = sortedTimes[p95Index];
            p95ResponseTime.Should().BeLessThan(1000); // 95th percentile < 1000ms
        }
    }

    [Fact]
    public async Task AboutPage_LoadTest_MeetsPerformanceBaseline()
    {
        // Arrange
        using var client = _factory.CreateClient();
        const int numberOfRequests = 30;
        
        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;

        // Act
        var tasks = Enumerable.Range(0, numberOfRequests)
            .Select(async _ =>
            {
                try
                {
                    var response = await client.GetAsync("/Home/About");
                    if (response.IsSuccessStatusCode)
                        Interlocked.Increment(ref successCount);
                }
                catch
                {
                    // Count as failure
                }
            });

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        successCount.Should().BeGreaterThan(25); // At least 80% success rate
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(3000); // Complete within 3 seconds
    }

    [Fact]
    public async Task HealthCheck_LoadTest_MeetsPerformanceBaseline()
    {
        // Arrange
        using var client = _factory.CreateClient();
        const int numberOfRequests = 100; // Health checks should handle higher load
        
        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;
        var responseTimes = new List<long>();

        // Act
        var tasks = Enumerable.Range(0, numberOfRequests)
            .Select(async _ =>
            {
                var requestStopwatch = Stopwatch.StartNew();
                try
                {
                    var response = await client.GetAsync("/health");
                    requestStopwatch.Stop();
                    responseTimes.Add(requestStopwatch.ElapsedMilliseconds);
                    
                    if (response.IsSuccessStatusCode)
                        Interlocked.Increment(ref successCount);
                }
                catch
                {
                    requestStopwatch.Stop();
                }
            });

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert - Health checks should be very reliable and fast
        successCount.Should().BeGreaterThan(95); // At least 95% success rate
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000); // Complete within 2 seconds
        
        if (responseTimes.Any())
        {
            var averageResponseTime = responseTimes.Average();
            averageResponseTime.Should().BeLessThan(50); // Health checks should be very fast
        }
    }

    [Fact]
    public async Task QuestionSubmission_LoadTest_HandlesReasonableLoad()
    {
        // Arrange - Clear existing data
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuestionAnswerService>();
        service.Clear();

        using var client = _factory.CreateClient();
        
        // Pre-fetch anti-forgery token (simplified for performance test)
        var homeResponse = await client.GetAsync("/");
        var homeContent = await homeResponse.Content.ReadAsStringAsync();
        var token = ExtractAntiForgeryToken(homeContent);

        const int numberOfSubmissions = 20; // Reasonable load for form submissions
        var questions = new[]
        {
            "Will this performance test pass?",
            "Is the application fast enough?",
            "Can it handle the load?",
            "Should we optimize further?",
            "Are the metrics good?"
        };

        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;

        // Act
        var semaphore = new SemaphoreSlim(5); // Limit concurrency for form submissions
        var tasks = Enumerable.Range(0, numberOfSubmissions)
            .Select(async i =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var question = questions[i % questions.Length];
                    var formData = new Dictionary<string, string>
                    {
                        ["nextQuestion"] = question
                    };

                    if (token != null)
                    {
                        formData["__RequestVerificationToken"] = token;
                    }

                    var formContent = new FormUrlEncodedContent(formData);
                    var response = await client.PostAsync("/", formContent);
                    
                    if (response.IsSuccessStatusCode)
                        Interlocked.Increment(ref successCount);
                }
                catch
                {
                    // Count as failure
                }
                finally
                {
                    semaphore.Release();
                }
            });

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert - Form submissions may have lower success rate due to anti-forgery complexity
        successCount.Should().BeGreaterThan(15); // At least 75% success rate
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10000); // Complete within 10 seconds
    }

    [Fact]
    public async Task ConcurrentUsers_MixedWorkload_SystemStability()
    {
        // Arrange
        using var client = _factory.CreateClient();
        const int requestsPerEndpoint = 20;
        
        var stopwatch = Stopwatch.StartNew();
        var totalSuccessCount = 0;

        // Act - Multiple concurrent workloads
        var homePageTasks = Enumerable.Range(0, requestsPerEndpoint)
            .Select(async _ =>
            {
                try
                {
                    var response = await client.GetAsync("/");
                    if (response.IsSuccessStatusCode)
                        Interlocked.Increment(ref totalSuccessCount);
                }
                catch { }
            });

        var aboutPageTasks = Enumerable.Range(0, requestsPerEndpoint / 2)
            .Select(async _ =>
            {
                try
                {
                    var response = await client.GetAsync("/Home/About");
                    if (response.IsSuccessStatusCode)
                        Interlocked.Increment(ref totalSuccessCount);
                }
                catch { }
            });

        var healthCheckTasks = Enumerable.Range(0, requestsPerEndpoint * 2)
            .Select(async _ =>
            {
                try
                {
                    var response = await client.GetAsync("/health");
                    if (response.IsSuccessStatusCode)
                        Interlocked.Increment(ref totalSuccessCount);
                }
                catch { }
            });

        await Task.WhenAll(homePageTasks.Concat(aboutPageTasks).Concat(healthCheckTasks));
        stopwatch.Stop();

        // Assert - System should handle mixed concurrent load
        var expectedMinimumSuccesses = requestsPerEndpoint + (requestsPerEndpoint / 2) + (requestsPerEndpoint * 2) - 10; // Allow some failures
        totalSuccessCount.Should().BeGreaterThan(expectedMinimumSuccesses);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(8000); // Complete within 8 seconds
    }

    [Fact]
    public async Task StaticFiles_LoadTest_OptimalPerformance()
    {
        // Arrange
        using var client = _factory.CreateClient();
        const int numberOfRequests = 50;
        
        // Test various static file requests (may or may not exist, but should handle gracefully)
        var staticFiles = new[] { "/favicon.ico", "/css/site.css", "/js/site.js", "/lib/bootstrap/dist/css/bootstrap.min.css" };
        
        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;

        // Act
        var tasks = Enumerable.Range(0, numberOfRequests)
            .Select(async i =>
            {
                try
                {
                    var file = staticFiles[i % staticFiles.Length];
                    var response = await client.GetAsync(file);
                    
                    // Accept both OK (file exists) and NotFound (file doesn't exist) as success
                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound)
                        Interlocked.Increment(ref successCount);
                }
                catch
                {
                    // Count as failure
                }
            });

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        successCount.Should().BeGreaterThan(45); // At least 90% should get proper responses (200 or 404)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000); // Static file handling should be fast
    }

    [Fact]
    public async Task ResponseTime_SingleRequest_MeetsBaseline()
    {
        // Arrange
        using var client = _factory.CreateClient();
        
        // Act - Measure single request response time
        var stopwatch = Stopwatch.StartNew();
        var response = await client.GetAsync("/");
        stopwatch.Stop();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Single request < 1 second
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