using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DevryInfrastructure;
using SnippetAssistant.Python;

namespace SnippetAssistant
{
    /// <summary>
    /// Entry point for starting the code review process
    /// </summary>
    public class CodeReviewService
    {
        public static bool IsLinux
        {
            get
            {
                int p = (int) Environment.OSVersion.Platform;
                return p is 4 or 6 or 128; 
            }
        }

        public static Dictionary<string, string> SupportedLanguages => new Dictionary<string, string>()
        {
            {"py", "Python"}
        };

        public Task DownloadFile(string url, string downloadDestination)
        {
            using WebClient client = new WebClient();
            client.DownloadFile(
                new Uri(url),
                downloadDestination
            );

            return Task.CompletedTask;
        }

        /// <summary>
        /// Analyze a given file for a particular language
        /// </summary>
        /// <param name="language"></param>
        /// <param name="codeFilePath"></param>
        /// <returns>HTML Report based on scan</returns>
        public async Task<ReportBase> AnalyzeResults(string language, string codeFilePath)
        {
            string results = await ExecuteTool(language, codeFilePath);
            string originalCode = await File.ReadAllTextAsync(codeFilePath);
            
            ReportBase report = null;
            
            switch (language.ToLower())
            {
                case "python":
                    var deserializedOutput = Newtonsoft.Json.JsonConvert.DeserializeObject<PythonOutput>(results);
                    report = new PythonReport(deserializedOutput, originalCode);
                    break;
            
                default:
                    throw new NotImplementedException($"{language} has not been implemented yet");
            }

            return report;
        }
        
        /// <summary>
        /// Executes the appropriate script to scan a given path
        /// </summary>
        /// <param name="language"></param>
        /// <param name="codeFilePath"></param>
        /// <returns></returns>
        private async Task<string> ExecuteTool(string language, string codeFilePath)
        {
            Process toolProcess = new Process();

            FileInfo fileInfo = new FileInfo(codeFilePath);
            
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = "powershell.exe",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            if (IsLinux)
            {
                startInfo.Arguments = $"./{language}-scan.sh \"{fileInfo.FullName}\" \"{fileInfo.Name}\"";
                startInfo.FileName = "/bin/bash";
            }
            else
                startInfo.Arguments = $"{language}-scan.ps1 \"{Path.Join(StorageHandler.ToolProfilesPath, language)}\" \"{fileInfo.FullName}\" {fileInfo.Name}";
                        
            toolProcess.Start();
            await toolProcess.WaitForExitAsync();

            return await File.ReadAllTextAsync($"{fileInfo.Name}.json");
        }
    }
}