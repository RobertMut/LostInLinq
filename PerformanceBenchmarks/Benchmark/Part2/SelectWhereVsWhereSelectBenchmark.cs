using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using PerformanceBenchmarks.Benchmark.Core;
using PerformanceBenchmarks.Core.Base;
using PerformanceBenchmarks.Core.Struct;
using ZLinq;

namespace PerformanceBenchmarks.Benchmark.Part2;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class SelectWhereVsWhereSelectBenchmark
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
    public void LinqSelectWhere(Example[] input) =>
        input.Select(x => new NewExample()
            {
                HalvedInt = x.ExampleNumber / 2,
                NewStringProp = x.ExampleString
            })
            .Where(x => x.NewStringProp[0] == 'c' || x.NewStringProp[1] == '2')
            .ToArray()
            .Consume(new Consumer());

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void LinqWhereSelect(Example[] input) =>
        input.Where(x => x.ExampleString[0] == 'c' || x.ExampleString[1] == '2')
            .Select(x => new NewExample()
            {
                HalvedInt = x.ExampleNumber / 2,
                NewStringProp = x.ExampleString
            })
            .ToArray()
            .Consume(new Consumer());

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void ParallelLinqSelectWhere(Example[] input) =>
        input.AsParallel()
            .Select(x => new NewExample()
            {
                HalvedInt = x.ExampleNumber / 2,
                NewStringProp = x.ExampleString
            })
            .Where(x => x.NewStringProp[0] == 'c' || x.NewStringProp[1] == '2')
            .ToArray().Consume(new Consumer());

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void ParallelLinqWhereSelect(Example[] input) =>
        input
            .AsParallel()
            .Where(x => x.ExampleString[0] == 'c' || x.ExampleString[1] == '2')
            .Select(x => new NewExample()
            {
                HalvedInt = x.ExampleNumber / 2,
                NewStringProp = x.ExampleString
            })
            .ToArray().Consume(new Consumer());

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void NonAllocWhereSelect_WithoutShortcut(Example[] input)
    {
        new InternalSelect<InternalWhere<ArrayStructEnumerator<Example>, Example>, Example, NewExample>(
                new InternalWhere<ArrayStructEnumerator<Example>, Example>(new ArrayStructEnumerator<Example>(input),
                    x => x.ExampleString[0] == 'c' || x.ExampleString[1] == '2'), x => new NewExample())
            .ToArray()
            .Consume(new Consumer());
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void NonAllocSelectWhere(Example[] input)
    {
        input
            .StructSelect(x => new NewExample()
            {
                HalvedInt = x.ExampleNumber / 2,
                NewStringProp = x.ExampleString
            })
            .StructWhere(x => x.NewStringProp[0] == 'c' || x.NewStringProp[1] == '2')
            .ToArray().Consume(new Consumer());
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void NonAllocWhereSelect(Example[] input)
    {
        input.StructWhere(x => x.ExampleString[0] == 'c' || x.ExampleString[1] == '2')
            .StructSelect(x => new NewExample()
            {
                HalvedInt = x.ExampleNumber / 2,
                NewStringProp = x.ExampleString
            })
            .ToArray().Consume(new Consumer());
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void ZLinqSelectWhere(Example[] input)
    {
        input.AsValueEnumerable()
            .Select(x => new NewExample()
            {
                HalvedInt = x.ExampleNumber / 2,
                NewStringProp = x.ExampleString
            })
            .Where(x => x.NewStringProp[0] == 'c' || x.NewStringProp[1] == '2')
            .ToArray().Consume(new Consumer());
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void ZLinqWhereSelect(Example[] input)
    {
        input.AsValueEnumerable()
            .Where(x => x.ExampleString[0] == 'c' || x.ExampleString[1] == '2')
            .Select(x => new NewExample()
            {
                HalvedInt = x.ExampleNumber / 2,
                NewStringProp = x.ExampleString
            }).ToArray().Consume(new Consumer());
    }
}