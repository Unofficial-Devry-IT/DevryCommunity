using Domain.Common;
using Domain.Entities;

namespace Domain.Events.CodeSnippets
{
    public class CodeSnippetCreatedEvent : DomainEvent
    {
        public CodeSnippetCreatedEvent(CodeSnippet snippet)
        {
            Snippet = snippet;
        }

        public CodeSnippet Snippet { get; }
    }
}