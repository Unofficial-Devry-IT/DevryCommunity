using System;
using System.Linq;
using System.Threading.Tasks;
using DevryBot.Options;
using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.SlashCommands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevryBot.Discord.Extensions
{
    public static class InteractionExtensions
    {

        /// <summary>
        /// Ensure the the interaction context contains a valid guild
        /// - Ran on slash commands
        /// </summary>
        /// <param name="context"></param>
        /// <returns>True if the guild is valid</returns>
        public static async Task<bool> ValidateGuild(this InteractionContext context)
        {
            if (context.Guild != null) 
                return true;

            DiscordInteractionResponseBuilder discordInteractionResponseBuilder = new()
            {
                Content = "Error: This is a guild command!",

                // Make all errors visible to just the user, makes the channel more clean for everyone else.
                IsEphemeral = true
            };

            // send the response
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                discordInteractionResponseBuilder);

            return false;
        }

        /// <summary>
        /// Send user a timeout message because they took too long
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        public static async Task SendTimeout(this InteractionContext context, string message)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                .WithTitle("You took too long")
                .WithDescription(message)
                .WithTimestamp(DateTime.Now);

            await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
        }
        
        /// <summary>
        /// Retrieve boolean value from user
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <param name="bot"></param>
        /// <returns></returns>
        public static async Task<bool> YesNo(this InteractionContext context, string message, IBot bot)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                .WithTitle("Require Input")
                .WithDescription(message)
                .WithColor(DiscordColor.Cyan)
                .WithFooter("Please click the button below");

            string yesId = $"{context.Member.Id}_yes";
            string noId = $"{context.Member.Id}_no";

            DiscordButtonComponent yesButton =
                new DiscordButtonComponent(ButtonStyle.Success, yesId, "Yes", false, null);
            DiscordButtonComponent noButton = new DiscordButtonComponent(ButtonStyle.Danger, noId, "No", false,null);

            DiscordWebhookBuilder responseBuilder = new();
            
            responseBuilder.AddEmbed(embed.Build());
            responseBuilder.AddComponents(yesButton, noButton);

            var response = await context.EditResponseAsync(responseBuilder);
            var interaction =
                await bot.Interactivity.WaitForButtonAsync(response, new[] { yesButton, noButton },
                    TimeSpan.FromMinutes(5));

            await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Acknowledged"));
            
            if (interaction.TimedOut)
            {
                await context.SendTimeout("You took too long to answer my question");
                return false;
            }

            return interaction.Result.Id.ToLower().EndsWith("yes");
        }
        
        /// <summary>
        /// Retrieve value from user
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <param name="bot"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> GetValue<T>(this InteractionContext context, string message, IBot bot)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                .WithTitle("Require Input")
                .WithColor(DiscordColor.Cyan)
                .WithFooter("Please response like a normal message")
                .WithDescription(message);

            DiscordEmbedBuilder errorEmbed = new DiscordEmbedBuilder()
                .WithTitle("Invalid Input")
                .WithColor(DiscordColor.Red)
                .WithFooter("Please response with the right stuff this time");

            bool valid = true;
            string errorMessage = "";
            T value = default;
            
            do
            {
                try
                {
                    DiscordWebhookBuilder responseBuilder = new();
                    responseBuilder.AddEmbed(string.IsNullOrEmpty(errorMessage) ? embed : errorEmbed);
                    await context.EditResponseAsync(responseBuilder);

                    var nextMessage = await bot.Interactivity.WaitForMessageAsync(x =>
                        x.Author.Id == context.User.Id && x.ChannelId == context.Channel.Id, TimeSpan.FromMinutes(5));

                    if (nextMessage.TimedOut)
                    {
                        await nextMessage.Result.DeleteAsync();
                        return default;
                    }
                    
                    value = (T) Convert.ChangeType(nextMessage.Result.Content, typeof(T));
                    await nextMessage.Result.DeleteAsync();
                }
                catch (Exception ex)
                {
                    valid = false;
                    errorMessage = $"Invalid input. Require something that can convert into a {typeof(T).Name}";
                }
                
            } while (!valid);

            return value;
        }

        /// <summary>
        /// Update the interaction component to a 'thinking state'
        /// This provides visual feedback that the user is waiting on the server
        /// to do something
        /// </summary>
        /// <param name="context"></param>
        public static async Task ImThinking(this InteractionContext context)
        {
            // InteractionResponseType.DeferredChannelMessageWithSource let's the user know that we got the command,
            // and that the bot is "thinking" -- or really just taking a really long time
            await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new()
            {
                IsEphemeral = true
            });
        }

    }
}