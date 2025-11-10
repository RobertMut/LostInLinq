using PerformanceBenchmarks.Core.Base;
using PerformanceBenchmarks.Core.Struct;
using Shouldly;
using TestProject1;

namespace PerformanceBenchmarks.Tests;

[TestFixture]
[TestOf(typeof(GroupByStruct))]
public class InternalGroupByTest
{
    private TestData[] _testData;

    [SetUp]
    public void Setup()
    {
        _testData = TestDataGenerator.Generate(2000);
    }

    [Test]
    public void StructGroupBy_ShouldBe_TheSameAsLinq_And_EachKeyIsCorrectlyGrouped()
    {
        StructEnumerable<InternalGroupBy<ArrayStructEnumerator<TestData>, double, TestData>, Grouping<double, TestData>> myGroupBy =
            _testData.StructGroupBy(x => Math.Ceiling(x.Number * 100 % 3));
        Dictionary<double, List<TestData>> linqGroupBy = _testData.GroupBy(x => Math.Ceiling((x.Number * 100) % 3))
            .ToDictionary(x => x.Key, v => v.ToList());
        Dictionary<double, List<TestData>> dictionary = new Dictionary<double, List<TestData>>();
        Grouping<double, TestData> grouping = default;
        while (myGroupBy.Enumerator.Next(ref grouping))
        {
            if (!dictionary.TryAdd(grouping.Key, grouping.AsSpan().ToArray().ToList()))
            {
                dictionary[grouping.Key].AddRange(grouping.AsSpan());
            }
        }

        foreach (var kvp in linqGroupBy)
        {
            dictionary.ContainsKey(kvp.Key).ShouldBeTrue();
            kvp.Value.SequenceEqual(dictionary[kvp.Key]).ShouldBeTrue();
        }
    }

    [Test]
    public void StructGroupBy_Should_ProcessEmptyCollection()
    {
        var myGroupBy = new object[] { }.StructGroupBy(x => x).ToArray();
        myGroupBy.ShouldBeNull();
    }

    [Test]
    public void StructGroupBy_Should_ProcessWithMultipleEnumerators()
    {
        var grouping = _testData.StructSelect(x => new { Id = x.Guid, Message = x.Message })
            .StructWhere(x => x.Message.StartsWith("i")).StructGroupBy(x => x).ToArray();

        grouping.ShouldNotBeNull();
    }

    [Test]
    public void StructGroupBy_Should_GroupBySimpleKey()
    {
        var numbers = Enumerable.Range(1, 100).ToArray();
        var linqGroupBy = numbers.GroupBy(x => x % 5).ToDictionary(g => g.Key, g => g.ToList());

        var myGroupBy = numbers.StructGroupBy(x => x % 5);
        var dictionary = new Dictionary<int, List<int>>();
        Grouping<int, int> grouping = default;
        while (myGroupBy.Enumerator.Next(ref grouping))
        {
            dictionary[grouping.Key] = grouping.AsSpan().ToArray().ToList();
        }

        linqGroupBy.Keys.OrderBy(k => k).ShouldBe(dictionary.Keys.OrderBy(k => k));
        foreach (var key in linqGroupBy.Keys)
        {
            linqGroupBy[key].ShouldBeEquivalentTo(dictionary[key]);
        }
    }

    [Test]
    public void StructGroupBy_Should_HandleSingleGroup()
    {
        var numbers = new[] { 1, 1, 1, 1, 1 };
        var linqResult = numbers.GroupBy(x => x).ToArray();
        var structResult = numbers.StructGroupBy(x => x).ToArray();

        structResult.Length.ShouldBe(1);
        structResult[0].Key.ShouldBe(1);
        structResult[0].AsSpan().Length.ShouldBe(5);
    }

    [Test]
    public void StructGroupBy_Should_HandleAllUniqueKeys()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };
        var linqResult = numbers.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        var structResult = numbers.StructGroupBy(x => x);

        var dictionary = new Dictionary<int, int>();
        Grouping<int, int> grouping = default;
        while (structResult.Enumerator.Next(ref grouping))
        {
            dictionary[grouping.Key] = grouping.ElementCount;
        }

        dictionary.Count.ShouldBe(5);
        foreach (var key in linqResult.Keys)
        {
            dictionary[key].ShouldBe(1);
        }
    }

    [Test]
    public void StructGroupBy_Should_HandleStringKeys()
    {
        var words = new[] { "apple", "banana", "apricot", "blueberry", "avocado", "berry" };
        var linqGroupBy = words.GroupBy(x => x[0].ToString()).ToDictionary(g => g.Key, g => g.ToList());

        var myGroupBy = words.StructGroupBy(x => x[0].ToString());
        var dictionary = new Dictionary<string, List<string>>();
        Grouping<string, string> grouping = default;
        while (myGroupBy.Enumerator.Next(ref grouping))
        {
            dictionary[grouping.Key] = grouping.AsSpan().ToArray().ToList();
        }

        linqGroupBy.Keys.OrderBy(k => k).ShouldBe(dictionary.Keys.OrderBy(k => k));
        foreach (var key in linqGroupBy.Keys)
        {
            linqGroupBy[key].ShouldBeEquivalentTo(dictionary[key]);
        }
    }

    [Test]
    public void StructGroupBy_Should_HandleLargeNumberOfGroups()
    {
        var data = Enumerable.Range(0, 10000).ToArray();
        var linqGroupBy = data.GroupBy(x => x % 100).ToDictionary(g => g.Key, g => g.Count());

        var myGroupBy = data.StructGroupBy(x => x % 100);
        var dictionary = new Dictionary<int, int>();
        Grouping<int, int> grouping = default;
        while (myGroupBy.Enumerator.Next(ref grouping))
        {
            dictionary[grouping.Key] = grouping.ElementCount;
        }

        dictionary.Count.ShouldBe(100);
        foreach (var key in linqGroupBy.Keys)
        {
            dictionary[key].ShouldBe(linqGroupBy[key]);
        }
    }

    [Test]
    public void StructGroupBy_Should_HandleLargeGroups()
    {
        var data = Enumerable.Repeat(1, 10000).Concat(Enumerable.Repeat(2, 10000)).ToArray();
        var linqGroupBy = data.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        var myGroupBy = data.StructGroupBy(x => x);
        var dictionary = new Dictionary<int, int>();
        Grouping<int, int> grouping = default;
        while (myGroupBy.Enumerator.Next(ref grouping))
        {
            dictionary[grouping.Key] = grouping.ElementCount;
        }

        dictionary.Count.ShouldBe(2);
        dictionary[1].ShouldBe(10000);
        dictionary[2].ShouldBe(10000);
    }

    [Test]
    public void StructGroupBy_Should_HandleChainedOperations()
    {
        var numbers = Enumerable.Range(1, 1000).ToArray();
        var linqResult = numbers
            .Where(x => x % 2 == 0)
            .Select(x => x / 2)
            .GroupBy(x => x % 10)
            .ToDictionary(g => g.Key, g => g.ToList());

        var structGroupBy = numbers
            .StructWhere(x => x % 2 == 0)
            .StructSelect(x => x / 2)
            .StructGroupBy(x => x % 10);

        var dictionary = new Dictionary<int, List<int>>();
        Grouping<int, int> grouping = default;
        while (structGroupBy.Enumerator.Next(ref grouping))
        {
            dictionary[grouping.Key] = grouping.AsSpan().ToArray().ToList();
        }

        linqResult.Keys.OrderBy(k => k).ShouldBe(dictionary.Keys.OrderBy(k => k));
        foreach (var key in linqResult.Keys)
        {
            linqResult[key].ShouldBeEquivalentTo(dictionary[key]);
        }
    }
}