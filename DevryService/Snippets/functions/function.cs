public static int Average(params int[] numbers)
{
    int sum = 0;

    foreach (int i in numbers)
        sum += i;

    return sum / numbers.Length;
}

// The params allows us to put in as many arguments as we want
Console.WriteLine(Average(2, 3, 4, 5, 6));

// Or we can pass in an array of integers!
Console.WriteLine(Average(new int[] { 1, 2, 3, 4, 5 }));