using System;
using System.Collections.Generic;
using DevryServices.Common.Models;

namespace Domain.Entities
{
    public interface IDiscordCommand : IExtendableObject
    {
        string CommandName { get; }
        IList<string> RestrictedRoles { get; }
        string Description { get; }
        string Emoji { get; }
        TimeSpan? TimeoutOverride { get; }
    }
}