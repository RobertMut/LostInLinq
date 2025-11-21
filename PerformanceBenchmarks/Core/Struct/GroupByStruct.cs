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