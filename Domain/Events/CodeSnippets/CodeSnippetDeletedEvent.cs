using Domain.Common;
using Domain.Entities;

namespace Domain.Events.CodeSnippets
{
    public class CodeSnippetDeletedEvent : DomainEvent
    {
        public CodeSnippet Snippet { get; }

        public CodeSnippetDeletedEvent(CodeSnippet snippet)
        {
            Snippet = snippet;
        }
    }
}