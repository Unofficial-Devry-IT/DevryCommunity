using System.Threading.Tasks;
using Razor.Templating.Core;
using SnippetAssistant.Helpers;

namespace SnippetAssistant
{
    public class ReportBase
    {
        /// <summary>
        /// Title of report
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Compiled HTML Contents
        /// </summary>
        public string Contents { get; private set; }
        
        /// <summary>
        /// Type of language that shall be used for highlighting purposes
        /// </summary>
        public readonly string Language;
        
        /// <summary>
        /// Original code the user provided
        /// </summary>
        protected readonly string OriginalCode;

        public ReportBase(string fileContents, string language)
        {
            Language = language;
            OriginalCode = fileContents;
        }
        
        /// <summary>
        /// Generates the HTML page that the user shall receive later
        /// </summary>
        /// <returns>Compiled HTML from Main.cshtml template</returns>
        public async Task<string> GenerateReport()
        {
            Contents = await GenerateSummary() +
                       await GenerateErrorReport() +
                       Html.CreateCodeBlock(Language, OriginalCode);

            return await RazorTemplateEngine.RenderAsync("~/Views/Shared/Main.cshtml", this);
        }

        /// <summary>
        /// Generate Summary block that appears on the top of the report
        /// </summary>
        /// <returns>Compiled HTML for "jumbotron"</returns>
        protected async virtual Task<string> GenerateSummary() 
            => "<p>Default Summary</p>";
        
        /// <summary>
        /// Generate the error/suggestion messages
        /// </summary>
        /// <returns>Compiled HTML for the bulk of our report</returns>
        protected async virtual Task<string> GenerateErrorReport()  
            => "<p>No errors to report</p>";
    }
}