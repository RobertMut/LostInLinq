using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using PerformanceBenchmarks.Benchmark.Core;
using PerformanceBenchmarks.Core.Struct;
using ZLinq;

namespace PerformanceBenchmarks.Benchmark.Part1;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class WhereBenchmark
{
    public IEnumerable<Example[]> Data()
    {
        //yield return Generator.Generate(5_000);
        //yield return Generator.Generate(10_000);
        //yield return Generator.Generate(50_000);
        yield return Generator.Generate(500_000);
    }


    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void LinqWhere(Example[] input) =>
        input.Where(x => x.ExampleNumber % 2 == 0)
            .ToArray()
            .Consume(new Consumer());

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void ParallelLinqWhere(Example[] input) =>
        input.AsParallel().Where(x => x.ExampleNumber % 2 == 0)
            .ToArray().Consume(new Consumer());

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void WhereUsingEnumerator(Example[] input) =>
        input.WhereUsingEnumerator(x => x.ExampleNumber % 2 == 0).ToArray().Consume(new Consumer());

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void NonAllocWhere(Example[] input)
    {
        input.StructWhere(x => x.ExampleNumber % 2 == 0).ToArray().Consume(new Consumer());
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void ZLinqWhere(Example[] input)
    {
        input.AsValueEnumerable().Where(x => x.ExampleNumber % 2 == 0).ToArray().Consume(new Consumer());
    }
}