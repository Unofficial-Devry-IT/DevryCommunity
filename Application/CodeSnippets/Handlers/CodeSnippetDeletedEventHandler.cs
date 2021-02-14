using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.CodeSnippets;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.CodeSnippets.Handlers
{
    public class CodeSnippetDeletedEventHandler : INotificationHandler<DomainEventNotification<CodeSnippetDeletedEvent>>
    {
        private readonly ILogger<CodeSnippetDeletedEventHandler> _logger;

        public CodeSnippetDeletedEventHandler(ILogger<CodeSnippetDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<CodeSnippetDeletedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}