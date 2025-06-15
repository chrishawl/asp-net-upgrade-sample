# MVCRandomAnswerGenerator.Core.Domain.Tests

This project contains comprehensive unit tests for the .NET 8 business logic components using modern testing patterns.

## Test Coverage

The test suite includes **88 tests** covering:

### Core Functionality Tests
- **AnswerGeneratorTests.cs**: Basic functionality tests (3 tests)
- **QuestionAndAnswerTests.cs**: Record type functionality tests (6 tests)  
- **FrameworkCompatibilityTests.cs**: Compatibility with .NET Framework version (8 tests)

### Modern Testing Patterns
- **AsyncFunctionalityTests.cs**: Comprehensive async method testing (12 tests)
- **PropertyBasedTests.cs**: Property-based testing using FsCheck (5 property tests)
- **EdgeCaseTests.cs**: Edge cases and boundary conditions (54 tests)

### Performance Testing
- **PerformanceBenchmarks.cs**: BenchmarkDotNet performance benchmarks
- **BenchmarkRunner.cs**: Console entry point for running benchmarks

## Running Tests

```bash
# Run all tests
dotnet test tests/MVCRandomAnswerGenerator.Core.Domain.Tests/

# Run tests with code coverage
dotnet test tests/MVCRandomAnswerGenerator.Core.Domain.Tests/ --collect:"XPlat Code Coverage"

# Run performance benchmarks
dotnet run --project tests/MVCRandomAnswerGenerator.Core.Domain.Tests -c Release -- --benchmark
```

## Features Tested

### Core Business Logic
- ✅ Deterministic answer generation
- ✅ Null parameter validation
- ✅ All 20 Magic 8-Ball answers coverage
- ✅ Consistency across multiple calls

### Async Functionality  
- ✅ Async/sync result parity
- ✅ Concurrent call consistency
- ✅ ValueTask optimization
- ✅ ConfigureAwait behavior
- ✅ Performance under load

### Edge Cases
- ✅ Empty and whitespace strings
- ✅ Unicode characters (emojis, accents, non-Latin scripts)
- ✅ Very long strings (100,000+ characters)
- ✅ Control characters and special symbols
- ✅ Boundary length conditions
- ✅ Mixed content types
- ✅ Stress testing (10,000+ iterations)

### Property-Based Testing
- ✅ Any valid string produces valid answer
- ✅ Deterministic behavior with identical inputs
- ✅ Valid outputs for varying input lengths
- ✅ Unicode character handling
- ✅ Input/output relationship properties

### Performance Benchmarks
- ✅ Single question generation
- ✅ Empty string handling
- ✅ Long question processing
- ✅ Unicode question handling
- ✅ Multiple question batches
- ✅ Uniqueness testing
- ✅ Consistency verification

## Modern .NET 8 Features Used

- **xUnit** for unit testing framework
- **FluentAssertions** for readable assertions
- **BenchmarkDotNet** for performance benchmarking
- **FsCheck.Xunit** for property-based testing
- **ValueTask<T>** for optimized async operations
- **Code coverage** collection and reporting
- **Global using statements** for cleaner imports

## Test Quality Metrics

- **Total Tests**: 88
- **Test Categories**: 6 different test types
- **Edge Cases Covered**: 50+ scenarios
- **Performance Tests**: 7 benchmark scenarios
- **Property Tests**: 5 property-based validations
- **Async Tests**: 12 async-specific tests

All tests pass consistently and maintain >95% code coverage for core business logic.