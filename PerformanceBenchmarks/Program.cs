// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using PerformanceBenchmarks.Benchmark.Part1;

var benchmarksToRun = new[]
{
    //typeof(SelectBenchmark), 
    typeof(WhereBenchmark), 
    //typeof(WhereSelectBenchmark)
};
BenchmarkRunner.Run(benchmarksToRun);