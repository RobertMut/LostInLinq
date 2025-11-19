using PerformanceBenchmarks.Core.Abstractions;
using PerformanceBenchmarks.Core.Base;

namespace PerformanceBenchmarks.Core.Struct;

public static class GroupByStruct
{
    public static StructEnumerable<InternalGroupBy<TEnumerator, TKey, TElement>, ReadOnlyGrouping<TKey, TElement>>
        StructGroupBy<TEnumerator, TElement, TKey>(
            this StructEnumerable<TEnumerator, TElement> source,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey>? comparer = null)
        where TEnumerator : struct, IStructEnumerator<TElement>, allows ref struct
    {
        return new(new InternalGroupBy<TEnumerator, TKey, TElement>(source.Enumerator, keySelector, comparer));
    }

    public static StructEnumerable<InternalGroupBy<ArrayStructEnumerator<TElement>, TKey, TElement>,
            ReadOnlyGrouping<TKey, TElement>>
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

public ref struct InternalGroupBy<TEnumerator, TKey, TElement> : IStructEnumerator<ReadOnlyGrouping<TKey, TElement>>
    where TEnumerator : struct, IStructEnumerator<TElement>, allows ref struct
{
    private TEnumerator _source;
    private readonly Func<TElement, TKey> _selector;
    private readonly IEqualityComparer<TKey> _comparer;
    private Base.Lookup<TKey, TElement> _lookup;
    private Base.Lookup<TKey,TElement>.LookupIterator _lookupIterator;
    private bool _initialized;

    public InternalGroupBy(TEnumerator source, Func<TElement, TKey> selector, IEqualityComparer<TKey>? comparer = null)
    {
        _source = source;
        _selector = selector;
        _comparer = comparer ?? EqualityComparer<TKey>.Default;
        _lookup = default;
        _initialized = false;
    }
    
    public void Dispose()
    {
        _source.Dispose();
        _lookup.Dispose();
    }

    public bool Next(ref ReadOnlyGrouping<TKey, TElement> current)
    {
        Initialize();
        if (_lookupIterator.Next())
        {
            current = _lookupIterator.Current;
            return true;
        }
        
        current = default;
        return false;
    }

    public bool GetCountToLeftEnumerate(out int count)
    {
        Initialize();
        count = _lookupIterator.Remaining;

        return true;
    }

    public bool GetUnderlying(ref ReadOnlySpan<ReadOnlyGrouping<TKey, TElement>> span)
    {
        span = default;
        return false;
    }

    public bool TryCopy(ref Span<ReadOnlyGrouping<TKey, TElement>> destination, int offset)
    {
        return false;
    }

    private ReadOnlyGrouping<TKey, TElement>[] LookupToArray(ref Base.Lookup<TKey, TElement> lookup)
    {
        // Because we need to collect all groupings from the lookup into an array
        if (lookup.Count == 0)
        {
            return Array.Empty<ReadOnlyGrouping<TKey, TElement>>();
        }

        // Because we minimize allocations by using a pre-allocated array with known count
        int count = lookup.Count;
        var groupings = new ReadOnlyGrouping<TKey, TElement>[count];
        int arrayIndex = 0;

        // Because entries in the lookup are stored with 1-based indexing
        for (int i = 1; i <= count; i++)
        {
            var grouping = lookup.GetGroupingByIndex(i);
            groupings[arrayIndex++] = new ReadOnlyGrouping<TKey, TElement>(in grouping);
        }

        // Because we may have skipped empty entries, resize if needed
        if (arrayIndex < count)
        {
            Array.Resize(ref groupings, arrayIndex);
        }

        return groupings;
    }

    private void Initialize()
    {
        if (!_initialized)
        {
            _lookup = Base.Lookup<TKey, TElement>.Create(_source, _selector, _comparer);
            _lookupIterator = _lookup.GetIterator();
            _initialized = true;
        }
    }
}