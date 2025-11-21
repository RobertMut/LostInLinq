using System.Runtime.CompilerServices;
using PerformanceBenchmarks.Core.Abstractions;
using PerformanceBenchmarks.Core.Base;

namespace PerformanceBenchmarks.Core.Struct;

public static class SelectStruct
{ 
    public static StructEnumerable<InternalSelect<TEnumerator, TIn, TOut>, TOut> StructSelect<TEnumerator, TIn, TOut>(
        this StructEnumerable<TEnumerator, TIn> source,
        Func<TIn, TOut> selector)
    where TEnumerator : struct, IStructEnumerator<TIn>, allows ref struct
    {
        return new(new InternalSelect<TEnumerator, TIn, TOut>(source.Enumerator, selector));
    } 
    
    public static StructEnumerable<InternalSelectArray<TIn, TOut>, TOut> StructSelect<TIn, TOut>(
        this TIn[] array,
        Func<TIn, TOut> selector)
    {
        return new(new InternalSelectArray<TIn, TOut>(new ArrayStructEnumerator<TIn>(array), selector));
    }
}

public ref struct InternalSelect<TEnumerator, TIn, TOut> : IStructEnumerator<TOut>
where TEnumerator : struct, IStructEnumerator<TIn>, allows ref struct
{
    internal TEnumerator _source;
    internal readonly Func<TIn, TOut> Selector;

    public InternalSelect(TEnumerator source, Func<TIn, TOut> selector)
    {
        _source = source;
        Selector = selector;
    }

    public void Dispose()
    {
        _source.Dispose();
    }

    public bool Next(ref TOut current)
    {
        TIn? fromSource = default(TIn);
        if (_source.Next(ref fromSource))
        {
            current = Selector(fromSource);
            return true;
        }
        
        Unsafe.SkipInit(out current);
        return false;
    }

    public bool GetCountToLeftEnumerate(out int count)
    {
        if (_source.GetCountToLeftEnumerate(out int sourceCount))
        {
            count = sourceCount;

            return true;
        }

        Unsafe.SkipInit(out count);
        return false;
    }

    public bool GetUnderlying(ref ReadOnlySpan<TOut> span)
    {
        Unsafe.SkipInit(out span);
        return false;
    }

    public bool TryCopy(ref Span<TOut> destination, int offset) => false;
}

public ref struct InternalSelectArray<TIn, TOut> : IStructEnumerator<TOut>
{
    private ArrayStructEnumerator<TIn> _source;
    private Span<TIn> _span;
    private readonly Func<TIn, TOut> _selector;

    public InternalSelectArray(ArrayStructEnumerator<TIn> source, Func<TIn, TOut> selector)
    {
        _source = source;
        _span = source.Array.AsSpan();
        _selector = selector;
    }

    public void Dispose()
    {
        _source.Dispose();
    }

    public bool Next(ref TOut current)
    {
        if (_source.Index < _source.Count - 1)
        {
            current = _selector(_span[++_source.Index]);
            return true;
        }

        return false;
    }

    public bool GetCountToLeftEnumerate(out int count)
    {
        if (_source.GetCountToLeftEnumerate(out int sourceCount))
        {
            count = sourceCount;

            return true;
        }

        Unsafe.SkipInit(out count);
        return false;
    }

    public bool GetUnderlying(ref ReadOnlySpan<TOut> span)
    {
        return false;
    }

    public bool TryCopy(ref Span<TOut> destination, int offset) => false;
}
