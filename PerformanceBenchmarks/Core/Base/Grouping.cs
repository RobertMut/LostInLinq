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

    //Because, we're renting a larger array
    public ReadOnlySpan<TElement> AsSpan() => Elements.AsSpan(0, ElementCount);

    public void Dispose()
    {
        ArrayPool<TElement>.Shared.Return(Elements);
    }
}