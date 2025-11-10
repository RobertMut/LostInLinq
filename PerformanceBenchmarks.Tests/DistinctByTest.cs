using PerformanceBenchmarks.Core.Struct;
using Shouldly;
using TestProject1;

namespace PerformanceBenchmarks.Tests;

[TestFixture]
[TestOf(typeof(DistinctByStruct))]
public class DistinctByTests
{
    private TestData[] _testData;

    [SetUp]
    public void Setup()
    {
        _testData = TestDataGenerator.Generate(10000);
    }

    [Test]
    public void StructDistinctBy_ShouldBe_TheSameAs_Linq_WithSimpleKey()
    {
        var numbers = new[] { 1, 2, 3, 2, 4, 3, 5, 1 };
        int[] linqResult = numbers.DistinctBy(x => x).ToArray();
        int[] structResult = numbers.StructDistinctBy(x => x).ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
    }

    [Test]
    public void StructDistinctBy_Should_HandleEmptyArray()
    {
        var emptyArray = Array.Empty<int>();
        int[] linqResult = emptyArray.DistinctBy(x => x).ToArray();
        int[] structResult = emptyArray.StructDistinctBy(x => x).ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
        structResult.Length.ShouldBe(0);
    }

    [Test]
    public void StructDistinctBy_Should_HandleAllUnique()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };
        int[] linqResult = numbers.DistinctBy(x => x).ToArray();
        int[] structResult = numbers.StructDistinctBy(x => x).ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
        structResult.Length.ShouldBe(5);
    }

    [Test]
    public void StructDistinctBy_Should_HandleAllDuplicates()
    {
        var numbers = new[] { 1, 1, 1, 1, 1 };
        int[] linqResult = numbers.DistinctBy(x => x).ToArray();
        int[] structResult = numbers.StructDistinctBy(x => x).ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
        structResult.Length.ShouldBe(1);
        structResult[0].ShouldBe(1);
    }

    [Test]
    public void StructDistinctBy_Should_HandleSingleElement()
    {
        var numbers = new[] { 42 };
        int[] linqResult = numbers.DistinctBy(x => x).ToArray();
        int[] structResult = numbers.StructDistinctBy(x => x).ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
        structResult.Length.ShouldBe(1);
        structResult[0].ShouldBe(42);
    }

    [Test]
    public void StructDistinctBy_Should_PreserveFirstOccurrence()
    {
        var numbers = new[] { 1, 2, 3, 2, 4, 1, 5 };
        int[] linqResult = numbers.DistinctBy(x => x).ToArray();
        int[] structResult = numbers.StructDistinctBy(x => x).ToArray();

        for (int i = 0; i < linqResult.Length; i++)
        {
            structResult[i].ShouldBe(linqResult[i]);
        }
    }

    [Test]
    public void StructDistinctBy_Should_HandleComplexKey()
    {
        var words = new[] { "apple", "banana", "apricot", "blueberry", "avocado", "berry" };
        string[] linqResult = words.DistinctBy(x => x[0]).ToArray();
        string[] structResult = words.StructDistinctBy(x => x[0]).ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
    }

    [Test]
    public void StructDistinctBy_Should_HandleClassObjects()
    {
        var linqResult = _testData.DistinctBy(x => x.Guid).ToArray();
        var structResult = _testData.StructDistinctBy(x => x.Guid).ToArray();

        structResult.Length.ShouldBe(linqResult.Length);
        
        for (int i = 0; i < linqResult.Length; i++)
        {
            structResult[i].Guid.ShouldBe(linqResult[i].Guid);
        }
    }

    [Test]
    public void StructDistinctBy_Should_HandleStringKeys()
    {
        var people = new[]
        {
            new { Name = "John", Age = 30 },
            new { Name = "Jane", Age = 25 },
            new { Name = "John", Age = 35 },
            new { Name = "Bob", Age = 30 }
        };

        var linqResult = people.DistinctBy(x => x.Name).ToArray();
        var structResult = people.StructDistinctBy(x => x.Name).ToArray();

        structResult.Length.ShouldBe(linqResult.Length);
        for (int i = 0; i < linqResult.Length; i++)
        {
            structResult[i].Name.ShouldBe(linqResult[i].Name);
        }
    }

    [Test]
    public void StructDistinctBy_Should_HandleNumericKeys()
    {
        var people = new[]
        {
            new { Name = "John", Age = 30 },
            new { Name = "Jane", Age = 25 },
            new { Name = "Bob", Age = 30 },
            new { Name = "Alice", Age = 25 }
        };

        var linqResult = people.DistinctBy(x => x.Age).ToArray();
        var structResult = people.StructDistinctBy(x => x.Age).ToArray();

        structResult.Length.ShouldBe(linqResult.Length);
        for (int i = 0; i < linqResult.Length; i++)
        {
            structResult[i].Age.ShouldBe(linqResult[i].Age);
        }
    }

    [Test]
    public void StructDistinctBy_Should_HandleChainedOperations()
    {
        var numbers = Enumerable.Range(1, 1000).ToArray();
        var linqResult = numbers
            .Where(x => x % 2 == 0)
            .Select(x => x / 2)
            .DistinctBy(x => x % 10)
            .ToArray();

        var structResult = numbers
            .StructWhere(x => x % 2 == 0)
            .StructSelect(x => x / 2)
            .StructDistinctBy(x => x % 10)
            .ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
    }

    [Test]
    public void StructDistinctBy_Should_HandleLargeDataset()
    {
        var largeData = Enumerable.Range(0, 100000).Select(x => x % 1000).ToArray();
        int[] linqResult = largeData.DistinctBy(x => x).ToArray();
        int[] structResult = largeData.StructDistinctBy(x => x).ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
        structResult.Length.ShouldBe(1000);
    }

    [Test]
    public void StructDistinctBy_Should_HandleHighDuplicationRate()
    {
        var data = Enumerable.Repeat(new[] { 1, 2, 3 }, 10000).SelectMany(x => x).ToArray();
        int[] linqResult = data.DistinctBy(x => x).ToArray();
        int[] structResult = data.StructDistinctBy(x => x).ToArray();

        structResult.ShouldBeEquivalentTo(linqResult);
        structResult.Length.ShouldBe(3);
    }

    [Test]
    public void StructDistinctBy_Should_PreserveOrderAcrossKeys()
    {
        var numbers = new[] { 5, 1, 8, 1, 3, 5, 9, 3, 2 };
        int[] linqResult = numbers.DistinctBy(x => x).ToArray();
        int[] structResult = numbers.StructDistinctBy(x => x).ToArray();

        for (int i = 0; i < linqResult.Length; i++)
        {
            structResult[i].ShouldBe(linqResult[i]);
        }
    }
}