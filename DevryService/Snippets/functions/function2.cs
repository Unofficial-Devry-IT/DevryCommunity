using System.Linq;

public static int Average(params int[] numbers)
{
    return numbers.Sum() / numbers.Count();

    // or you can do
    // return numbers.Average();
}

Console.WriteLine(Average(1,2,3,4,5,6));
Console.WriteLine(Average(new int[] {1,2,322,22,33}));