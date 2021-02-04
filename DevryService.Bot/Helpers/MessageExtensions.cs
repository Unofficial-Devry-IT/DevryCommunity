using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Bot.Helpers
{
    public static class MessageExtensions
    {
        public static string ToDiscordMessage(this string text, DiscordMember member = null)
        {
            string[] words = text.Split(" ");

            for(int i = 0; i < words.Length; i++)
            {
                if(words[i].StartsWith("#"))
                {
                    DiscordChannel channel = Bot.Instance.MainGuild.Channels.FirstOrDefault(x => x.Value.Name.Equals(words[i].Substring(1), StringComparison.OrdinalIgnoreCase)).Value;

                    if (channel != null)
                        words[i] = channel.Mention;
                }
                else if(words[i].StartsWith("@"))
                {
                    DiscordRole role = Bot.Instance.MainGuild.Roles.FirstOrDefault(x => x.Value.Name.ToLower().Contains(words[i].Substring(1).ToLower())).Value;

                    if (role != null)
                        words[i] = role.Mention;
                }
                else if(words[i].Contains("[USER]"))
                {
                    if(member != null)
                        words[i] = words[i].Replace("[USER]", member.DisplayName);
                }
            }

            return string.Join(" ", words);
        }
    }
}
