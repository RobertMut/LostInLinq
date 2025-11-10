using PerformanceBenchmarks.Core.Struct;
using Shouldly;
using TestProject1;

namespace PerformanceBenchmarks.Tests;

public class WhereTests
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
    public void WhereStruct_ShouldBe_TheSameAs_Linq_UsingClass()
    {
        TestData[] linqResult = _testData.Where(x => x.Number % 2 == 0 && x.Message.StartsWith('x')).ToArray();
        TestData[] structWhereResult = _testData.StructWhere(x => x.Number % 2 == 0 && x.Message.StartsWith('x')).ToArray();

        linqResult.ShouldBeEquivalentTo(structWhereResult);
    }

    [Test]
    public void WhereStruct_ShouldBe_TheSameAs_Linq_UsingNumbers()
    {
        int[] linqResult = _numbers.Where(x => x * 5 % 3 == 0).ToArray();
        int[] structWhereResult = _numbers.StructWhere(x => x * 5 % 3 == 0).ToArray();

        linqResult.ShouldBeEquivalentTo(structWhereResult);
    }

    [Test]
    public void WhereStruct_Should_HandleEmptyArray()
    {
        var emptyArray = Array.Empty<int>();
        int[] linqResult = emptyArray.Where(x => x > 0).ToArray();
        int[] structWhereResult = emptyArray.StructWhere(x => x > 0).ToArray();

        structWhereResult.ShouldBeEquivalentTo(linqResult);
        structWhereResult.Length.ShouldBe(0);
    }

    [Test]
    public void WhereStruct_Should_HandleAllItemsMatch()
    {
        var allMatch = new[] { 2, 4, 6, 8, 10 };
        int[] linqResult = allMatch.Where(x => x % 2 == 0).ToArray();
        int[] structWhereResult = allMatch.StructWhere(x => x % 2 == 0).ToArray();

        structWhereResult.ShouldBeEquivalentTo(linqResult);
        structWhereResult.Length.ShouldBe(5);
    }

    [Test]
    public void WhereStruct_Should_HandleNoItemsMatch()
    {
        var noneMatch = new[] { 1, 3, 5, 7, 9 };
        int[] linqResult = noneMatch.Where(x => x % 2 == 0).ToArray();
        int[] structWhereResult = noneMatch.StructWhere(x => x % 2 == 0).ToArray();

        structWhereResult.ShouldBeEquivalentTo(linqResult);
        structWhereResult.Length.ShouldBe(0);
    }

    [Test]
    public void WhereStruct_Should_HandleComplexPredicate()
    {
        int[] linqResult = _numbers.Where(x => x > 1000 && x < 5000 && x % 7 == 0).ToArray();
        int[] structWhereResult = _numbers.StructWhere(x => x > 1000 && x < 5000 && x % 7 == 0).ToArray();

        linqResult.ShouldBeEquivalentTo(structWhereResult);
    }

    [Test]
    public void WhereStruct_Should_HandleSingleElement_Match()
    {
        var singleElement = new[] { 42 };
        int[] linqResult = singleElement.Where(x => x == 42).ToArray();
        int[] structWhereResult = singleElement.StructWhere(x => x == 42).ToArray();

        structWhereResult.ShouldBeEquivalentTo(linqResult);
        structWhereResult.Length.ShouldBe(1);
    }

    [Test]
    public void WhereStruct_Should_HandleSingleElement_NoMatch()
    {
        var singleElement = new[] { 42 };
        int[] linqResult = singleElement.Where(x => x == 100).ToArray();
        int[] structWhereResult = singleElement.StructWhere(x => x == 100).ToArray();

        structWhereResult.ShouldBeEquivalentTo(linqResult);
        structWhereResult.Length.ShouldBe(0);
    }

    [Test]
    public void WhereStruct_Should_PreserveOrder()
    {
        var testArray = new[] { 10, 5, 8, 3, 6, 1, 9, 2, 7, 4 };
        int[] linqResult = testArray.Where(x => x > 5).ToArray();
        int[] structWhereResult = testArray.StructWhere(x => x > 5).ToArray();

        for (int i = 0; i < linqResult.Length; i++)
        {
            structWhereResult[i].ShouldBe(linqResult[i]);
        }
    }

    [Test]
    public void WhereStruct_Should_HandleLargeDataset_Efficiently()
    {
        var largeData = Enumerable.Range(0, 1000000).ToArray();
        int[] linqResult = largeData.Where(x => x % 13 == 0).ToArray();
        int[] structWhereResult = largeData.StructWhere(x => x % 13 == 0).ToArray();

        structWhereResult.ShouldBeEquivalentTo(linqResult);
    }
}
