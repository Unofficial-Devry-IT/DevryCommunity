using System;
using DevryServices.Common.Models;

namespace Domain.Entities
{
    public interface IDiscordWizard : IExtendableObject
    {
        string AuthorName { get; }
        string AuthorIcon { get; }
        string Headline { get; }
        string Description { get; }
        TimeSpan? TimeoutOverride { get; }
    }
}