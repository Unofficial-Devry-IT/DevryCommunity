using System.IO;

StreamReader reader = new StreamReader("config.cfg");
string line;

while((line = reader.ReadLine()) != null)
{
    Console.WriteLine(line);
}

reader.Close();