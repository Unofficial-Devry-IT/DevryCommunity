using System;
using System.IO;
using Newtonsoft.Json;
using SnippetAssistant.Python;

namespace SnippetAssistant
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string path = @"C:\Users\jonbr\OneDrive\Desktop\prospector_report.json";
            PythonOutput report = JsonConvert.DeserializeObject<PythonOutput>(File.ReadAllText(path));
            PythonReport parser = new PythonReport(report, File.ReadAllText(@"C:\Users\jonbr\PycharmProjects\pythonProject1\Enemy.py"));
            
            string value = parser.GenerateReport().Result;
            File.WriteAllText(@"C:\Users\jonbr\OneDrive\Desktop\report.html", value);
            Console.WriteLine(value);
        }
    }
}