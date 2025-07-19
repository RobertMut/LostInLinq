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
        _numbers = new int[100000];
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
}