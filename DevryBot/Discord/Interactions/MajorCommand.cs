using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlusNextGen;
using DSharpPlusNextGen.Entities;
using DSharpPlusNextGen.Enums;
using DSharpPlusNextGen.EventArgs;
using DiscordMember = DSharpPlus.Entities.DiscordMember;

namespace DevryBot.Discord.Interactions
{
    public static class MajorCommand
    {
        private static readonly ulong WELCOME_CHANNEL_ID = 618254766396538903;
        
        public static async Task ClientOnGuildMemberAdded(DiscordMember member)
        {
            DiscordButtonComponent button = new DiscordButtonComponent(
                ButtonStyle.Success,
                "test1",
                "Test Button",
                false,
                new DiscordComponentEmoji(":rofl:"));

            DiscordButtonComponent button2 = new(ButtonStyle.Primary,
                "test2",
                "Test Button 2",
                false,
                new DiscordComponentEmoji(":calendar:"));
            
            DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder();
            messageBuilder.AddComponents(button, button2);

            var message = await Bot.Instance.MainGuild.Channels[WELCOME_CHANNEL_ID]
                .SendMessageAsync(messageBuilder);

            var buttonInteraction = 
                await Bot.Interactivity.WaitForButtonAsync(message, new[] {button, button2}, TimeSpan.FromMinutes(5));
            
        }
    }
}