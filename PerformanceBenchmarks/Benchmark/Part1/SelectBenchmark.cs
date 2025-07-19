using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using PerformanceBenchmarks.Benchmark.Core;
using PerformanceBenchmarks.Core.Struct;
using ZLinq;

namespace PerformanceBenchmarks.Benchmark.Part1;

 [MemoryDiagnoser]
 [ThreadingDiagnoser]
 
 public class SelectBenchmark
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
     public void LinqSelect(Example[] input) =>
         input.Select(x => new NewExample()
         {
             HalvedInt = x.ExampleNumber / 2,
             NewStringProp = x.ExampleString
         }).ToArray().Consume(new Consumer());
     
     [Benchmark]
     [ArgumentsSource(nameof(Data))]
     public void ParallelLinqSelect(Example[] input) =>
         input.AsParallel().Select(x => new NewExample()
         {
             HalvedInt = x.ExampleNumber / 2,
             NewStringProp = x.ExampleString
         }).ToArray().Consume(new Consumer());
     
     
     [Benchmark]
     [ArgumentsSource(nameof(Data))]
     public void SelectUsingEnumerator(Example[] input) =>
         input.SelectUsingEnumerator(x => new NewExample()
         {
             HalvedInt = x.ExampleNumber / 2,
             NewStringProp = x.ExampleString
         }).ToArray().Consume(new Consumer());
     
     [Benchmark]
     [ArgumentsSource(nameof(Data))]
     public void NonAllocSelect(Example[] input)
     {
         input.StructSelect(x => new NewExample()
         {
             HalvedInt = x.ExampleNumber / 2,
             NewStringProp = x.ExampleString
         }).ToArray().Consume(new Consumer());

     }
     
     [Benchmark]
     [ArgumentsSource(nameof(Data))]
     public void ZLinqSelect(Example[] input)
     {
         input.AsValueEnumerable().Select(x => new NewExample()
         {
             HalvedInt = x.ExampleNumber / 2,
             NewStringProp = x.ExampleString
         }).ToArray().Consume(new Consumer());

     }
 }