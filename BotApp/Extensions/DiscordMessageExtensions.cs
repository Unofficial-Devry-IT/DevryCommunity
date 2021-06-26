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
                // If the word starts with a # symbol we want to "mention" a channel!
                if (words[i].StartsWith("#"))
                {
                    // We do NOT want to inclue a period as it may skew our search. We also don't want the first character (#)
                    int length = words[i].Replace(".", "").Length - 1;
                    
                    DiscordChannel channel = guild.Channels.FirstOrDefault(x =>
                        x.Value.Name.Equals(words[i].Substring(1, length), StringComparison.OrdinalIgnoreCase)).Value;
                    
                    // If the channel was valid -- we replace the word with the designated mention 
                    if (channel != null)
                        words[i] = channel.Mention;
                }
                
                // We want to mention some sort of role
                else if (words[i].StartsWith("@"))
                {
                    int length = words[i].Replace(".", "").Length - 1;
                    
                    DiscordRole role = guild.Roles
                        .FirstOrDefault(x => x.Value.Name.ToLower().Contains(words[i].Substring(1, length).ToLower())).Value;

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