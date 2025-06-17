using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MVCRandomAnswerGenerator.Core.Domain;
using MVCRandomAnswerGenerator.Core.Web.Configuration;
using MVCRandomAnswerGenerator.Core.Web.HealthChecks;
using MVCRandomAnswerGenerator.Core.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure strongly-typed options
builder.Services.Configure<AnswerGeneratorOptions>(
    builder.Configuration.GetSection(AnswerGeneratorOptions.SectionName));

// Validate configuration options at startup
builder.Services.AddOptions<AnswerGeneratorOptions>()
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Register business logic services
builder.Services.AddScoped<IAnswerGenerator, AnswerGenerator>();

// Register application services
builder.Services.AddSingleton<IQuestionAnswerService, InMemoryQuestionAnswerService>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<AnswerGeneratorHealthCheck>("answer_generator");

// Configure logging for structured logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddDebug();
}
// Add JSON logging for structured logs in production
if (!builder.Environment.IsDevelopment())
{
    builder.Logging.AddJsonConsole();
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Configure health checks endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                data = entry.Value.Data
            })
        });
        await context.Response.WriteAsync(result);
    }
});

// Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Log application startup
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var options = app.Services.GetRequiredService<IOptions<AnswerGeneratorOptions>>().Value;
logger.LogInformation("Starting {ApplicationTitle}", options.ApplicationTitle);
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("Configuration - MaxStoredQuestions: {MaxStoredQuestions}, EnableCaching: {EnableCaching}", 
    options.MaxStoredQuestions, options.EnableCaching);

app.Run();
