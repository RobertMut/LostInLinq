using PerformanceBenchmarks.Core.Abstractions;
using PerformanceBenchmarks.Core.Base;

namespace PerformanceBenchmarks.Core.Struct;

public static class ToArrayHelper
{
    public static TIn[] ToArray<TEnumerator, TIn, TKey>(
        this StructEnumerable<InternalDistinctBy<TEnumerator, TIn, TKey>, TIn> enumerable)
        where TEnumerator : struct, IStructEnumerator<TIn>, allows ref struct
    {
        return enumerable.Enumerator.FromArrayBuilder<InternalDistinctBy<TEnumerator, TIn, TKey>, TIn>();
    }
    
    public static T[] ToArray<T>(this StructEnumerable<InternalWhereArray<T>, T> enumerable)
    {
        return enumerable.Enumerator.FromArrayBuilder<InternalWhereArray<T>, T>();
    }

    public static Grouping<TKey, TElement>[] ToArray<TEnumerator, TKey, TElement>(
        this StructEnumerable<InternalGroupBy<TEnumerator, TKey, TElement>, Grouping<TKey, TElement>> enumerator)
        where TEnumerator : struct, IStructEnumerator<TElement>, allows ref struct
    {
        ReadOnlySpan<Grouping<TKey, TElement>> span = default;
        if (enumerator.Enumerator.GetUnderlying(ref span))
        {
            return span.ToArray();
        }

        return null;
    }

    public static TOut[] ToArray<TEnumerator, TIn, TOut>(
        this StructEnumerable<InternalSelect<TEnumerator, TIn, TOut>, TOut> enumerable)
        where TEnumerator : struct, IStructEnumerator<TIn>, allows ref struct
    {
        return enumerable.Enumerator.FromArrayBuilder<InternalSelect<TEnumerator, TIn, TOut>, TOut>();
    }

    public static T[] ToArray<TEnumerator, T>(this StructEnumerable<InternalWhere<TEnumerator, T>, T> enumerable)
        where TEnumerator : struct, IStructEnumerator<T>, allows ref struct
    {
        return enumerable.Enumerator.FromArrayBuilder<InternalWhere<TEnumerator, T>, T>();
    }

    public static T[] ToArray<TEnumerator, T>(this StructEnumerable<TEnumerator, T> enumerable)
        where TEnumerator : struct, IStructEnumerator<T>, allows ref struct
    {
        var enumerator = enumerable.Enumerator;
        T[] result = null;
        ReadOnlySpan<T> span = default;
        
        if (enumerator.GetUnderlying(ref span))
        {
            return span.ToArray();
        }
        
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
    
    private static T[] FromArrayBuilder<TEnumerator, T>(this TEnumerator enumerator)
        where TEnumerator : IStructEnumerator<T>, allows ref struct
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
}