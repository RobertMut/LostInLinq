namespace PerformanceBenchmarks.Benchmark.Core;

public class Example
{
    public string ExampleString { get; set; }
    public int ExampleNumber { get; set; }
    public ExampleEnum ExampleEnum { get; set; }
}

public enum ExampleEnum {
    First,
    Second,
    Third
}