namespace PerformanceBenchmarks.Benchmark.Part1;

public static class SelectExtensions
{
    public static IEnumerable<TResult> SelectUsingEnumerator<T, TResult>(
        this IEnumerable<T> enumerable, 
        Func<T, TResult> func)
    {
        TResult[] result = new TResult[enumerable.Count()];
        
        using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
        {
            int idx = 0;
            while (enumerator.MoveNext())
            {
                
                result[idx++] = func(enumerator.Current);
            }
        }
        
        return result;
    }
}