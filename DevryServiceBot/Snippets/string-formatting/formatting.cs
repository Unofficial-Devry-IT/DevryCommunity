public string Log(string message, object obj = null)
{
    /*
        With the dollar sign '$' in front of the quotes we can
        Use string interpolation to insert our arguments!
    */
    System.Console.WriteLine($"{message}\n\t{obj?.ToString()}");

    /*
        Can use string.Format as well
        Similar to python we can use the curly brackets {}
        Inside of these brackets we can use numbers (Starting at 0)
        To specify which argument goes where
    */
    System.Console.WriteLine(string.Format("{0}\n\t{1}", 
        message, obj?.ToString()));
}