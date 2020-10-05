using System.IO;

// Since StreamReader implements **IDisposable** we can use a using block!
// This automatically cleans up the object if used in a using blockStreamReader reader = new StreamReader("config.cfg");

using(StreamReader reader = new StreamReader("config.cfg"))
{
    string line;
    while((line = reader.ReadLine()) != null)
    {
        Console.WriteLine(line);
    }
}
// reader is automatically closed or "Disposed"
