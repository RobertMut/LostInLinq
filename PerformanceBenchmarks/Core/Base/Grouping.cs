using System.Buffers;

namespace PerformanceBenchmarks.Core.Base;

public struct Grouping<TKey, TElement> : IDisposable
{
    public uint HashCode;

    public TKey Key;

    public int HashNext;

    public int Next;

    public TElement[]? Elements;

    public int ElementCount;

    public int ElementCapacity => Elements.Length;

    public TElement this[int index] =>
        index > Elements.Length - 1 ? throw new IndexOutOfRangeException() : Elements[index];

    public void Dispose()
    {
        ArrayPool<TElement>.Shared.Return(Elements);
    }
}

public readonly struct ReadOnlyGrouping<TKey, TElement>
{
    private readonly Grouping<TKey, TElement> _grouping;

    public ReadOnlyGrouping(in Grouping<TKey, TElement> grouping)
    {
        _grouping = grouping;
    }

    public TKey Key => _grouping.Key;
    public int Count => _grouping.ElementCount;

    public ReadOnlySpan<TElement> Elements => _grouping.Elements != null
        ? _grouping.Elements.AsSpan(0, _grouping.ElementCount)
        : ReadOnlySpan<TElement>.Empty;
}