using PerformanceBenchmarks.Core.Abstractions;
using PerformanceBenchmarks.Core.Base;

namespace PerformanceBenchmarks.Core.Struct;

public static class WhereStruct
{
    public static InternalWhereArray<TIn> StructWhere<TIn>(this TIn[] arr,
        Func<TIn, bool> filter)
    {
        return new InternalWhereArray<TIn>(new ArrayStructEnumerator<TIn>(arr), filter);
    }

    public static InternalWhere<IStructEnumerator<TIn>, TIn> StructWhere<TIn>(this IStructEnumerator<TIn> source, Func<TIn, bool> filter)
    {
        return new InternalWhere<IStructEnumerator<TIn>, TIn>(source, filter);
    }
}

public struct InternalWhere<TEnumerator, TIn> : IStructEnumerator<TIn>
    where TEnumerator : IStructEnumerator<TIn>
{
    internal TEnumerator _source;
    internal readonly Func<TIn, bool> _filter;

    public InternalWhere(TEnumerator source, Func<TIn, bool> filter)
    {
        _source = source;
        _filter = filter;
    }

    public void Dispose()
    {
        _source.Dispose(); 
    }

    public bool Next(ref TIn current)
    {
        TIn? fromSource = default(TIn);
        while(_source.Next(ref fromSource))
        {
            if (_filter(fromSource))
            {
                current = fromSource;
            
                return true;
            }
        }
        
        return false;
    }

    public bool GetCountToLeftEnumerate(out int count)
    {
        bool val = _source.GetCountToLeftEnumerate(out int enumeratorCount);
        
        count = enumeratorCount;
        return val;
    }

    public bool GetUnderlying(ref ReadOnlySpan<TIn> span)
    {
        span = default;

        return false;
    }

    public bool TryCopy(ref Span<TIn> destination, int offset) => false;
}

//concrete - array
public struct InternalWhereArray<TIn> : IStructEnumerator<TIn>
{
    internal ArrayStructEnumerator<TIn> _source;
    internal readonly Func<TIn, bool> _filter;

    public InternalWhereArray(ArrayStructEnumerator<TIn> source, Func<TIn, bool> filter)
    {
        _source = source;
        _filter = filter;
    }

    public void Dispose()
    {
        _source.Dispose(); 
    }

    public bool Next(ref TIn current)
    {
        TIn? fromSource = default(TIn);
        while(_source.Next(ref fromSource))
        {
            if (_filter(fromSource))
            {
                current = fromSource;
            
                return true;
            }
        }
        
        return false;
    }

    public bool GetCountToLeftEnumerate(out int count)
    {
        bool val = _source.GetCountToLeftEnumerate(out int enumeratorCount);
        
        count = enumeratorCount;
        return val;
    }

    public bool GetUnderlying(ref ReadOnlySpan<TIn> span)
    {
        span = default;

        return false;
    }

    public bool TryCopy(ref Span<TIn> destination, int offset) => false;
}

//generic
public struct InternalWhereSelect<TEnumerator, TIn, TOut> : IStructEnumerator<TOut>
    where TEnumerator : IStructEnumerator<TIn>
{
    internal TEnumerator _source;
    internal readonly Func<TIn, bool> _filter;
    internal readonly Func<TIn, TOut> _selector;
    
    public InternalWhereSelect(ref TEnumerator source, Func<TIn, bool> filter, Func<TIn, TOut> selector)
    {
        _source = source;
        _filter = filter;
        _selector = selector;
    }

    public void Dispose()
    {
        _source.Dispose(); 
    }

    public bool Next(ref TOut current)
    {
        TIn source = default(TIn);
        while(_source.Next(ref source))
        {
            if (_filter(source))
            {
                current = _selector(source);

                return true;
            }
        }
        
        return false;
    }

    public bool GetCountToLeftEnumerate(out int count)
    {
        bool val = _source.GetCountToLeftEnumerate(out int enumeratorCount);
        
        count = enumeratorCount;
        return val;
    }

    public bool GetUnderlying(ref ReadOnlySpan<TOut> span)
    {
        span = default;

        return false;
    }

    public bool TryCopy(ref Span<TOut> destination, int offset)
    {
        return false;
    }
}

//concrete - array
public struct InternalWhereSelectArray<TIn, TOut> : IStructEnumerator<TOut>
{
    private readonly TIn[] _source;
    private int _index;
    private readonly int _count;
    private readonly Func<TIn, bool> _filter;
    private readonly Func<TIn, TOut> _selector;
    
    public InternalWhereSelectArray(TIn[] source, Func<TIn, bool> filter, Func<TIn, TOut> selector)
    {
        _source = source;
        _filter = filter;
        _selector = selector;
        _index = -1;
        _count = source.Length;
    }

    public void Dispose()
    {
        _index = -1;
    }

    public bool Next(ref TOut current)
    {
        TIn source = default(TIn);
        
        while(_index < _count - 1)
        {
            _index++;
            source = _source[_index];
            
            if (_filter(source))
            {
                current = _selector(source);
                
                return true;
            }
        }
        
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

    public bool GetUnderlying(ref ReadOnlySpan<TOut> span)
    {
        span = default;

        return false;
    }

    public bool TryCopy(ref Span<TOut> destination, int offset)
    {
        return false;
    }
}