```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.116
  [Host]     : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2
  Job-BDXEZF : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2

IterationCount=100  RunStrategy=Monitoring  

```
| Method                            | Mean       | Error     | StdDev     | Median     | Min        | Max        | Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------- |-----------:|----------:|-----------:|-----------:|-----------:|-----------:|------:|--------:|----------:|------------:|
| GenerateAnswer_SingleQuestion     |  10.267 μs | 21.604 μs |  63.700 μs |   3.211 μs |   2.865 μs |   639.9 μs |  1.00 |    0.00 |   1.02 KB |        1.00 |
| GenerateAnswer_EmptyString        |  10.714 μs | 20.427 μs |  60.229 μs |   4.869 μs |   3.015 μs |   606.5 μs |  1.30 |    0.26 |   1.02 KB |        1.00 |
| GenerateAnswer_LongQuestion       |  11.054 μs | 20.900 μs |  61.626 μs |   4.809 μs |   3.898 μs |   621.0 μs |  1.39 |    0.37 |   1.02 KB |        1.00 |
| GenerateAnswer_UnicodeQuestion    |   9.983 μs | 21.639 μs |  63.803 μs |   3.191 μs |   2.965 μs |   640.9 μs |  0.97 |    0.17 |   1.02 KB |        1.00 |
| GenerateAnswers_MultipleQuestions |  26.314 μs | 31.629 μs |  93.260 μs |  16.942 μs |  13.495 μs |   949.1 μs |  4.87 |    1.16 |   4.01 KB |        3.95 |
| GenerateAnswers_UniquenessTest    | 188.113 μs | 42.104 μs | 124.146 μs | 172.248 μs | 168.245 μs | 1,413.9 μs | 50.65 |   10.41 |  37.39 KB |       36.82 |
| GenerateAnswers_ConsistencyTest   |  18.623 μs | 30.694 μs |  90.502 μs |   8.812 μs |   8.376 μs |   914.3 μs |  2.75 |    0.72 |   4.01 KB |        3.95 |
