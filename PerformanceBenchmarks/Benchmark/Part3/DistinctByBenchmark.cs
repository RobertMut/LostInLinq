using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using PerformanceBenchmarks.Benchmark.Core;
using PerformanceBenchmarks.Core.Struct;
using ZLinq;

namespace PerformanceBenchmarks.Benchmark.Part3;

[MemoryDiagnoser]
public class DistinctBenchmark
{
    public IEnumerable<Example[]> Data()
    {
        yield return Generator.Generate(500_000);
    }

    
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void LinqDistinct(Example[] input)
    {
        Enumerable.DistinctBy(input, x => x.ExampleEnum).ToArray().Consume(new Consumer());
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void ZLinqDistinct(Example[] input)
    {
        input.AsValueEnumerable().DistinctBy(x => x.ExampleEnum).ToArray().Consume(new Consumer());
    }
    
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void StructDistinct(Example[] input)
    {
        input.StructDistinctBy(x => x.ExampleEnum).ToArray().Consume(new Consumer());
    }
}