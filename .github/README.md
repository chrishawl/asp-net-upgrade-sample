# GitHub Actions CI/CD Pipeline

This repository includes a comprehensive GitHub Actions workflow for continuous integration and testing of the .NET Framework application.

## Workflow: `ci.yml`

### Triggers
- **Push**: Runs on pushes to `main`, `develop`, and any `copilot/*` branches
- **Pull Request**: Runs on PRs targeting `main` and `develop` branches

### Job: `build-and-test`
Runs on `windows-latest` to support .NET Framework 4.7.2 builds.

#### Steps:
1. **Checkout code** - Gets the latest source code
2. **Setup MSBuild** - Configures MSBuild for .NET Framework projects
3. **Setup NuGet** - Installs latest NuGet for package management
4. **Setup .NET Framework 4.7.2 Developer Pack** - Downloads and installs the required targeting pack
5. **Restore NuGet packages** - Restores all project dependencies
6. **Build solution** - Compiles the entire solution in Release configuration
7. **Create test results directory** - Prepares output location for test results
8. **Run xUnit tests** - Executes all unit tests using xUnit console runner
9. **Publish test results** - Reports test results in GitHub Actions UI
10. **Upload test results** - Archives test results as build artifacts

### Test Coverage
The pipeline runs comprehensive tests covering:
- **49 total tests** across all components
- **AnswerGenerator** logic and deterministic behavior
- **HomeController** MVC actions and security features
- **QuestionAndAnswer** model validation
- **MVC Pipeline** integration testing

### Test Results
- Test results are published to the GitHub Actions UI for easy viewing
- XML test reports are uploaded as artifacts for detailed analysis
- Failed tests will cause the build to fail, preventing broken code from being merged

### Requirements
- Windows runner (required for .NET Framework)
- .NET Framework 4.7.2 Developer Pack
- MSBuild and NuGet tools
- xUnit test runner

This CI pipeline ensures code quality and prevents regressions during the migration process from .NET Framework to .NET 8.