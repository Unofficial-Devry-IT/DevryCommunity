using System;
using System.Collections.Generic;
using DevryServices.Common.Models;
using Domain.Entities.Configs;

namespace Domain.Entities
{
    public interface IDiscordCommand
    {
        Config CurrentConfig { get; }
    }
}