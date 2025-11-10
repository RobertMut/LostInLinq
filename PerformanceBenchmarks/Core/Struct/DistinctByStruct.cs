using MyHashSet.MinimalHashSet;
using PerformanceBenchmarks.Core.Abstractions;
using PerformanceBenchmarks.Core.Base;

namespace PerformanceBenchmarks.Core.Struct;

public static class DistinctByStruct
{
    public static StructEnumerable<InternalDistinctBy<ArrayStructEnumerator<T>, T, TKey>, T> StructDistinctBy<T, TKey>(
        this T[] array,
        Func<T, TKey> keySelector,
        EqualityComparer<TKey>? comparer = null)
    {
        return new(new InternalDistinctBy<ArrayStructEnumerator<T>, T, TKey>(new ArrayStructEnumerator<T>(array), keySelector, comparer));
    }

    public static StructEnumerable<InternalDistinctBy<TEnumerator, T, TKey>, T> StructDistinctBy<TEnumerator, T, TKey>(
        this StructEnumerable<TEnumerator, T> source, Func<T, TKey> keySelector, EqualityComparer<TKey>? comparer = null)
    where TEnumerator : struct, IStructEnumerator<T>, allows ref struct
    {
        return new(new InternalDistinctBy<TEnumerator, T, TKey>(source.Enumerator, keySelector, comparer));
    }
}

public ref struct InternalDistinctBy<TEnumerator, TIn, TKey> : IStructEnumerator<TIn>
    where TEnumerator : struct, IStructEnumerator<TIn>, allows ref struct
{
    private TEnumerator _source;
    private readonly Func<TIn, TKey> _selector;
    private MinimalHashSet<TKey> _set;

    public InternalDistinctBy(TEnumerator source, Func<TIn, TKey> selector, EqualityComparer<TKey>? comparer = null)
    {
        _source = source;
        _selector = selector;
        _set = new MinimalHashSet<TKey>(comparer ?? EqualityComparer<TKey>.Default);
    }

    public void Dispose()
    {
        _set.Dispose();
        _source.Dispose();
    }

    public bool Next(ref TIn current)
    {
        TIn? fromSource = default(TIn);
        while (_source.Next(ref fromSource))
        {
            if (_set.Add(_selector(fromSource)))
            {
                current = fromSource;
                return true;
            }
        }

        return false;
    }

    public bool GetCountToLeftEnumerate(out int count)
    {
        count = 0;

        return false;
    }

    public bool GetUnderlying(ref ReadOnlySpan<TIn> span) => false;

    public bool TryCopy(ref Span<TIn> destination, int offset) => false;
}