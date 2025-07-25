// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using PerformanceBenchmarks.Benchmark.Core;
using PerformanceBenchmarks.Benchmark.Part1;
using PerformanceBenchmarks.Benchmark.Part2;
using PerformanceBenchmarks.Core.Struct;

var benchmarksToRun = new[]
{
    typeof(SelectBenchmark), 
    typeof(WhereBenchmark), 
    typeof(WhereSelectBenchmark),
    typeof(SelectWhereVsWhereSelectBenchmark)
};
BenchmarkRunner.Run(benchmarksToRun);