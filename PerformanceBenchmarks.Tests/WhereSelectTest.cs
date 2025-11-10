using PerformanceBenchmarks.Core.Struct;
using Shouldly;
using TestProject1;

namespace PerformanceBenchmarks.Tests;

public class WhereSelectTests
{
    private TestData[] _testData;
    private int[] _numbers;

    [SetUp]
    public void Setup()
    {
        _testData = TestDataGenerator.Generate(100000);
        _numbers = Enumerable.Range(0, 100000).ToArray();
    }

    [Test]
    public void WhereSelectStruct_ShouldBe_TheSameAs_Linq_UsingClass()
    {
        var linqResult = _testData
            .Where(x => x.Number % 2 == 0 && x.Message.StartsWith('x'))
            .Select(x => $"{x.Number}{x.Message}")
            .ToArray();
        var structWhereResult = _testData
            .StructWhere(x => x.Number % 2 == 0 && x.Message.StartsWith('x'))
            .StructSelect(x => $"{x.Number}{x.Message}")
            .ToArray();

        linqResult.ShouldBeEquivalentTo(structWhereResult);
    }

    [Test]
    public void WhereSelectStruct_ShouldBe_TheSameAs_Linq_UsingNumbers()
    {
        var linqResult = _numbers
            .Where(x => x * 5 % 3 == 0)
            .Select(x => x.ToString())
            .ToArray();
        var structWhereResult = _numbers
            .StructWhere(x => x * 5 % 3 == 0)
            .StructSelect(x => x.ToString())
            .ToArray();

        linqResult.ShouldBeEquivalentTo(structWhereResult);
    }

    [Test]
    public void WhereSelectStruct_Should_HandleEmptyResult()
    {
        var linqResult = _numbers
            .Where(x => x > 200000)
            .Select(x => x * 2)
            .ToArray();
        var structResult = _numbers
            .StructWhere(x => x > 200000)
            .StructSelect(x => x * 2)
            .ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
        structResult.Length.ShouldBe(0);
    }

    [Test]
    public void WhereSelectStruct_Should_HandleComplexChaining()
    {
        var linqResult = _testData
            .Where(x => x.Number > 0.5)
            .Select(x => new { Id = x.Guid, Score = x.Number * 100 })
            .Where(x => x.Score > 75)
            .Select(x => x.Id.ToString())
            .ToArray();

        var structResult = _testData
            .StructWhere(x => x.Number > 0.5)
            .StructSelect(x => new { Id = x.Guid, Score = x.Number * 100 })
            .StructWhere(x => x.Score > 75)
            .StructSelect(x => x.Id.ToString())
            .ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
    }

    [Test]
    public void SelectWhereStruct_Should_WorkInReverseOrder()
    {
        var linqResult = _numbers
            .Select(x => x * 2)
            .Where(x => x > 1000)
            .ToArray();
        var structResult = _numbers
            .StructSelect(x => x * 2)
            .StructWhere(x => x > 1000)
            .ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
    }

    [Test]
    public void WhereSelectStruct_Should_HandleMultipleFilters()
    {
        var linqResult = _numbers
            .Where(x => x % 2 == 0)
            .Where(x => x % 3 == 0)
            .Where(x => x > 100)
            .Select(x => x.ToString())
            .ToArray();

        var structResult = _numbers
            .StructWhere(x => x % 2 == 0)
            .StructWhere(x => x % 3 == 0)
            .StructWhere(x => x > 100)
            .StructSelect(x => x.ToString())
            .ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
    }

    [Test]
    public void WhereSelectStruct_Should_HandleMultipleTransformations()
    {
        var linqResult = _numbers
            .Select(x => x * 2)
            .Select(x => x + 10)
            .Select(x => x.ToString())
            .Where(x => x.Length > 1)
            .ToArray();

        var structResult = _numbers
            .StructSelect(x => x * 2)
            .StructSelect(x => x + 10)
            .StructSelect(x => x.ToString())
            .StructWhere(x => x.Length > 1)
            .ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
    }

    [Test]
    public void WhereSelectStruct_Should_HandleEmptyArray()
    {
        var emptyArray = Array.Empty<int>();
        var linqResult = emptyArray
            .Where(x => x > 0)
            .Select(x => x.ToString())
            .ToArray();
        var structResult = emptyArray
            .StructWhere(x => x > 0)
            .StructSelect(x => x.ToString())
            .ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
        structResult.Length.ShouldBe(0);
    }

    [Test]
    public void WhereSelectStruct_Should_PreserveOrderInChain()
    {
        var testArray = new[] { 10, 5, 8, 3, 6, 1, 9, 2, 7, 4 };
        var linqResult = testArray
            .Where(x => x > 3)
            .Select(x => x * 10)
            .Where(x => x < 80)
            .ToArray();

        var structResult = testArray
            .StructWhere(x => x > 3)
            .StructSelect(x => x * 10)
            .StructWhere(x => x < 80)
            .ToArray();

        for (int i = 0; i < linqResult.Length; i++)
        {
            structResult[i].ShouldBe(linqResult[i]);
        }
    }

    [Test]
    public void WhereSelectStruct_Should_HandleLargeChain()
    {
        var largeData = Enumerable.Range(0, 10000).ToArray();
        var linqResult = largeData
            .Where(x => x % 2 == 0)
            .Select(x => x * 3)
            .Where(x => x > 100)
            .Select(x => x.ToString())
            .Where(x => x.Length > 2)
            .Select(int.Parse)
            .ToArray();

        var structResult = largeData
            .StructWhere(x => x % 2 == 0)
            .StructSelect(x => x * 3)
            .StructWhere(x => x > 100)
            .StructSelect(x => x.ToString())
            .StructWhere(x => x.Length > 2)
            .StructSelect(int.Parse)
            .ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
    }
}