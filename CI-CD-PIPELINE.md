# .NET 8 CI/CD Pipeline Documentation

This document describes the comprehensive CI/CD pipeline implemented for the .NET 8 application migration.

## Pipeline Overview

The `.github/workflows/dotnet-core.yml` pipeline provides enterprise-grade CI/CD with the following features:

### 🏗️ **Multi-Platform Builds**
- **Linux** (ubuntu-latest): Primary platform for deployment
- **Windows** (windows-latest): Compatibility testing  
- **macOS** (macos-latest): Cross-platform validation

### 🧪 **Comprehensive Testing**
- **Unit Tests**: 74 domain tests + 14 web tests
- **Performance Benchmarks**: 7 BenchmarkDotNet scenarios
- **Code Coverage**: Quality gates with 90% threshold for business logic
- **Test Reporting**: TRX format with detailed results

### 🔒 **Security & Quality**
- **CodeQL Analysis**: GitHub's semantic code analysis
- **Vulnerability Scanning**: NuGet package security audit
- **Container Security**: Trivy scanning for Docker images
- **Dependency Checks**: Automated security vulnerability detection

### 🐳 **Docker Containerization**
- **Multi-stage Builds**: Optimized for production
- **Security**: Non-root user, minimal attack surface
- **Health Checks**: Built-in application health monitoring
- **Container Testing**: Automated deployment validation

### 📊 **Quality Gates**
- **Code Coverage**: 90% threshold for business logic
- **Test Results**: All tests must pass
- **Security Scan**: No high/critical vulnerabilities
- **Container Health**: Health endpoint validation

### 🚀 **Deployment Automation**
- **Staging Environment**: Automated deployment on main branch
- **Artifact Management**: Docker images, test results, coverage reports
- **Performance Monitoring**: Benchmark regression detection

## Pipeline Jobs

### 1. **build-and-test**
```yaml
strategy:
  matrix:
    os: [ubuntu-latest, windows-latest, macos-latest]
```

**Responsibilities:**
- Build all .NET 8 projects
- Execute unit tests with coverage collection
- Run performance benchmarks (Ubuntu only)
- Upload test results and coverage data
- Cache NuGet packages for performance

### 2. **code-quality**
```yaml
runs-on: ubuntu-latest
needs: build-and-test
```

**Responsibilities:**
- Run CodeQL security analysis
- Perform .NET security audit
- Check for vulnerable dependencies
- Generate SARIF security reports

### 3. **docker-build**
```yaml
runs-on: ubuntu-latest
needs: build-and-test
if: github.ref == 'refs/heads/main' || github.event_name == 'pull_request'
```

**Responsibilities:**
- Build optimized Docker images
- Run container security scan (Trivy)
- Test container functionality
- Cache Docker layers for performance
- Export Docker artifacts

### 4. **coverage-check**
```yaml
runs-on: ubuntu-latest
needs: build-and-test
```

**Responsibilities:**
- Generate detailed coverage reports
- Enforce 90% coverage threshold for business logic
- Create HTML coverage reports
- Validate test quality metrics

### 5. **deploy-staging**
```yaml
runs-on: ubuntu-latest
needs: [build-and-test, code-quality, docker-build, coverage-check]
if: github.ref == 'refs/heads/main' && github.event_name == 'push'
environment: staging
```

**Responsibilities:**
- Deploy to staging environment
- Load and tag Docker images
- Execute deployment validation
- Prepare for production deployment

## Configuration Files

### Docker Configuration
- **`Dockerfile`**: Multi-stage production build
- **`docker-compose.yml`**: Local development environment
- **`docker-compose.override.yml`**: Development overrides
- **`.dockerignore`**: Optimized build context

### Coverage Configuration
- **`tests/*/coverlet.runsettings`**: Coverage collection settings
- **Exclusions**: Test projects, Program.cs, generated code
- **Formats**: Cobertura XML for reporting

### Pipeline Triggers
```yaml
on:
  push:
    branches: [ main, develop ]
    paths: [ 'src/MVCRandomAnswerGenerator.Core.*/**', ... ]
  pull_request:
    branches: [ main ]
```

## Performance Benchmarks

The pipeline includes automated performance testing with BenchmarkDotNet:

| Benchmark | Mean | Allocated |
|-----------|------|-----------|
| Single Question | ~9.4 μs | 1.02 KB |
| Long Question | ~10.8 μs | 1.02 KB |
| Multiple Questions | ~24 μs | 4.01 KB |
| Uniqueness Test | ~194 μs | 37.39 KB |

## Quality Metrics

### Code Coverage Requirements
- **Business Logic**: 90% minimum (Domain project)
- **Web Controllers**: High coverage expected
- **Infrastructure**: Coverage informational only

### Security Requirements
- **No critical vulnerabilities** in dependencies
- **Container security scan** must pass
- **CodeQL analysis** must complete successfully

### Test Requirements
- **All unit tests** must pass
- **Performance benchmarks** must complete
- **Multi-platform compatibility** verified

## Local Development

### Running Tests with Coverage
```bash
# Domain tests
dotnet test tests/MVCRandomAnswerGenerator.Core.Domain.Tests/ --collect:"XPlat Code Coverage"

# Web tests  
dotnet test tests/MVCRandomAnswerGenerator.Core.Web.Tests/ --collect:"XPlat Code Coverage"

# Generate coverage report
reportgenerator -reports:"**/*.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"Html"
```

### Docker Development
```bash
# Build and run locally
docker-compose up --build

# Development with hot reload
docker-compose -f docker-compose.yml -f docker-compose.override.yml up

# Health check
curl http://localhost:5000/health
```

### Performance Testing
```bash
# Run benchmarks
dotnet run --project tests/MVCRandomAnswerGenerator.Core.Domain.Tests/ -c Release -- --benchmark
```

## Environment Variables

### Required Secrets
- **`CODECOV_TOKEN`**: For coverage reporting (optional)
- **Additional deployment secrets** as needed

### Configuration
- **`DOTNET_VERSION`**: 8.0.x
- **`BUILD_CONFIGURATION`**: Release
- **`DOCKER_IMAGE_NAME`**: mvc-random-answer-generator

## Monitoring and Alerts

### Pipeline Notifications
- **PR Status Checks**: Required for merge
- **Test Result Comments**: Detailed feedback
- **Coverage Reports**: HTML artifacts
- **Performance Trends**: Benchmark history

### Health Monitoring
- **Container Health Checks**: `/health` endpoint
- **Application Metrics**: Built-in ASP.NET Core metrics
- **Performance Regression**: Benchmark comparison

## Deployment Strategy

### Staging Environment
- **Automatic deployment** on main branch
- **Environment protection** rules
- **Rollback capability** via Docker tags

### Production Readiness
- **All quality gates** must pass
- **Security scans** completed
- **Performance validation** confirmed
- **Manual approval** gates (configured separately)

---

This pipeline implements modern DevOps practices ensuring high-quality, secure, and performant .NET 8 applications with comprehensive testing and deployment automation.