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
        _numbers = new int[100000];
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
}
