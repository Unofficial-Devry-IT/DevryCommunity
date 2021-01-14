using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Core.Util
{
    public static class DiscordMarkdown
    {
        public static string GetText(CommandContext context, string plainText)
        {
            string[] words = plainText.Split(" ");

            for(int i = 0; i < words.Length; i++)
            {
                if(words[i].StartsWith("#"))
                {
                    try
                    {
                        DiscordChannel channel = context.Guild.Channels.FirstOrDefault(x => x.Value.Name.ToLower().Contains(words[i].ToLower().Substring(1))).Value;
                        if (channel != null)
                            words[i] = channel.Mention;
                    }
                    catch { }
                }
                else if(words[i].StartsWith("@"))
                {
                    DiscordRole role = context.Guild.Roles.Where(x => x.Value.Name.ToLower().Contains(words[i].Substring(1).ToLower()))
                        .FirstOrDefault().Value;

                    if (role != null)
                        words[i] = role.Mention;
                }
                else if(words[i].Contains("[USER]"))
                {
                    words[i] = words[i].Replace("[USER]", context.Member.Mention);
                }
            }

            return string.Join(" ", words);
        }
    }
}
