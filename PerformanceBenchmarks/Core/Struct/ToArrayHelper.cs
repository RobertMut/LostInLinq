using PerformanceBenchmarks.Core.Abstractions;
using PerformanceBenchmarks.Core.Base;

namespace PerformanceBenchmarks.Core.Struct;

public static class ToArrayHelper
{
    public static TOut[] ToArray<TEnumerator, TIn, TOut>(this InternalWhereSelect<TEnumerator, TIn, TOut> enumerator)
        where TEnumerator : struct, IStructEnumerator<TIn>
    {
        TOut[] result = null;

        var arr = new ArrayBuilder<TOut>();
        
        TOut? current = default;
        int idx = 0;
        while (enumerator.Next(ref current))
        {
            arr.Add(current);
        }

        result = arr.ToArray();
        
        arr.Clear();

        return result;
    }
    
    public static T[] ToArray<TEnumerator, T>(this InternalWhere<TEnumerator, T> enumerator) //gamechanger
        where TEnumerator : struct, IStructEnumerator<T>
    {
        T[] result = null;

        var arr = new ArrayBuilder<T>();
        
        T? current = default;
        int idx = 0;
        while (enumerator.Next(ref current))
        {
            arr.Add(current);
        }

        result = arr.ToArray();
        
        arr.Clear();

        return result;
    }
    
    public static T[] ToArray<T>(this IStructEnumerator<T> enumerator)
    {
        T[] result = null;

        if (enumerator.GetCountToLeftEnumerate(out int count))
        {
            result = GC.AllocateUninitializedArray<T>(count);
            
            T current = default;
            int idx = 0;
            
            while (enumerator.Next(ref current))
            {
                result[idx++] = current;
            }
        }

        return result;
    }
}