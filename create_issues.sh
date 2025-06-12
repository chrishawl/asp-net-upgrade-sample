#!/bin/bash

# Create milestones
echo "Creating milestones..."

gh api repos/chrishawl/asp-net-upgrade-sample/milestones --method POST --field title="Phase 2: Core Business Logic Migration" --field description="Migrate core business logic to .NET 8 with modern patterns"

gh api repos/chrishawl/asp-net-upgrade-sample/milestones --method POST --field title="Phase 3: Web Layer Migration" --field description="Migrate controllers, views, and web-specific functionality"

gh api repos/chrishawl/asp-net-upgrade-sample/milestones --method POST --field title="Phase 4: Infrastructure and Deployment" --field description="CI/CD pipelines and containerization"

gh api repos/chrishawl/asp-net-upgrade-sample/milestones --method POST --field title="Phase 5: Performance and Observability" --field description="Performance optimization, monitoring, and security"

gh api repos/chrishawl/asp-net-upgrade-sample/milestones --method POST --field title="Phase 6: Documentation and Finalization" --field description="Documentation, performance comparison, and validation"

gh api repos/chrishawl/asp-net-upgrade-sample/milestones --method POST --field title="Phase 7: Production Readiness" --field description="Production deployment strategy and final validation"

echo "Milestones created successfully!"

# Create labels
echo "Creating labels..."

gh label create "priority:critical" --color "d73a4a" --description "Critical priority items that block other work"
gh label create "priority:high" --color "ff6600" --description "High priority items"
gh label create "priority:medium" --color "fbca04" --description "Medium priority items"
gh label create "phase:foundation" --color "0e8a16" --description "Phase 1: Foundation and Setup"
gh label create "phase:core-logic" --color "1d76db" --description "Phase 2: Core Business Logic Migration"
gh label create "phase:web-layer" --color "5319e7" --description "Phase 3: Web Layer Migration"
gh label create "phase:infrastructure" --color "f9d0c4" --description "Phase 4: Infrastructure and Deployment"
gh label create "phase:performance" --color "c2e0c6" --description "Phase 5: Performance and Observability"
gh label create "phase:documentation" --color "fef2c0" --description "Phase 6: Documentation and Finalization"
gh label create "phase:production" --color "d4c5f9" --description "Phase 7: Production Readiness"
gh label create "migration" --color "ededed" --description ".NET Framework to .NET 8 migration"
gh label create "testing" --color "bfd4f2" --description "Testing related tasks"

echo "Labels created successfully!"

# Create Issues
echo "Creating issues..."

# Issue 1
gh issue create \
  --title "Issue 1: Repository Structure Setup" \
  --body "**Priority**: Critical  
**Estimated Effort**: 2-4 hours  
**Dependencies**: None

**Description**: Establish the parallel development structure to support both .NET Framework and .NET 8 versions side-by-side.

**Tasks**:
- [ ] Create \`src/\` directory structure
- [ ] Move existing code to \`src/MVCRandomAnswerGenerator.Framework/\`
- [ ] Create \`src/MVCRandomAnswerGenerator.Core/\` for .NET 8 version
- [ ] Create \`tests/\` directory structure
- [ ] Update solution file to include both projects

**Acceptance Criteria**:
- [ ] Both projects can be built independently
- [ ] Solution file includes both Framework and Core projects
- [ ] Directory structure follows .NET best practices
- [ ] README.md updated with new structure

**Testing Strategy**:
- Verify both projects build without errors
- Verify solution loads correctly in Visual Studio/VS Code" \
  --label "priority:critical,phase:foundation,migration" \
  --milestone "Phase 1: Foundation and Setup"

# Issue 2
gh issue create \
  --title "Issue 2: Legacy Codebase Unit Tests" \
  --body "**Priority**: Critical  
**Estimated Effort**: 4-6 hours  
**Dependencies**: Issue 1

**Description**: Create comprehensive unit tests for the existing .NET Framework application to establish a testing baseline.

**Tasks**:
- [ ] Create \`tests/MVCRandomAnswerGenerator.Framework.Tests/\` project
- [ ] Add xUnit test framework via NuGet
- [ ] Implement unit tests for \`AnswerGenerator\` class
- [ ] Implement unit tests for \`HomeController\` class
- [ ] Implement unit tests for \`QuestionAndAnswer\` model
- [ ] Add integration tests for MVC pipeline

**Acceptance Criteria**:
- [ ] \`AnswerGenerator.GenerateAnswer()\` has comprehensive tests
- [ ] \`HomeController\` GET and POST actions have tests
- [ ] Model validation tests exist
- [ ] Test coverage > 90% for core business logic
- [ ] All tests pass consistently

**Test Examples**:
\`\`\`csharp
// AnswerGenerator Tests
[Fact]
public void GenerateAnswer_WithSameQuestion_ReturnsSameAnswer()
[Fact]
public void GenerateAnswer_WithDifferentQuestions_ReturnsDifferentAnswers()
[Theory]
public void GenerateAnswer_WithValidQuestion_ReturnsKnownAnswer()

// HomeController Tests
[Fact]
public void Index_GET_ReturnsViewWithModel()
[Fact]
public void Index_POST_AddsQuestionToModel()
[Fact]
public void Index_POST_RedirectsToIndex()
\`\`\`" \
  --label "priority:critical,phase:foundation,migration,testing" \
  --milestone "Phase 1: Foundation and Setup"

# Issue 3
gh issue create \
  --title "Issue 3: CI/CD Pipeline for Legacy Application" \
  --body "**Priority**: High  
**Estimated Effort**: 3-5 hours  
**Dependencies**: Issue 2

**Description**: Establish GitHub Actions pipeline for the .NET Framework version to ensure consistent builds and test execution.

**Tasks**:
- [ ] Create \`.github/workflows/dotnet-framework.yml\`
- [ ] Configure Windows runner for .NET Framework builds
- [ ] Add build, test, and artifact generation steps
- [ ] Configure test result reporting
- [ ] Add code coverage reporting

**Acceptance Criteria**:
- [ ] Pipeline builds .NET Framework project successfully
- [ ] All unit tests run and results are reported
- [ ] Code coverage metrics are collected
- [ ] Artifacts are generated for deployments
- [ ] Pipeline runs on PR and main branch commits

**Pipeline Structure**:
\`\`\`yaml
name: .NET Framework CI/CD
on: [push, pull_request]
jobs:
  build-and-test:
    runs-on: windows-latest
    steps:
      - name: Checkout
      - name: Setup MSBuild
      - name: Restore NuGet packages
      - name: Build solution
      - name: Run tests
      - name: Generate coverage report
\`\`\`" \
  --label "priority:high,phase:foundation,migration" \
  --milestone "Phase 1: Foundation and Setup"

# Issue 4
gh issue create \
  --title "Issue 4: Business Logic Library Migration" \
  --body "**Priority**: Critical  
**Estimated Effort**: 6-8 hours  
**Dependencies**: Issue 3

**Description**: Migrate core business logic (\`AnswerGenerator\` and models) to .NET 8 class library, modernizing the code with latest C# features.

**Tasks**:
- [ ] Create \`src/MVCRandomAnswerGenerator.Core.Domain/\` class library
- [ ] Migrate \`AnswerGenerator\` class with modern C# 12 features
- [ ] Migrate \`QuestionAndAnswer\` model using record types
- [ ] Implement \`IAnswerGenerator\` interface for dependency injection
- [ ] Add nullable reference types support
- [ ] Implement async patterns where appropriate

**Acceptance Criteria**:
- [ ] \`AnswerGenerator\` uses modern C# patterns (collection expressions, primary constructors)
- [ ] \`QuestionAndAnswer\` implemented as record type with validation
- [ ] Interface-based design for testability
- [ ] Nullable reference types enabled
- [ ] XML documentation for public APIs
- [ ] Benchmark tests show equivalent or better performance" \
  --label "priority:critical,phase:core-logic,migration" \
  --milestone "Phase 2: Core Business Logic Migration"

# Issue 5
gh issue create \
  --title "Issue 5: Business Logic Unit Tests (.NET 8)" \
  --body "**Priority**: Critical  
**Estimated Effort**: 4-6 hours  
**Dependencies**: Issue 4

**Description**: Create comprehensive unit tests for the .NET 8 business logic components using modern testing patterns.

**Tasks**:
- [ ] Create \`tests/MVCRandomAnswerGenerator.Core.Domain.Tests/\` project
- [ ] Implement unit tests using xUnit and modern .NET 8 testing features
- [ ] Add performance benchmarks using BenchmarkDotNet
- [ ] Implement property-based testing where appropriate
- [ ] Add tests for async functionality

**Acceptance Criteria**:
- [ ] All business logic has equivalent test coverage to Framework version
- [ ] Performance benchmarks confirm no regression
- [ ] Async functionality properly tested
- [ ] Property-based tests for edge cases
- [ ] Test coverage > 95% for core business logic" \
  --label "priority:critical,phase:core-logic,migration,testing" \
  --milestone "Phase 2: Core Business Logic Migration"

# Issue 6
gh issue create \
  --title "Issue 6: ASP.NET Core Web Application Setup" \
  --body "**Priority**: Critical  
**Estimated Effort**: 8-12 hours  
**Dependencies**: Issue 5

**Description**: Create the ASP.NET Core 8 web application structure with modern patterns and dependency injection.

**Tasks**:
- [ ] Create \`src/MVCRandomAnswerGenerator.Core.Web/\` ASP.NET Core project
- [ ] Configure modern dependency injection container
- [ ] Set up modern configuration system (appsettings.json)
- [ ] Implement modern logging with structured logging
- [ ] Configure modern authentication/authorization (if needed)
- [ ] Set up health checks and observability

**Acceptance Criteria**:
- [ ] ASP.NET Core 8 project template created
- [ ] Dependency injection properly configured
- [ ] Configuration system uses strongly-typed options
- [ ] Structured logging implemented
- [ ] Health checks endpoint available
- [ ] Development/Production configurations separated
- [ ] Hot reload enabled for development" \
  --label "priority:critical,phase:core-logic,migration" \
  --milestone "Phase 2: Core Business Logic Migration"

# Issue 7
gh issue create \
  --title "Issue 7: Controller Migration to ASP.NET Core" \
  --body "**Priority**: Critical  
**Estimated Effort**: 6-8 hours  
**Dependencies**: Issue 6

**Description**: Migrate the HomeController to ASP.NET Core with modern patterns, dependency injection, and async/await.

**Tasks**:
- [ ] Migrate \`HomeController\` to ASP.NET Core controller base class
- [ ] Implement constructor dependency injection
- [ ] Convert actions to async patterns where appropriate
- [ ] Implement modern model binding and validation
- [ ] Add proper error handling and logging
- [ ] Implement modern anti-forgery token handling

**Acceptance Criteria**:
- [ ] Controller uses dependency injection for all dependencies
- [ ] Actions follow async/await patterns where appropriate
- [ ] Model validation uses modern validation attributes
- [ ] Error handling provides structured error responses
- [ ] Logging integrated with ASP.NET Core logging pipeline
- [ ] Anti-forgery tokens properly implemented" \
  --label "priority:critical,phase:web-layer,migration" \
  --milestone "Phase 3: Web Layer Migration"

# Issue 8
gh issue create \
  --title "Issue 8: View Migration to Modern Razor" \
  --body "**Priority**: High  
**Estimated Effort**: 4-6 hours  
**Dependencies**: Issue 7

**Description**: Migrate Razor views to ASP.NET Core with modern HTML helpers, tag helpers, and responsive design.

**Tasks**:
- [ ] Migrate \`_Layout.cshtml\` to ASP.NET Core layout
- [ ] Update \`Index.cshtml\` with modern Razor syntax
- [ ] Implement modern tag helpers instead of HTML helpers
- [ ] Update CSS to modern Bootstrap 5
- [ ] Implement responsive design patterns
- [ ] Add modern client-side validation

**Acceptance Criteria**:
- [ ] Views use modern Razor syntax and tag helpers
- [ ] Bootstrap 5 implemented with responsive design
- [ ] Client-side validation works properly
- [ ] Layout is mobile-friendly
- [ ] Accessibility standards met (WCAG 2.1)
- [ ] Modern form handling implemented" \
  --label "priority:high,phase:web-layer,migration" \
  --milestone "Phase 3: Web Layer Migration"

# Issue 9
gh issue create \
  --title "Issue 9: Web Application Unit Tests (.NET 8)" \
  --body "**Priority**: Critical  
**Estimated Effort**: 6-8 hours  
**Dependencies**: Issue 8

**Description**: Create comprehensive unit and integration tests for the ASP.NET Core web application.

**Tasks**:
- [ ] Create \`tests/MVCRandomAnswerGenerator.Core.Web.Tests/\` project
- [ ] Implement controller unit tests with mocking
- [ ] Create integration tests using ASP.NET Core Test Host
- [ ] Add end-to-end tests for complete user workflows
- [ ] Implement API testing for future API endpoints
- [ ] Add performance tests for web endpoints

**Acceptance Criteria**:
- [ ] Controller actions have comprehensive unit tests
- [ ] Integration tests cover complete request/response cycles
- [ ] End-to-end tests verify user workflows
- [ ] Performance tests establish baseline metrics
- [ ] Test coverage > 90% for web layer
- [ ] All tests run reliably in CI/CD pipeline" \
  --label "priority:critical,phase:web-layer,migration,testing" \
  --milestone "Phase 3: Web Layer Migration"

# Issue 10
gh issue create \
  --title "Issue 10: CI/CD Pipeline for .NET 8 Application" \
  --body "**Priority**: High  
**Estimated Effort**: 4-6 hours  
**Dependencies**: Issue 9

**Description**: Create comprehensive CI/CD pipeline for the .NET 8 application with modern DevOps practices.

**Tasks**:
- [ ] Create \`.github/workflows/dotnet-core.yml\`
- [ ] Configure multi-platform builds (Windows, Linux, macOS)
- [ ] Add comprehensive test execution and reporting
- [ ] Implement code coverage with quality gates
- [ ] Add security scanning and dependency checks
- [ ] Configure Docker containerization
- [ ] Add deployment automation

**Acceptance Criteria**:
- [ ] Pipeline builds on multiple platforms
- [ ] All tests execute and results are reported
- [ ] Code coverage meets quality thresholds (>90%)
- [ ] Security vulnerabilities are detected and reported
- [ ] Docker images are built and tagged
- [ ] Deployment to staging environment automated
- [ ] Performance regression tests included" \
  --label "priority:high,phase:infrastructure,migration" \
  --milestone "Phase 4: Infrastructure and Deployment"

# Issue 11
gh issue create \
  --title "Issue 11: Docker Containerization" \
  --body "**Priority**: Medium  
**Estimated Effort**: 3-4 hours  
**Dependencies**: Issue 10

**Description**: Create optimized Docker containers for both applications with multi-stage builds and security best practices.

**Tasks**:
- [ ] Create optimized Dockerfile for .NET 8 application
- [ ] Implement multi-stage build for smaller images
- [ ] Add security scanning for container images
- [ ] Create docker-compose for local development
- [ ] Implement health checks in containers
- [ ] Configure for cloud deployment

**Acceptance Criteria**:
- [ ] Docker image builds successfully with minimal size
- [ ] Multi-stage build optimizes layer caching
- [ ] Container security scan passes
- [ ] Health checks respond correctly
- [ ] Application runs correctly in container
- [ ] Docker-compose enables local development" \
  --label "priority:medium,phase:infrastructure,migration" \
  --milestone "Phase 4: Infrastructure and Deployment"

# Issue 12
gh issue create \
  --title "Issue 12: Performance Optimization and Monitoring" \
  --body "**Priority**: Medium  
**Estimated Effort**: 4-6 hours  
**Dependencies**: Issue 11

**Description**: Implement comprehensive performance monitoring, optimization, and observability features for the .NET 8 application.

**Tasks**:
- [ ] Implement Application Performance Monitoring (APM)
- [ ] Add structured logging with correlation IDs
- [ ] Implement health checks and readiness probes
- [ ] Add performance counters and metrics
- [ ] Implement distributed tracing
- [ ] Add memory and performance profiling

**Acceptance Criteria**:
- [ ] APM dashboard shows application performance metrics
- [ ] Structured logging captures all important events
- [ ] Health checks provide detailed application status
- [ ] Performance benchmarks show improvement over Framework version
- [ ] Memory usage is optimized and monitored
- [ ] Tracing provides end-to-end request visibility" \
  --label "priority:medium,phase:performance,migration" \
  --milestone "Phase 5: Performance and Observability"

# Issue 13
gh issue create \
  --title "Issue 13: Security Hardening" \
  --body "**Priority**: High  
**Estimated Effort**: 3-5 hours  
**Dependencies**: Issue 12

**Description**: Implement comprehensive security measures and follow .NET 8 security best practices.

**Tasks**:
- [ ] Implement modern authentication/authorization patterns
- [ ] Add HTTPS enforcement and security headers
- [ ] Implement Content Security Policy (CSP)
- [ ] Add rate limiting and request throttling
- [ ] Implement input validation and sanitization
- [ ] Add security audit logging
- [ ] Configure secure cookies and session management

**Acceptance Criteria**:
- [ ] HTTPS enforced in all environments
- [ ] Security headers properly configured
- [ ] CSP prevents XSS attacks
- [ ] Rate limiting prevents abuse
- [ ] Input validation prevents injection attacks
- [ ] Security events properly logged
- [ ] Security scan passes with no high/critical issues" \
  --label "priority:high,phase:performance,migration" \
  --milestone "Phase 5: Performance and Observability"

# Issue 14
gh issue create \
  --title "Issue 14: Comprehensive Documentation" \
  --body "**Priority**: Medium  
**Estimated Effort**: 4-6 hours  
**Dependencies**: Issue 13

**Description**: Create comprehensive documentation for both applications, migration process, and operational procedures.

**Tasks**:
- [ ] Update README.md with architecture diagrams
- [ ] Create migration guide documentation
- [ ] Document API endpoints (if applicable)
- [ ] Create operational runbooks
- [ ] Document deployment procedures
- [ ] Create troubleshooting guides
- [ ] Add architecture decision records (ADRs)

**Acceptance Criteria**:
- [ ] README.md explains both applications and their differences
- [ ] Migration guide provides step-by-step instructions
- [ ] API documentation is complete and accurate
- [ ] Operational procedures are documented
- [ ] Troubleshooting guide covers common issues
- [ ] Architecture decisions are documented
- [ ] Documentation is up-to-date and accurate" \
  --label "priority:medium,phase:documentation,migration" \
  --milestone "Phase 6: Documentation and Finalization"

# Issue 15
gh issue create \
  --title "Issue 15: Performance Comparison and Validation" \
  --body "**Priority**: High  
**Estimated Effort**: 3-4 hours  
**Dependencies**: Issue 14

**Description**: Conduct comprehensive performance comparison between .NET Framework and .NET 8 versions to validate migration success.

**Tasks**:
- [ ] Create performance benchmarking suite
- [ ] Compare memory usage between versions
- [ ] Measure request/response times
- [ ] Analyze startup performance
- [ ] Compare resource utilization
- [ ] Document performance improvements
- [ ] Create performance regression tests

**Acceptance Criteria**:
- [ ] Benchmark suite covers all major functionality
- [ ] .NET 8 version shows measurable performance improvements
- [ ] Memory usage is optimized in .NET 8 version
- [ ] Startup time is faster in .NET 8 version
- [ ] Performance regression tests prevent future degradation
- [ ] Performance report documents all improvements
- [ ] Baseline metrics established for monitoring" \
  --label "priority:high,phase:documentation,migration,testing" \
  --milestone "Phase 6: Documentation and Finalization"

# Issue 16
gh issue create \
  --title "Issue 16: Production Deployment Strategy" \
  --body "**Priority**: Critical  
**Estimated Effort**: 6-8 hours  
**Dependencies**: Issue 15

**Description**: Implement production-ready deployment strategy with blue-green deployment, monitoring, and rollback capabilities.

**Tasks**:
- [ ] Configure production environments
- [ ] Implement blue-green deployment strategy
- [ ] Set up production monitoring and alerting
- [ ] Configure backup and disaster recovery
- [ ] Implement feature flags for gradual rollout
- [ ] Create rollback procedures
- [ ] Configure production security measures

**Acceptance Criteria**:
- [ ] Blue-green deployment successfully implemented
- [ ] Production monitoring alerts on issues
- [ ] Backup and recovery procedures tested
- [ ] Feature flags enable controlled rollout
- [ ] Rollback procedures tested and documented
- [ ] Production security measures active
- [ ] Load testing validates production readiness" \
  --label "priority:critical,phase:production,migration" \
  --milestone "Phase 7: Production Readiness"

echo "All issues created successfully!"
