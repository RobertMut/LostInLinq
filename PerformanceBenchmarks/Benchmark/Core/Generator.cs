namespace PerformanceBenchmarks.Benchmark.Core;

public static class Generator
{
    public static Example[] Generate(int count)
    {
        Example[] arr = new Example[count];
        Random rng = new();
        
        for (int i = 0; i < count; i++)
        {
            arr[i] = new Example()
            { 
                ExampleString = Guid.NewGuid().ToString(),
                ExampleNumber = rng.Next(0, int.MaxValue),
                ExampleEnum = (ExampleEnum)rng.Next(0, 2)
            };
        }

        return arr;
    }
}