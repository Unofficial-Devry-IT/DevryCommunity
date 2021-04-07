using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Extensions;
using Domain.Enums;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace BotApp.Interaction.Roles
{
    public class JoinRoleInteraction : InteractionBase
    {
        public JoinRoleInteraction(CommandContext context) : base(context)
        {
        }
        private string[] GetBlacklistedRoles()
        {
            try
            {
                JsonDocument doc = JsonDocument.Parse(CurrentConfig.ExtensionData);
                JsonElement element = doc.RootElement.GetProperty("BlacklistedRoles");

                return element.EnumerateArray()
                    .Select(x => x.GetString())
                    .ToArray();
            }
            catch (Exception e)
            {
                Bot.Instance.Logger.LogError(e, $"Was unable to retrieve blacklisted roles from {InteractionName()}");
                return new string[] { };
            }
        }

        /// <summary>
        /// Determines which character should be used to dictate a role being ignored or not
        /// </summary>
        /// <returns></returns>
        private string IgnoreRolesWithCharacter()
        {
            try
            {
                JsonDocument doc = JsonDocument.Parse(CurrentConfig.ExtensionData);
                JsonElement element = doc.RootElement.GetProperty("IgnoreRolesWithCharacter");

                return element.GetString();
            }
            catch
            {
                return "^";
            }
        }

        /// <summary>
        /// Simply aggregating all the lists of roles to be 1 dictionary
        /// where the key value is the "index" value linking to said role
        /// makes it easy for tracking between multiple pages of
        /// roles
        /// </summary>
        /// <param name="courses"></param>
        /// <returns></returns>
        Dictionary<int, DiscordRole> GenerateRoleMap(Dictionary<string, List<DiscordRole>> courses)
        {
            int currentCount = 0;
            Dictionary<int, DiscordRole> map = new Dictionary<int, DiscordRole>();

            foreach (string key in courses.Keys)
                foreach (var item in courses[key])
                {
                    map.Add(currentCount, item);
                    currentCount++;
                }

            return map;
        }
        
        /*
         * There are multiple stages within this join role interaction
         *  1.) Parse all the course types by categories (CEIS, CIS, etc) --> then display to user
         *      1a) Retrieve user input on which course type they're looking for
         *  2.) Based on user input --> display the roles that pertain to their selection
         *      2a) Retrieve user input on which course type they're looking for
         *  3.) Add user to the role(s) they picked
         */
        
        protected override async Task ExecuteAsync()
        {
            // Roles users cannot join via the bot (lowercased to simplify comparisons
            string[] blacklistedRoles = GetBlacklistedRoles()
                .Select(x=>x.ToLower())
                .ToArray();

            // Make it appear as if the bot is 'doing' something
            await Context.TriggerTypingAsync();

            string ignoreCharacter = IgnoreRolesWithCharacter(); // based on configuration (IgnoreRolesWithCharacter)
            var roles = Context.Guild.Roles
                .Where(x => !blacklistedRoles.Contains(x.Value.Name.ToLower()) && !x.Value.Name.Contains(ignoreCharacter))
                .OrderBy(x => x.Value.Name)
                .Select(x => x.Value)
                .ToList();

            /*
             *  Based on the discord environment we have created
             *  Classes follow the pattern of
             *  COURSE_CATEGORY COURSE_NUMBER COURSE_TITLE
             *  --> So if we were to split everything via spaces (some might have dashes in place of spaces)
             *  - we just need the first item
             */
            
            List<string> courseTypes = roles.Select(x => x.Name.Trim().Replace("-", " ").Split(" ").First())
                .Distinct()
                .ToList();

            
            // Generate the messages required to showcase all the options
            var builders = CurrentConfig
                .BuildEmbed(Context.User.Username)
                .WithDescription("Please select the course(s) you're in. If you want to join more than 1, separate your choices with commas!")
                .AddFields(courseTypes.ToOptions(false));

            // Get the user's response to course types
            string responseString = await RetrieveData<string>(builders, true);
            
            // Ensure the user provided valid data
            if (string.IsNullOrEmpty(responseString.Trim()))
                throw new Exception("Invalid Course Type -- Cannot be null or empty");

            string[] parameters = responseString
                .Trim()
                .Replace(",", " ")
                .Split(" ");

            Dictionary<string, List<DiscordRole>> selectedGroups = new Dictionary<string, List<DiscordRole>>();

            foreach (var selection in parameters)
            {
                if (int.TryParse(selection, out int index))
                {
                    if (index < 0 || index >= courseTypes.Count)
                        throw new Exception($"Invalid Input Provided - Only numeric values are allowed");
                    
                    selectedGroups.Add(courseTypes[index], roles.Where(x=>x.Name.ToLower().StartsWith(courseTypes[index].ToLower())).ToList());
                }
            }

            // Used to keep the numbers across multiple pages aligned. (1-24 on page 1) then (25-30 on page 2)
            int optionCount = 0;

            // STAGE 2 -- have the user select from the courses
            foreach (var key in selectedGroups.Keys)
            {
                var embeds = CurrentConfig
                    .BuildEmbed(Context.User.Username)
                    .AddFields(selectedGroups[key]
                        .Select(x=>x.Name)
                        .ToList()
                        .ToOptions(optionCount));

                optionCount += selectedGroups[key].Count;
                
                // Send all the embeds that are necessary for this group
                foreach (var item in embeds)
                {
                    // Make sure we are tracking each message
                    var message = await Context.RespondAsync(item.Build());
                    TrackedMessages.Add(message);
                }
            }
            
            Dictionary<int, DiscordRole> roleMap = GenerateRoleMap(selectedGroups);

            responseString = await RetrieveData<string>(CurrentConfig.BuildEmbed()
                    .WithDescription(
                        "Please select the class(es) you're in. If you want to join more than 1, separate your choices with commas!"),
                    true);

            if (string.IsNullOrEmpty(responseString))
                throw new Exception($"Invalid Input was provided. Cannot be null/empty");
            
            parameters = responseString.Replace(",", " ").Split(" ");
            
            // Roles that were applied to the user
            List<DiscordRole> appliedRoles = new List<DiscordRole>();

            // Make it look like the bot is still doing something
            await Context.TriggerTypingAsync();
            
            // Go through each user selection and apply the necessary roles
            foreach (string selection in parameters)
            {
                if (int.TryParse(selection, out int index))
                {
                    /*
                     * We don't have to throw an exception if the user input was off
                     * Rather - notify the user something was wrong, exit out of continuing
                     * Then allow the last part of this to say "these changes were applied"
                     * or - "no changes were applied"
                     */
                    
                    if (index < 0 || index > optionCount)
                    {
                        await Context.ReplyWithStatus(StatusCode.ERROR,
                            $"Invalid Input. Expected a value between 0 and {optionCount}. You provided {selection}",
                            Context.User.Username);
                        break;
                    }

                    await Context.Member.GrantRoleAsync(roleMap[index],
                        $"{Context.User.Username} requested access to {roleMap[index].Name} role");
                    appliedRoles.Add(roleMap[index]);
                    
                    await Task.Delay(1000); // must delay this otherwise discord thinks we're spamming it
                }
            }
            
            // Inform the users the changes (if any) that were made -- utilizing the status codes for visualization
            if (appliedRoles.Count > 0)
                await Context.ReplyWithStatus(StatusCode.SUCCESS,
                    $"Hey {Context.Member.Username}, the following roles were applied: \n{string.Join(", ", appliedRoles)}",
                    Context.User.Username);
            else
                await Context.ReplyWithStatus(StatusCode.WARNING,
                    $"Hey, {Context.User.Username}, no changes were applied",
                    Context.User.Username);
        }
    }
}