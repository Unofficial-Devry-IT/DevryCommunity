using SnippetAssistant.Core;

namespace SnippetAssistant.Python
{
    public class PythonOutput : IOutput
    {
        public PythonSummary Summary { get; set; }
        public PythonMessage[] Messages { get; set; }
    }
}