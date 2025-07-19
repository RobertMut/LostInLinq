namespace PerformanceBenchmarks.Benchmark.Part1;

public static class WhereExtensions
{
    public static IEnumerable<T> WhereUsingEnumerator<T>(
        this IEnumerable<T> enumerable, 
        Func<T, bool> filter)
    {
        T[] result = new T[enumerable.Count()];
        int idx = 0;
        
        using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                var curr = enumerator.Current;
                if (filter(curr))
                {
                    result[idx++] = curr;
                }   
            }
        }
        
        
        return Segment(result, idx);
    }

    private static T[] Segment<T>(T[] tooBig, int idx)
    {
        T[] cut = new T[idx + 1];

        for (int i = 0; i < idx; i++)
        {
            cut[i] = tooBig[i];
        }

        return cut;
    }
}