using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using PerformanceBenchmarks.Benchmark.Core;
using PerformanceBenchmarks.Core.Struct;
using ZLinq;

namespace PerformanceBenchmarks.Benchmark.Part3;

[MemoryDiagnoser]
public class GroupByBenchmark
{
    public IEnumerable<Example[]> Data()
    {
        yield return Generator.Generate(500_000);
    }
    
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void LinqGroupBy(Example[] input)
    {
        input.GroupBy(x => x.ExampleNumber % 3).ToArray().Consume(new Consumer());
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void ZLinqGroupBy(Example[] input)
    {
        input.AsValueEnumerable().GroupBy(x => x.ExampleNumber % 3).ToArray().Consume(new Consumer());
    }
    
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void StructGroupBy(Example[] input)
    {
        input.StructGroupBy(x => x.ExampleNumber % 3).ToArray().Consume(new Consumer());
    }
}