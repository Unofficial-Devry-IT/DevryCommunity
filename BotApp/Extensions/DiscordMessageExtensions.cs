using System;
using System.Linq;
using DSharpPlus.Entities;

namespace BotApp.Extensions
{
    public static class DiscordMessageExtensions
    {
        public static string ToDiscordMessage(this string text, DiscordGuild guild, DiscordMember member = null)
        {
            string[] words = text.Split(" ");

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].StartsWith("#"))
                {
                    DiscordChannel channel = guild.Channels.FirstOrDefault(x =>
                        x.Value.Name.Equals(words[i].Substring(1), StringComparison.OrdinalIgnoreCase)).Value;

                    if (channel != null)
                        words[i] = channel.Mention;
                }
                else if (words[i].StartsWith("@"))
                {
                    DiscordRole role = guild.Roles
                        .FirstOrDefault(x => x.Value.Name.ToLower().Contains(words[i].Substring(1).ToLower())).Value;

                    if (role != null)
                        words[i] = role.Mention;
                }
                else if (words[i].Contains("[USER]"))
                {
                    if (member != null)
                        words[i] = words[i].Replace("[USER]", member.DisplayName);
                }
            }
            
            return string.Join(" ", words);
        }
    }
}