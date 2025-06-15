using BenchmarkDotNet.Running;
using MVCRandomAnswerGenerator.Core.Domain.Tests;

namespace MVCRandomAnswerGenerator.Core.Domain.Tests;

/// <summary>
/// Entry point for running performance benchmarks.
/// Run this with: dotnet run --project tests/MVCRandomAnswerGenerator.Core.Domain.Tests -c Release -- --benchmark
/// </summary>
public class BenchmarkProgram
{
    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--benchmark")
        {
            Console.WriteLine("Running performance benchmarks...");
            var summary = BenchmarkRunner.Run<PerformanceBenchmarks>();
            Console.WriteLine("Benchmark completed. Results saved to BenchmarkDotNet.Artifacts.");
        }
        else
        {
            Console.WriteLine("This is the benchmark runner for MVCRandomAnswerGenerator.Core.Domain.Tests");
            Console.WriteLine("To run benchmarks, use: dotnet run --project tests/MVCRandomAnswerGenerator.Core.Domain.Tests -c Release -- --benchmark");
        }
    }
}