using System;
using SnippetAssistant.Core;

namespace SnippetAssistant.Python
{
    public class PythonSummary : ISummary
    {
        public DateTime Started { get; set; }
        public string[] Libraries { get; set; }
        public string Scritness { get; set; }
        public string Profiles { get; set; }
        public string[] Tools { get; set; }
        public int MessageCount { get; set; }
        public DateTime Completed { get; set; }
        public string TimeTaken { get; set; }
        public string Formatter { get; set; }
    }
}