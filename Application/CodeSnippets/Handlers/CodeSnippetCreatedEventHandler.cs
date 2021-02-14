using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.CodeSnippets;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.CodeSnippets.Handlers
{
    public class CodeSnippetCreatedEventHandler : INotificationHandler<DomainEventNotification<CodeSnippetCreatedEvent>>
    {
        private readonly ILogger<CodeSnippetCreatedEventHandler> _logger;

        public CodeSnippetCreatedEventHandler(ILogger<CodeSnippetCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<CodeSnippetCreatedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}