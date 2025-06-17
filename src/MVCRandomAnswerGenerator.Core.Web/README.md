# MVCRandomAnswerGenerator.Core.Web

This is the ASP.NET Core 8 web application for the Random Answer Generator, implementing modern patterns and best practices.

## Features Implemented

### ✅ ASP.NET Core 8 Web Application Setup
- Modern ASP.NET Core 8 project structure with minimal hosting model
- SDK-style project file with .NET 8 targeting
- Project reference to `MVCRandomAnswerGenerator.Core.Domain`

### ✅ Dependency Injection
- Scoped registration of `IAnswerGenerator` service
- Constructor injection throughout the application
- Service validation at startup

### ✅ Configuration System
- Strongly-typed configuration options using `AnswerGeneratorOptions`
- Configuration validation with data annotations
- Separate development and production settings
- Options pattern with `IOptions<T>`

### ✅ Structured Logging
- Console and debug logging providers
- JSON logging for production environments
- Structured logging with correlation and context
- Log level configuration per environment

### ✅ Health Checks
- Custom health check for Answer Generator service
- Health check endpoint at `/health` with JSON response
- Validation of service functionality and configuration
- Detailed health check data including test results

### ✅ Environment Configuration
- Separate appsettings for Development and Production
- Environment-specific logging configuration
- Development exception page for debugging
- HSTS and HTTPS redirection for production

### ✅ Hot Reload Support
- Hot reload enabled for development profiles
- Development-friendly configuration
- Fast iteration during development

## Project Structure

```
MVCRandomAnswerGenerator.Core.Web/
├── Configuration/
│   └── AnswerGeneratorOptions.cs      # Strongly-typed configuration
├── HealthChecks/
│   └── AnswerGeneratorHealthCheck.cs  # Custom health check
├── Properties/
│   └── launchSettings.json            # Launch profiles with hot reload
├── Program.cs                         # Application startup and configuration
├── appsettings.json                   # Production configuration
├── appsettings.Development.json       # Development configuration
└── MVCRandomAnswerGenerator.Core.Web.csproj
```

## Endpoints

- **`/`** - Root endpoint (placeholder for HomeController)
- **`/health`** - Health check endpoint with JSON response

## Next Steps (Issue 7)

The web application is ready for controller migration:
- Migrate HomeController from .NET Framework version
- Implement modern MVC patterns with dependency injection
- Add async/await patterns where appropriate
- Implement proper model validation and error handling

## Configuration Options

The application supports the following configuration options:

```json
{
  "AnswerGenerator": {
    "MaxStoredQuestions": 100,
    "EnableCaching": true,
    "ApplicationTitle": "ASP.NET Core Random Answer Generator",
    "ApplicationDescription": "A Magic 8-Ball style answer generator built with ASP.NET Core 8"
  }
}
```

## Running the Application

```bash
# Development
dotnet run

# Production
dotnet run --environment Production
```

The application will start on:
- HTTP: http://localhost:5196
- HTTPS: https://localhost:7087