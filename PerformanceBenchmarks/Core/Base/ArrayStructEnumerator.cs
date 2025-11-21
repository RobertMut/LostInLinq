using PerformanceBenchmarks.Core.Abstractions;

namespace PerformanceBenchmarks.Core.Base;

public ref struct ArrayStructEnumerator<T> : IStructEnumerator<T>
{
    internal readonly T[] Array;
    internal int Index;
    internal readonly int Count;

    public ArrayStructEnumerator(T[] array)
    {
        Array = array ?? throw new ArgumentNullException(nameof(array));
        Count = array.Length;
        Index = -1;
    }
    
    public void Dispose()
    {
        Index = -1;
    }

    public bool Next(ref T current)
    {
        if (Index < Count - 1)
        {
            current = Array[++Index];
            return true;
        }

        current = default(T);
        return false;
    }

    public bool GetCountToLeftEnumerate(out int count)
    {
        if (Index < 0)
        {
            count = Count;

            return true;
        }
        
        if(Index >= Count)
        {
            count = 0;
            return false;
        }
        
        count = Count - Index - 1;
        return true;
    }

    public bool GetUnderlying(ref ReadOnlySpan<T> span)
    {
        if(Array == null || Count == 0)
        {
            span = default(ReadOnlySpan<T>);
            return false;
        }
        
        span = Array.AsSpan();

        return true;
    }

    public bool TryCopy(ref Span<T> destination, int offset)
    {
        ReadOnlySpan<T> source = default;
        if (GetUnderlying(ref source))
        {
            if (offset < 0 || offset >= source.Length)
            {
                return false;
            }
            ReadOnlySpan<T> slice = source.Slice(offset);

            for (int i = 0; i < slice.Length; i++)
            {
                destination[i] = slice[i];
            }

            return true;
        }

        return false;
    }
}