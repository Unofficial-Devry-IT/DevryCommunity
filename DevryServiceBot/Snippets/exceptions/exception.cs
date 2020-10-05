Console.WriteLine("How old are you?");
string input = Console.ReadLine();

try
{
    // Must be a number. Otherwise it will throw an exception
    int age = int.Parse(input);
}
catch
{
    Console.WriteLine("Invalid input. Was expecting a number");
}