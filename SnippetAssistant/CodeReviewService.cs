using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DevryApplication.Tasks.Scheduling;
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

        /// <summary>
        /// Languages that are currently supported
        /// Key: file extension
        /// Value: Language name
        /// </summary>
        public static Dictionary<string, string> SupportedLanguages => new()
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
            string results = await ExecuteProspectorTool(language, codeFilePath);
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
        ///  Cleanup the files associated with this report
        /// </summary>
        /// <param name="deleteAfterMinutes"></param>
        /// <param name="originalFile"></param>
        /// <param name="reportFile"></param>
        public void Cleanup(int deleteAfterMinutes, string originalFile, string reportFile)
        {
            if (File.Exists(originalFile))
                File.Delete(originalFile);
            
            SchedulerBackgroundService.Instance.ScheduleFileDelete(reportFile, DateTime.Now.AddMinutes(deleteAfterMinutes));
        }
        
        /// <summary>
        /// Executes the appropriate script to scan a given path
        /// </summary>
        /// <param name="language"></param>
        /// <param name="codeFilePath"></param>
        /// <returns></returns>
        private async Task<string> ExecuteProspectorTool(string language, string codeFilePath)
        {
            Process toolProcess = new Process();
            string profilePath = Path.Join(StorageHandler.ToolProfilesPath, language, "ProspectorProfile.yml");
            
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "prospector",
                Arguments = $"--profile-path \"{profilePath}\" --output-format json \"{codeFilePath}\"",
                RedirectStandardOutput = true
            };

            toolProcess.StartInfo = startInfo;
            toolProcess.Start();
            await toolProcess.WaitForExitAsync();
            return await toolProcess.StandardOutput.ReadToEndAsync();
        }
    }
}