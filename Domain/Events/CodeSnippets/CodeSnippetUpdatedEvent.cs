using Domain.Common;
using Domain.Entities;

namespace Domain.Events.CodeSnippets
{
    public class CodeSnippetUpdatedEvent : DomainEvent
    {
        public CodeSnippetUpdatedEvent(CodeSnippet snippet)
        {
            Snippet = snippet;
        }

        public CodeSnippet Snippet { get; }
    }
}