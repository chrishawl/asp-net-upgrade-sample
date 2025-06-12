# ASP.NET Framework to .NET 8 Migration Sample

This repository demonstrates a comprehensive migration path from ASP.NET MVC (.NET Framework 4.7.2) to ASP.NET Core (.NET 8).

## Overview

This project showcases a **side-by-side migration approach** where both the legacy .NET Framework and modern .NET 8 versions coexist during the migration process. This ensures:

- **Zero downtime** during migration
- **Functional parity** validation at each step  
- **Performance comparison** between versions
- **Risk mitigation** with fallback options

## Project Structure

```
├── src/
│   ├── MVCRandomAnswerGenerator.Framework/  # .NET Framework 4.7.2 MVC app
│   └── MVCRandomAnswerGenerator.Core/       # .NET 8 version (placeholder)
├── tests/                                   # Test projects for both versions
├── MIGRATION_ROADMAP.md                     # Comprehensive 16-issue migration plan
└── aspnet-upgrade-sample.sln               # Solution file for both projects
```

### Source Projects
- **Framework Version**: `src/MVCRandomAnswerGenerator.Framework/` - Original .NET Framework 4.7.2 MVC app
- **Core Version**: `src/MVCRandomAnswerGenerator.Core/` - Modern .NET 8 version (to be implemented)

### Test Projects
- **Framework Tests**: `tests/MVCRandomAnswerGenerator.Framework.Tests/` - Unit tests for legacy version
- **Core Tests**: `tests/MVCRandomAnswerGenerator.Core.*.Tests/` - Unit tests for .NET 8 version

This side-by-side structure allows for:
- Independent development and testing of both versions
- Easy comparison between legacy and modern implementations
- Gradual migration with reduced risk

## Migration Strategy

The migration follows a **7-phase approach** with **16 individual issues**:

1. **Phase 1: Foundation and Setup** (Issues 1-3)
2. **Phase 2: Core Business Logic Migration** (Issues 4-6)  
3. **Phase 3: Web Layer Migration** (Issues 7-9)
4. **Phase 4: Infrastructure and Deployment** (Issues 10-11)
5. **Phase 5: Performance and Observability** (Issues 12-13)
6. **Phase 6: Documentation and Finalization** (Issues 14-15)
7. **Phase 7: Production Readiness** (Issue 16)

## Getting Started

1. **Review the Migration Roadmap**: Start with `MIGRATION_ROADMAP.md`
2. **Check GitHub Issues**: Each phase has detailed issues with acceptance criteria
3. **Run the Legacy App**: Test the current .NET Framework version
4. **Follow the Issues**: Work through each issue systematically

## Key Features

- **Modern .NET 8 patterns**: Primary constructors, collection expressions, record types
- **Comprehensive testing**: >90% test coverage for both versions
- **CI/CD pipelines**: Automated builds and tests for both Framework and Core versions
- **Performance benchmarking**: Validate improvements during migration
- **Security hardening**: Modern security practices and scanning
- **Containerization**: Docker support with multi-stage builds

## Technology Stack

### Legacy (.NET Framework)
- ASP.NET MVC 5.2.7
- .NET Framework 4.7.2
- Entity Framework 6.x
- Bootstrap 4.5.0

### Modern (.NET 8)
- ASP.NET Core 8
- C# 12 with modern features
- Entity Framework Core 8
- Bootstrap 5
- Docker containerization
- OpenTelemetry observability

## Documentation

- `MIGRATION_ROADMAP.md` - Complete migration strategy and timeline
- GitHub Issues - Individual task breakdowns with acceptance criteria
- Architecture Decision Records (ADRs) - Coming in Phase 6

This migration approach ensures a **safe, incremental, and well-tested** path to modernizing your .NET Framework applications.
