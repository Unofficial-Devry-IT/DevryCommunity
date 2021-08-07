using System;
using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.Interactivity;
using DisCatSharp.SlashCommands;

namespace DevryBot
{
    public interface IBot
    {
        DiscordClient Client { get; }
        InteractivityExtension Interactivity { get; }
        SlashCommandsExtension SlashCommands { get; }
        IServiceProvider ServiceProvider { get; }
        DiscordGuild MainGuild { get; }
    }
}