namespace TestProject1;

public class TestDataGenerator
{
    public static TestData[] Generate(int count)
    {
        TestData[] arr = new TestData[count];

        Random rng = new();
        
        for(int i = 0; i < count; i++)
        {
            byte[] chars = new byte[10];
            
            rng.NextBytes(chars);
            arr[i] = new TestData()
            {
                Message = System.Text.Encoding.ASCII.GetString(chars),
                Number = rng.NextDouble(),
                Guid = Guid.NewGuid()
            };
        }

        return arr;
    }

    public static int[] GenerateNumbers(int count)
    {
        int[] arr = new int[count];

        Random rng = new();
        
        for(int i = 0; i < count; i++)
        {
            arr[i] = rng.Next();
        }

        return arr;
    }
}

public class TestData
{
    public string Message { get; set; }
    public double Number { get; set; }
    public Guid Guid { get; set; }
}