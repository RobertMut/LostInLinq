using System.Runtime.CompilerServices;
using PerformanceBenchmarks.Core.Abstractions;
using PerformanceBenchmarks.Core.Base;
using ZLinq.Linq;

namespace PerformanceBenchmarks.Core.Struct;

public static class WhereStruct
{
    public static StructEnumerable<InternalWhereArray<T>, T> StructWhere<T>(
        this T[] array,
        Func<T, bool> filter)
    {
        return new(new InternalWhereArray<T>(new ArrayStructEnumerator<T>(array), filter));
    }

    public static StructEnumerable<InternalWhere<TEnumerator, T>, T> StructWhere<TEnumerator, T>(
        this StructEnumerable<TEnumerator, T> source,
        Func<T, bool> filter) where TEnumerator : struct, IStructEnumerator<T>, allows ref struct
    {
        return new(new InternalWhere<TEnumerator, T>(source.Enumerator, filter));
    }

    public static StructEnumerable<InternalWhereSelect<TEnumerator, TIn, TOut>, TOut> StructWhere<
        TEnumerator, TIn, TOut>(
        this StructEnumerable<InternalSelect<TEnumerator, TIn, TOut>, TOut> source,
        Func<TOut, bool> filter)
        where TEnumerator : struct, IStructEnumerator<TIn>, allows ref struct
    {
        return new(new InternalWhereSelect<TEnumerator, TIn, TOut>(source.Enumerator._source, source.Enumerator.Selector, filter));
    }
}

public ref struct InternalWhere<TEnumerator, TIn> : IStructEnumerator<TIn>
    where TEnumerator : struct, IStructEnumerator<TIn>, allows ref struct
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
        while (_source.Next(ref fromSource))
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
        count = 0;
        return false;
    }

    public bool GetUnderlying(ref ReadOnlySpan<TIn> span)
    {
        span = default;

        return false;
    }

    public bool TryCopy(ref Span<TIn> destination, int offset) => false;
}

//concrete - array
public ref struct InternalWhereArray<TIn> : IStructEnumerator<TIn>
{
    private ArrayStructEnumerator<TIn> _source;
    private Span<TIn> _span;
    internal readonly Func<TIn, bool> _filter;

    public InternalWhereArray(ArrayStructEnumerator<TIn> source, Func<TIn, bool> filter)
    {
        _source = source;
        _span = source.Array.AsSpan();
        _filter = filter;
    }

    public void Dispose()
    {
        _source.Dispose();
    }

    public bool Next(ref TIn current)
    {
        while (_source.Index < _source.Count - 1)
        {
            var item = _span[++_source.Index];
            if (_filter(item))
            {
                current = item;

                return true;
            }
        }

        return false;
    }

    public bool GetCountToLeftEnumerate(out int count)
    {
        count = 0;
        return false;
    }

    public bool GetUnderlying(ref ReadOnlySpan<TIn> span)
    {
        return false;
    }

    public bool TryCopy(ref Span<TIn> destination, int offset) => false;
}

//generic
public ref struct InternalWhereSelect<TEnumerator, TIn, TOut> : IStructEnumerator<TOut>
    where TEnumerator : struct, IStructEnumerator<TIn>, allows ref struct
{
    private TEnumerator _source;
    private readonly Func<TIn, TOut> _selector;
    private readonly Func<TOut, bool> _filter;

    public InternalWhereSelect(TEnumerator source, Func<TIn, TOut> selector, Func<TOut, bool> filter)
    {
        _source = source;
        _selector = selector;
        _filter = filter;
    }

    public void Dispose()
    {
        _source.Dispose();
    }

    public bool Next(ref TOut current)
    {
        TIn source = default(TIn);
        while (_source.Next(ref source))
        {
            TOut selected = _selector(source);
            if (_filter(selected))
            {
                current = selected;

                return true;
            }
        }

        return false;
    }

    public bool GetCountToLeftEnumerate(out int count)
    {
        Unsafe.SkipInit(out count);
        return false;
    }

    public bool GetUnderlying(ref ReadOnlySpan<TOut> span)
    {
        return false;
    }

    public bool TryCopy(ref Span<TOut> destination, int offset)
    {
        return false;
    }
}