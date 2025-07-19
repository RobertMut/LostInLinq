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
        _numbers = new int[100000];
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
}
