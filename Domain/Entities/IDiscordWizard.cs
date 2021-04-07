using System;
using Domain.Common.Models;

namespace Domain.Entities
{
    public interface IDiscordWizard : IExtendableObject
    {
        string AuthorName { get; set; }
        string AuthorIcon { get; }
        string Headline { get; }
        string Description { get; }
        TimeSpan? TimeoutOverride { get; }
    }
}