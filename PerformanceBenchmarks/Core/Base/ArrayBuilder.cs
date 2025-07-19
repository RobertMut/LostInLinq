using System.Buffers;
using System.Runtime.CompilerServices;

namespace PerformanceBenchmarks.Core.Base;

public ref struct ArrayBuilder<T> 
{
    private const int DefaultCapacity = 5;
    private T[] _buffer;
    private SmallBuffer<T[]> _buffers;
    private int _count;
    private int _buffersIndex;
    private int _capacity;
    
    public ArrayBuilder()
    {
        _buffers = new SmallBuffer<T[]>();
        _count = 0;
        _buffersIndex = 0;
    }
    
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public T[] ToArray()
    {
        var result = GC.AllocateUninitializedArray<T>(_capacity);
        var resultSpan = result.AsSpan();

        int nextIdx = 0;
        for (int i = 0; i < _buffersIndex; i++)
        {
            Span<T> buffer = _buffers[i].AsSpan();
            buffer.CopyTo(resultSpan.Slice(nextIdx, buffer.Length));
            
            nextIdx += buffer.Length;
        }

        _buffer.AsSpan().Slice(0, _capacity - nextIdx).CopyTo(resultSpan.Slice(nextIdx, _capacity - nextIdx));

        return result;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Add(T item)
    {
        if (_buffer == null || _count >= _buffer.Length)
        {
            Resize(_count + 1);
        }

        _buffer[_count++] = item;
        _capacity++;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Clear()
    {
        if (_buffer != null)
        {
            ArrayPool<T>.Shared.Return(_buffer, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }

        for (int i = 0; i < _buffersIndex; i++)
        {
            ArrayPool<T>.Shared.Return(_buffers[i], RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
        
        _count = 0;
        _buffer = null;
        _buffers = new SmallBuffer<T[]>();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void Resize(int minimum)
    {
        int newCapacity = Math.Max((_buffer?.Length ?? 0) == 0 ? DefaultCapacity : _buffer.Length * 2, minimum);

        var newArr = ArrayPool<T>.Shared.Rent(newCapacity);
        
        if (_buffer != null)
        {
            _buffers[_buffersIndex++] = _buffer;
        }

        _buffer = newArr;
        _count = 0;
    }
}

public ref struct SmallBuffer<T> //approach found here: https://www.jacksondunstan.com/articles/5051
{
    private T _item1;
    private T _item2;
    private T _item3;
    private T _item4;
    private T _item5;
    private T _item6;
    private T _item7;
    private T _item8;
    private T _item9;
    private T _item10;
    private T _item11;
    private T _item12;
    private T _item13;

    public T this[int index]
    {
        get
        {
            return index switch
            {
                0 => _item1,
                1 => _item2,
                2 => _item3,
                3 => _item4,
                4 => _item5,
                5 => _item6,
                6 => _item7,
                7 => _item8,
                8 => _item9,
                9 => _item10,
                10 => _item11,
                11 => _item12,
                12 => _item13,
                _ => throw new IndexOutOfRangeException()
            };
        }
        set
        {
            switch (index)
            {
                case 0: _item1 = value; break;
                case 1: _item2 = value; break;
                case 2: _item3 = value; break;
                case 3: _item4 = value; break;
                case 4: _item5 = value; break;
                case 5: _item6 = value; break;
                case 6: _item7 = value; break;
                case 7: _item8 = value; break;
                case 8: _item9 = value; break;
                case 9: _item10 = value; break;
                case 10: _item11 = value; break;
                case 11: _item12 = value; break;
                case 12: _item13 = value; break;
            }
        }
    }
}