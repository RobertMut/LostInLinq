using PerformanceBenchmarks.Core.Abstractions;

namespace PerformanceBenchmarks.Core.Base;

public ref struct ArrayStructEnumerator<T> : IStructEnumerator<T>
{
    private readonly T[] _array;
    private int _index;
    private readonly int _count;

    public ArrayStructEnumerator(T[] array)
    {
        _array = array ?? throw new ArgumentNullException(nameof(array));
        _count = array.Length;
        _index = -1;
    }
    
    public void Dispose()
    {
        _index = -1;
    }

    public bool Next(ref T current)
    {
        if (_index < _count - 1)
        {
            current = _array[++_index];
            return true;
        }

        current = default(T);
        return false;
    }

    public bool GetCountToLeftEnumerate(out int count)
    {
        if (_index < 0)
        {
            count = _count;

            return true;
        }
        
        if(_index >= _count)
        {
            count = 0;
            return false;
        }
        
        count = _count - _index - 1;
        return true;
    }

    public bool GetUnderlying(ref ReadOnlySpan<T> span)
    {
        if(_array == null || _count == 0)
        {
            span = default(ReadOnlySpan<T>);
            return false;
        }
        
        span = _array.AsSpan();

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