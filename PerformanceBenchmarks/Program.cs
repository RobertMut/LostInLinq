// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using PerformanceBenchmarks.Benchmark.Part1;
using PerformanceBenchmarks.Benchmark.Part2;
using PerformanceBenchmarks.Benchmark.Part3;

var benchmarksToRun = new[]
{
    typeof(SelectBenchmark), 
    typeof(WhereBenchmark), 
    typeof(WhereSelectBenchmark),
    typeof(SelectWhereVsWhereSelectBenchmark),
    typeof(DistinctBenchmark),
    typeof(GroupByBenchmark)
};
BenchmarkRunner.Run(benchmarksToRun);