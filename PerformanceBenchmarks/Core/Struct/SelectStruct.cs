using PerformanceBenchmarks.Core.Abstractions;
using PerformanceBenchmarks.Core.Base;

namespace PerformanceBenchmarks.Core.Struct;

public static class SelectStruct
{
    public static IStructEnumerator<TOut> StructSelect<TIn, TOut>(this TIn[] array, Func<TIn, TOut> selector)
    {
        return new ArrayStructEnumerator<TIn>(array).InternalStructSelectArray(selector);
    }

    public static InternalWhereSelectArray<TIn, TOut> StructSelect<TIn, TOut>(
        this InternalWhereArray<TIn> source, Func<TIn, TOut> selector) =>
        new(source._source._array, source._filter, selector);

    private static IStructEnumerator<TOut> InternalStructSelectArray<TEnumerator, TIn, TOut>(
        this TEnumerator source, Func<TIn, TOut> selector)
        where TEnumerator : IStructEnumerator<TIn> => new InternalSelect<TEnumerator, TIn, TOut>(source, selector);
}

public struct InternalSelect<TEnumerator, TIn, TOut> : IStructEnumerator<TOut>
    where TEnumerator : IStructEnumerator<TIn>
{
    private TEnumerator _source;
    private readonly Func<TIn, TOut> _selector;

    public InternalSelect(TEnumerator source, Func<TIn, TOut> selector)
    {
        _source = source;
        _selector = selector;
    }

    public void Dispose()
    {
        _source.Dispose(); 
    }

    public bool Next(ref TOut current)
    {
        TIn? fromSource = default(TIn);
        if(_source.Next(ref fromSource))
        {
            current = _selector(fromSource);
            return true;
        }
        
        current = default(TOut);
        return false;
    }

    public bool GetCountToLeftEnumerate(out int count)
    {
        if (_source.GetCountToLeftEnumerate(out int sourceCount))
        {
            count = sourceCount;

            return true;
        }
        
        count = 0;
        return false;
    }

    public bool GetUnderlying(ref ReadOnlySpan<TOut> span)
    {
        span = default;

        return false;
    }

    public bool TryCopy(ref Span<TOut> destination, int offset) => false;
}