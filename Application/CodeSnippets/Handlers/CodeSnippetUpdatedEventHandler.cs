using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.CodeSnippets;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.CodeSnippets.Handlers
{
    public class CodeSnippetUpdatedEventHandler : INotificationHandler<DomainEventNotification<CodeSnippetUpdatedEvent>>
    {
        private readonly ILogger<CodeSnippetUpdatedEventHandler> _logger;

        public CodeSnippetUpdatedEventHandler(ILogger<CodeSnippetUpdatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<CodeSnippetUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}