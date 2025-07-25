using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using PerformanceBenchmarks.Benchmark.Core;
using PerformanceBenchmarks.Core.Struct;
using ZLinq;

namespace PerformanceBenchmarks.Benchmark.Part1;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class WhereSelectBenchmark
{
    public IEnumerable<Example[]> Data()
    {
        //yield return Generator.Generate(5000);
        //yield return Generator.Generate(10000);
        //yield return Generator.Generate(50000);
        yield return Generator.Generate(500000);
    }


    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void LinqWhereSelect(Example[] input) =>
        input.Where(x => x.ExampleNumber % 2 == 0)
            .Select(x => new NewExample()
            {
                HalvedInt = x.ExampleNumber / 2,
                NewStringProp = x.ExampleString
            })
            .ToArray()
            .Consume(new Consumer());

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void ParallelLinqWhereSelect(Example[] input) =>
        input.AsParallel().Where(x => x.ExampleNumber % 2 == 0)
            .Select(x => new NewExample()
            {
                HalvedInt = x.ExampleNumber / 2,
                NewStringProp = x.ExampleString
            })
            .ToArray().Consume(new Consumer());

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void NonAllocWhereSelect(Example[] input)
    {
        input.StructWhere(x => x.ExampleNumber % 2 == 0)
            .StructSelect(x => new NewExample()
            {
                HalvedInt = x.ExampleNumber / 2,
                NewStringProp = x.ExampleString
            })
            .ToArray().Consume(new Consumer());
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void ZLinqWhereSelect(Example[] input)
    {
        input.AsValueEnumerable()
            .Where(x => x.ExampleNumber % 2 == 0)
            .Select(x => new NewExample()
            {
                HalvedInt = x.ExampleNumber / 2,
                NewStringProp = x.ExampleString
            }).ToArray().Consume(new Consumer());
    }
}