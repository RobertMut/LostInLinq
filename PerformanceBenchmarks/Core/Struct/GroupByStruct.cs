using PerformanceBenchmarks.Core.Abstractions;
using PerformanceBenchmarks.Core.Base;

namespace PerformanceBenchmarks.Core.Struct;

public static class GroupByStruct
{
    public static StructEnumerable<InternalGroupBy<TEnumerator, TKey, TElement>, Grouping<TKey, TElement>>
        StructGroupBy<TEnumerator, TElement, TKey>(
            this StructEnumerable<TEnumerator, TElement> source,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey>? comparer = null)
        where TEnumerator : struct, IStructEnumerator<TElement>, allows ref struct
    {
        return new(new InternalGroupBy<TEnumerator, TKey, TElement>(source.Enumerator, keySelector, comparer));
    }

    public static StructEnumerable<InternalGroupBy<ArrayStructEnumerator<TElement>, TKey, TElement>,
            Grouping<TKey, TElement>>
        StructGroupBy<TElement, TKey>(
            this TElement[] array,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey>? comparer = null)
    {
        return new(new InternalGroupBy<ArrayStructEnumerator<TElement>, TKey, TElement>(
            new ArrayStructEnumerator<TElement>(array),
            keySelector, comparer));
    }
}

public ref struct InternalGroupBy<TEnumerator, TKey, TElement> : IStructEnumerator<Grouping<TKey, TElement>>
    where TEnumerator : struct, IStructEnumerator<TElement>, allows ref struct
{
    private TEnumerator _source;
    private readonly Func<TElement, TKey> _selector;
    private readonly IEqualityComparer<TKey> _comparer;
    private Base.Lookup<TKey, TElement> _lookup;
    private ArrayStructEnumerator<Grouping<TKey, TElement>> _groupingsEnumerator;

    public InternalGroupBy(TEnumerator source, Func<TElement, TKey> selector, IEqualityComparer<TKey>? comparer = null)
    {
        _source = source;
        _selector = selector;
        _comparer = comparer ?? EqualityComparer<TKey>.Default;
        _lookup = Base.Lookup<TKey, TElement>.Create(_source, _selector, _comparer);
        var groupings = LookupToArray(ref _lookup);
        //Because we need to move all groupings to one enumeration and use a concrete solution
        _groupingsEnumerator = new ArrayStructEnumerator<Grouping<TKey, TElement>>(groupings);
    }

    public void Dispose()
    {
        _source.Dispose();
        _lookup.Dispose();
        //Because we need to return all arrays
        Grouping<TKey, TElement> grouping = default;
        while (_groupingsEnumerator.Next(ref grouping))
        {
            grouping.Dispose();
        }

        _groupingsEnumerator.Dispose();
    }

    public bool Next(ref Grouping<TKey, TElement> current)
    {
        return _groupingsEnumerator.Next(ref current);
    }

    private Grouping<TKey, TElement>[] LookupToArray(ref Base.Lookup<TKey, TElement> lookup)
    {
        // Because we need to collect all groupings from the lookup into an array
        if (lookup.Count == 0)
        {
            return Array.Empty<Grouping<TKey, TElement>>();
        }

        // Because we minimize allocations by using ArrayPool with a known count
        int count = lookup.Count;
        var groupings = new Grouping<TKey, TElement>[count];
        int arrayIndex = 0;

        // Because entries in the lookup are stored with 1-based indexing
        for (int i = 1; i <= count; i++)
        {
            var grouping = lookup.GetGroupingByIndex(i);
            if (grouping.HasValue)
            {
                groupings[arrayIndex++] = grouping.Value;
            }
        }

        // Because we may have skipped empty entries, resize if needed
        if (arrayIndex < count)
        {
            Array.Resize(ref groupings, arrayIndex);
        }

        return groupings;
    }

    public bool GetCountToLeftEnumerate(out int count)
    {
        // Because we cannot know the group count without building the entire lookup
        if (_groupingsEnumerator.GetCountToLeftEnumerate(out int enumerateCount))
        {
            count = enumerateCount;
            return true;
        }

        count = 0;
        return false;
    }

    public bool GetUnderlying(ref ReadOnlySpan<Grouping<TKey, TElement>> span)
    {
        // Because we delegate to the array enumerator after initialization
        if (_groupingsEnumerator.GetUnderlying(ref span))
        {
            return true;
        }

        span = default;
        return false;
    }

    public bool TryCopy(ref Span<Grouping<TKey, TElement>> destination, int offset)
    {
        if (_groupingsEnumerator.TryCopy(ref destination, offset))
        {
            return true;
        }

        return false;
    }
}