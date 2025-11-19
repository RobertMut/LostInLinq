using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using PerformanceBenchmarks.Core.Abstractions;

namespace PerformanceBenchmarks.Core.Base;

[DebuggerDisplay("Count = {Count} LastGroupCount = {LastGroup._count}")]
public ref struct Lookup<TKey, TElement> : IDisposable
{
    private const double LoadFactor = 0.75;
    private const int InitialCapacity = 16;
    private const int InitialElementCapacity = 4;

    private Grouping<TKey, TElement>[] _entries;

    private int[] _buckets;
    private readonly IEqualityComparer<TKey> _comparer;
    private int _count;
    private int _threshold;
    private int _lastGroupIndex;
    private uint _bucketMask;

    private Lookup(IEqualityComparer<TKey>? comparer = null)
    {
        _comparer = comparer ?? EqualityComparer<TKey>.Default;
        _bucketMask = InitialCapacity - 1;
        _threshold = (int)(InitialCapacity * LoadFactor);
        _buckets = ArrayPool<int>.Shared.Rent(InitialCapacity);
        _entries = ArrayPool<Grouping<TKey, TElement>>.Shared.Rent(InitialCapacity);
        _buckets.AsSpan(0, InitialCapacity).Clear();

        _count = 0;
        _lastGroupIndex = 0;
    }

    public static Lookup<TKey, TElement> Create<TEnumerator>(TEnumerator source, Func<TElement, TKey> selector,
        IEqualityComparer<TKey>? comparer = null) where TEnumerator : IStructEnumerator<TElement>, allows ref struct
    {
        var lookup = new Lookup<TKey, TElement>(comparer);

        ReadOnlySpan<TElement> span = default;
        if (source.GetUnderlying(ref span))
        {
            foreach (var element in span)
            {
                var key = selector(element);
                if (key != null)
                {
                    lookup.Add(key, element);
                }
            }
        }
        else
        {
            TElement element = default;
            while (source.Next(ref element))
            {
                var key = selector(element);
                if (key != null)
                {
                    lookup.Add(key, element);
                }
            }
        }

        return lookup;
    }
    
    public int Count => _count;

    public readonly ref readonly Grouping<TKey, TElement> GetGroupingByIndex(int index)
    {
        if (index < 1 || index > _count)
        {
            throw new ArgumentOutOfRangeException();
        }

        ref Grouping<TKey, TElement> entry = ref _entries[index - 1];

        return ref entry;
    }

    public LookupIterator GetIterator()
    {
        return new LookupIterator(_entries.AsSpan(0, _count));
    }

    public void Add(TKey key, TElement value)
    {
        if (_count >= _threshold)
        {
            //Because we need to resize when threshold hit
            Resize();
        }

        uint hashCode = InternalGetHashCode(ref key);
        var bucketIdx = GetIndex(ref hashCode);
        int currentEntry = _buckets[bucketIdx];

        //Because we need to check if we already have the key in the bucket
        while (currentEntry > 0)
        {
            ref Grouping<TKey, TElement> entry = ref _entries[currentEntry - 1];

            //In hashset we would rather stop on the first match and notify the caller
            if (entry.HashCode == hashCode && _comparer.Equals(entry.Key, key))
            {
                AddToGroup(ref entry, value); 
                return;
            }

            currentEntry = entry.HashNext;
        }

        //Because we checked that there is no matching key
        CreateGroup(key, hashCode, value, bucketIdx);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddToGroup(ref Grouping<TKey, TElement> entry, TElement value)
    {
        //Because every Key has its own group, and we want to resize it if needed
        if (entry.ElementCount >= entry.ElementCapacity)
        {
            int newCapacity = entry.ElementCapacity == 0 ? InitialElementCapacity : entry.ElementCapacity * 2;
            var newArray = ArrayPool<TElement>.Shared.Rent(newCapacity);

            if (entry.Elements != null)
            {
                entry.Elements.AsSpan(0, entry.ElementCount).CopyTo(newArray);
                ArrayPool<TElement>.Shared.Return(entry.Elements,
                    clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<TElement>());
            }

            entry.Elements = newArray;
        }

        //Because element count changed
        entry.Elements[entry.ElementCount++] = value;
    }

    private void CreateGroup(TKey key, uint hashCode, TElement value, int bucketIndex)
    {
        if (_count >= _entries.Length)
        {
            Resize();

            //Because we resized the array, we need to get the new bucket index
            bucketIndex = GetIndex(ref hashCode);
        }

        int newIndex = _count + 1;
        ref Grouping<TKey, TElement> newEntry = ref _entries[_count];

        newEntry.HashCode = hashCode;
        newEntry.Key = key;
        newEntry.HashNext = _buckets[bucketIndex];
        //Because arrays can be huge
        newEntry.Elements = ArrayPool<TElement>.Shared.Rent(InitialElementCapacity);
        newEntry.Elements[0] = value;
        newEntry.ElementCount = 1;

        _buckets[bucketIndex] = newIndex;

        if (_lastGroupIndex == 0)
        {
            newEntry.Next = newIndex;
            _lastGroupIndex = newIndex;
        }
        else
        {
            ref Grouping<TKey, TElement> lastGroup = ref _entries[_lastGroupIndex - 1];
            newEntry.Next = lastGroup.Next;
            lastGroup.Next = newIndex;
            _lastGroupIndex = newIndex;
        }

        //Because we added a new entry
        _count++;
    }

    //The same as we used in MinimalHashSet
    private void Resize()
    {
        uint newSize = System.Numerics.BitOperations.RoundUpToPowerOf2((uint)_entries.Length * 2);
        var newBucket = ArrayPool<int>.Shared.Rent((int)newSize);
        var newEntries = ArrayPool<Grouping<TKey, TElement>>.Shared.Rent((int)newSize);
        newBucket.AsSpan(0, (int)newSize).Clear();

        _entries.AsSpan(0, _count).CopyTo(newEntries);

        _bucketMask = newSize - 1;
        
        for (int i = 0; i < _count; i++)
        {
            ref Grouping<TKey, TElement> entry = ref newEntries[i];
            int bucketIndex = GetIndex(ref entry.HashCode);
            entry.HashNext = newBucket[bucketIndex];
            newBucket[bucketIndex] = i + 1;
        }

        ArrayPool<int>.Shared.Return(_buckets, clearArray: true);
        ArrayPool<Grouping<TKey, TElement>>.Shared.Return(_entries, clearArray: true);
        _threshold = (int)(newSize * LoadFactor);

        _buckets = newBucket;
        _entries = newEntries;
    }

    private uint InternalGetHashCode(ref TKey item)
    {
        return (uint)(_comparer.GetHashCode(item) & 0x7FFFFFFF);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetIndex(ref uint hashCode)
    {
        return (int)(hashCode & _bucketMask);
    }

    public readonly void Dispose()
    {
        if (_buckets != null)
        {
            ArrayPool<int>.Shared.Return(_buckets);
            ArrayPool<Grouping<TKey, TElement>>.Shared.Return(_entries);
        }
    }
    
    public ref struct LookupIterator
    {
        private int _index;
        private readonly int _count;
        private readonly ReadOnlySpan<Grouping<TKey, TElement>> _entries;

        public LookupIterator(ReadOnlySpan<Grouping<TKey, TElement>> groups)
        {
            _entries = groups;
            _count = groups.Length;
            _index = 0;
        }

        public bool Next()
        {
            while (++_index <= _count)
            {
                ref readonly Grouping<TKey, TElement> entry = ref _entries[_index - 1];
                if (entry.Elements != null && entry.ElementCount > 0)
                {
                    return true;
                }

            }
            return false;
        }

        public ReadOnlyGrouping<TKey, TElement> Current
        {
            get
            {
                if(_index < 0 || _index > _count)
                {
                    throw new InvalidOperationException();
                }

                ref readonly var entry = ref _entries[_index - 1];
                return new ReadOnlyGrouping<TKey, TElement>(in entry);
            }
        }

        public int Remaining => _count - _index;
    }
}