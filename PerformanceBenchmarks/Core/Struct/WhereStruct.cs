using PerformanceBenchmarks.Core.Abstractions;
using PerformanceBenchmarks.Core.Base;

namespace PerformanceBenchmarks.Core.Struct;

public static class WhereStruct
{
    public static InternalWhere<StructEnumerator<TIn>, TIn> StructWhere<TIn>(this TIn[] array, Func<TIn, bool> filter)
    {
        return new StructEnumerator<TIn>(array).StructWhere(filter);
    }

    private static InternalWhere<TEnumerable, TIn> StructWhere<TIn, TEnumerable>(this TEnumerable source, Func<TIn, bool> filter)
        where TEnumerable : IStructEnumerator<TIn>
    {
        return new InternalWhere<TEnumerable, TIn>(source, filter);
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

public struct InternalWhereSelect<TEnumerator, TIn, TOut> : IStructEnumerator<TOut>
    where TEnumerator : IStructEnumerator<TIn>
{
    internal TEnumerator _source;
    internal readonly Func<TIn, bool> _filter;
    internal readonly Func<TIn, TOut> _selector;
    
    public InternalWhereSelect(TEnumerator source, Func<TIn, bool> filter, Func<TIn, TOut> selector)
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