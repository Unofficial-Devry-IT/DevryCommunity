using System;
using System.Collections.Generic;
using System.Linq;
using DisCatSharp.Entities;

namespace DevryBot.Discord.Extensions
{
    public static class StringExtensions
    {
        public static string ToWelcomeMessage(this string text, IEnumerable<DiscordMember> users, DiscordChannel channel)
        {
            string[] words = text.Split(" ");

            for (int i = 0; i < words.Length; i++)
            {
                bool endsWithPeriod = words[i].EndsWith(".");
                bool endsWithComma = words[i].EndsWith(",");
                bool modified = false;
                
                // we do not want to include a period, or comma as it may skew our search. We also don't want to include the 1st character
                int length = words[i]
                    .Replace(".", "")
                    .Replace(",", "").Length - 1;
                
                // if the words starts with a # symbol we want to "mention" a channel
                if (words[i].StartsWith("#"))
                {
                    DiscordChannel item = channel.Guild.Channels
                        .FirstOrDefault(x =>
                            x.Value.Name.Equals(words[i].Substring(1, length), StringComparison.OrdinalIgnoreCase))
                        .Value;

                    if (item != null)
                    {
                        words[i] = channel.Mention;
                        modified = true;
                    }
                }
                else if (words[i].StartsWith("@"))
                {
                    DiscordRole role = channel.Guild.Roles
                        .FirstOrDefault(x => x.Value.Name.ToLower().Contains(words[i].Substring(1, length).ToLower()))
                        .Value;

                    if (role != null)
                    {
                        words[i] = role.Mention;
                        modified = true;
                    }
                }
                else if (words[i].Contains("[USERS]"))
                {
                    words[i] = string.Join(", ", users.Select(x => x.Mention));
                    modified = true;
                }
                
                // If we modified the word to be a mention instead
                // we need to remember to add the period/comma back
                if (modified)
                {
                    if (endsWithComma)
                        words[i] += " ,";
                    else if (endsWithPeriod)
                        words[i] += " .";
                }
            }

            // Combine the message
            return string.Join(" ", words);
        }
    }
}