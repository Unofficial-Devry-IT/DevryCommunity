using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DevryInfrastructure;
using Newtonsoft.Json;
using SnippetAssistant.Python;

namespace SnippetAssistant
{
    class Program
    {
        private static string path = @"C:\Users\jonbr\OneDrive\Desktop\prospector_report.json";
        private static string languageFile = @"C:\Users\jonbr\PycharmProjects\pythonProject1\InputManager.py";
        private static string reportOutput = @"C:\Users\jonbr\OneDrive\Desktop\report.html";
        
        static void Main(string[] args)
        {
            string scanScriptPath = Path.Join(StorageHandler.ScriptsPath, "Python", "python-scan.ps1");
            FileInfo info = new FileInfo(languageFile);

            string parameters =
                $"--FilePath \"{scanScriptPath}\" --profile \"{Path.Join(StorageHandler.ToolProfilesPath, "Python")}\" " +
                $"--scan \"{info.FullName}\" --name \"{info.Name.Split(".").First()}\"";
            
            Process powershell = new Process()
            {
                StartInfo = new ProcessStartInfo("powershell.exe", parameters)
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true
                }
            };

            powershell.Start();
            powershell.WaitForExit();
            Console.WriteLine(powershell.StandardOutput.ReadToEnd());
            Console.WriteLine("Finished");
        }

        static void Test()
        {
            PythonOutput report = JsonConvert.DeserializeObject<PythonOutput>(File.ReadAllText(path));
            PythonReport parser = new PythonReport(report, File.ReadAllText(languageFile));

            string value = parser.GenerateReport().Result;
            File.WriteAllText(reportOutput, value);
            Console.WriteLine(value);
        }
    }
}