using PerformanceBenchmarks.Core.Struct;
using Shouldly;
using TestProject1;

namespace PerformanceBenchmarks.Tests;

public class SelectTests
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
    public void StructSelect_ShouldBe_TheSameAs_Linq_UsingClass()
    {
        string[] linqResult = _testData.Select(x => x.Message).ToArray();
        string[] structSelectResult = _testData.StructSelect(x => x.Message).ToArray();

        linqResult.ShouldBeEquivalentTo(structSelectResult);
    }

    [Test]
    public void StructSelect_ShouldBe_TheSameAs_Linq_UsingNumbers()
    {
        int[] linqResult = _numbers.Select(x => (x / 2 * 14)).ToArray();
        int[] structSelectResult = _numbers.StructSelect(x => (x / 2 * 14)).ToArray();

        linqResult.ShouldBeEquivalentTo(structSelectResult);
    }

    [Test]
    public void StructSelect_Should_HandleEmptyArray()
    {
        var emptyArray = Array.Empty<int>();
        string[] linqResult = emptyArray.Select(x => x.ToString()).ToArray();
        string[] structSelectResult = emptyArray.StructSelect(x => x.ToString()).ToArray();

        structSelectResult.ShouldBeEquivalentTo(linqResult);
        structSelectResult.Length.ShouldBe(0);
    }

    [Test]
    public void StructSelect_Should_HandleTypeTransformation_IntToString()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };
        string[] linqResult = numbers.Select(x => $"Number: {x}").ToArray();
        string[] structSelectResult = numbers.StructSelect(x => $"Number: {x}").ToArray();

        structSelectResult.ShouldBeEquivalentTo(linqResult);
    }

    [Test]
    public void StructSelect_Should_HandleTypeTransformation_StringToInt()
    {
        var strings = new[] { "1", "2", "3", "4", "5" };
        int[] linqResult = strings.Select(int.Parse).ToArray();
        int[] structSelectResult = strings.StructSelect(int.Parse).ToArray();

        structSelectResult.ShouldBeEquivalentTo(linqResult);
    }

    [Test]
    public void StructSelect_Should_HandleComplexTransformation()
    {
        var result1 = _testData.Select(x => new { x.Guid, Length = x.Message.Length, IsEven = x.Number % 2 == 0 }).ToArray();
        var result2 = _testData.StructSelect(x => new { x.Guid, Length = x.Message.Length, IsEven = x.Number % 2 == 0 }).ToArray();

        for (int i = 0; i < result1.Length; i++)
        {
            result2[i].Guid.ShouldBe(result1[i].Guid);
            result2[i].Length.ShouldBe(result1[i].Length);
            result2[i].IsEven.ShouldBe(result1[i].IsEven);
        }
    }

    [Test]
    public void StructSelect_Should_HandleSingleElement()
    {
        var singleElement = new[] { 42 };
        string[] linqResult = singleElement.Select(x => x.ToString()).ToArray();
        string[] structSelectResult = singleElement.StructSelect(x => x.ToString()).ToArray();

        structSelectResult.ShouldBeEquivalentTo(linqResult);
        structSelectResult.Length.ShouldBe(1);
        structSelectResult[0].ShouldBe("42");
    }

    [Test]
    public void StructSelect_Should_PreserveOrder()
    {
        var testArray = new[] { 10, 5, 8, 3, 6, 1, 9, 2, 7, 4 };
        int[] linqResult = testArray.Select(x => x * 2).ToArray();
        int[] structSelectResult = testArray.StructSelect(x => x * 2).ToArray();

        for (int i = 0; i < linqResult.Length; i++)
        {
            structSelectResult[i].ShouldBe(linqResult[i]);
        }
    }

    [Test]
    public void StructSelect_Should_HandleIdentityTransformation()
    {
        int[] linqResult = _numbers.Select(x => x).ToArray();
        int[] structSelectResult = _numbers.StructSelect(x => x).ToArray();

        structSelectResult.ShouldBeEquivalentTo(linqResult);
    }

    [Test]
    public void StructSelect_Should_HandleMathematicalOperations()
    {
        var numbers = Enumerable.Range(1, 100).ToArray();
        double[] linqResult = numbers.Select(x => Math.Sqrt(x * x + 1)).ToArray();
        double[] structSelectResult = numbers.StructSelect(x => Math.Sqrt(x * x + 1)).ToArray();

        structSelectResult.ShouldBeEquivalentTo(linqResult);
    }

    [Test]
    public void StructSelect_Should_HandleLargeDataset()
    {
        var largeData = Enumerable.Range(0, 1000000).ToArray();
        long[] linqResult = largeData.Select(x => (long)x * x).ToArray();
        long[] structSelectResult = largeData.StructSelect(x => (long)x * x).ToArray();

        structSelectResult.ShouldBeEquivalentTo(linqResult);
    }
}
