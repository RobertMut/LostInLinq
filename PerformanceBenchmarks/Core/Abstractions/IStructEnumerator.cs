namespace PerformanceBenchmarks.Core.Abstractions;

public interface IStructEnumerator<T> : IDisposable 
{
    bool Next(ref T current);

    bool GetCountToLeftEnumerate(out int count);

    bool GetUnderlying(ref ReadOnlySpan<T> span);

    bool TryCopy(ref Span<T> destination, int offset);
}