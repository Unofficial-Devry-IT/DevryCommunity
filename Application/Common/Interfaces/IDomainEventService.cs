using System.Threading.Tasks;
using Domain.Common.Models;

namespace Application.Common.Interfaces
{
    /// <summary>
    /// Contract for handling/publishing events across the architecture
    /// </summary>
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}