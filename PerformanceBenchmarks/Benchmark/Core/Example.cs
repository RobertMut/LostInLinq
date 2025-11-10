using System.Diagnostics;

namespace PerformanceBenchmarks.Benchmark.Core;

[DebuggerDisplay("String: {ExampleString}, Number: {ExampleNumber}, Enum: {ExampleEnum}")]
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