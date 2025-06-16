using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MVCRandomAnswerGenerator.Core.Domain;
using MVCRandomAnswerGenerator.Core.Web.Configuration;

namespace MVCRandomAnswerGenerator.Core.Web.HealthChecks;

/// <summary>
/// Health check for the Answer Generator service to ensure it's functioning correctly.
/// </summary>
public sealed class AnswerGeneratorHealthCheck : IHealthCheck
{
    private readonly IAnswerGenerator _answerGenerator;
    private readonly IOptions<AnswerGeneratorOptions> _options;
    private readonly ILogger<AnswerGeneratorHealthCheck> _logger;

    public AnswerGeneratorHealthCheck(
        IAnswerGenerator answerGenerator,
        IOptions<AnswerGeneratorOptions> options,
        ILogger<AnswerGeneratorHealthCheck> logger)
    {
        _answerGenerator = answerGenerator;
        _options = options;
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Test the answer generator with a simple question
            var testQuestion = "Health check test question";
            var answer = _answerGenerator.GenerateAnswer(testQuestion);

            if (string.IsNullOrEmpty(answer))
            {
                _logger.LogWarning("Answer generator returned null or empty answer");
                return Task.FromResult(HealthCheckResult.Unhealthy("Answer generator returned null or empty answer"));
            }

            // Verify configuration is loaded
            var config = _options.Value;
            if (config.MaxStoredQuestions <= 0)
            {
                _logger.LogWarning("Invalid configuration: MaxStoredQuestions must be greater than 0");
                return Task.FromResult(HealthCheckResult.Degraded("Invalid configuration detected"));
            }

            _logger.LogDebug("Answer generator health check passed");
            
            var data = new Dictionary<string, object>
            {
                ["MaxStoredQuestions"] = config.MaxStoredQuestions,
                ["EnableCaching"] = config.EnableCaching,
                ["TestAnswer"] = answer
            };

            return Task.FromResult(HealthCheckResult.Healthy("Answer generator is functioning correctly", data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Answer generator health check failed");
            return Task.FromResult(HealthCheckResult.Unhealthy("Answer generator health check failed", ex));
        }
    }
}