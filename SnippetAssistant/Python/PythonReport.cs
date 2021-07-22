using System.Threading.Tasks;
using Razor.Templating.Core;

namespace SnippetAssistant.Python
{
    public class PythonReport : ReportBase
    {
        /// <summary>
        /// Report to use for report generation
        /// </summary>
        private readonly PythonOutput _report;
        
        public PythonReport(PythonOutput report, string originalCode) : base(originalCode, "python")
        {
            _report = report;
        }

        protected override Task<string> GenerateSummary()
        {
            _report.Summary.MessageCount = _report.Messages.Length;
            return RazorTemplateEngine.RenderAsync("~/Views/Shared/Summary.cshtml", _report.Summary);
        }

        protected override async Task<string> GenerateErrorReport()
        {
            string[] lines = OriginalCode.Split("\n");
            
            foreach (var message in _report.Messages)
            {
                if (!message.Location.Line.HasValue)
                    continue;

                int index = message.Location.Line.Value - 1;

                string text = "";

                if (index > 0)
                    text += lines[index - 1] + "\n\n";

                text += "# " + message.Message + "\n";
                
                text += lines[index];
                message.CommentedCode = text;
            }
            
            string messages =
                await RazorTemplateEngine.RenderAsync("~/Views/Python/PythonErrors.cshtml", _report.Messages);
            
            return messages;
        }
    }
}