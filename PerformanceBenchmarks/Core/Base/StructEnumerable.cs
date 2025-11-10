using PerformanceBenchmarks.Core.Abstractions;

namespace PerformanceBenchmarks.Core.Base;

public ref struct StructEnumerable<TEnumerator, T>(TEnumerator enumerator)
where TEnumerator : struct, IStructEnumerator<T>, allows ref struct
{
    public TEnumerator Enumerator = enumerator;
}