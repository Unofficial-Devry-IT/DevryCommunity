using DevryServiceBot.Util;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryServiceBot.Wizards
{
    [WizardInfo(Name = "Programming Hat", 
                Description = "Provides a variety of code snippets for various topics and in multiple languages",
                ReactionEmoji = ":desktop:",
                CommandName = "SnippetCommand.Snippet")]
    public class SnippetWizard : Wizard
    {
        List<string> Categories = LoadSnippet.GetSnippetCategories();
        const string Basic = "Snippet - Wizard. Please following the instructions below\n";

        public SnippetWizard(CommandContext context) : base(context.User.Id, context.Channel) { }

        /// <summary>
        /// Get the category from reply content
        /// </summary>
        /// <param name="replyContent"></param>
        /// <returns></returns>
        string GetCategory(string replyContent)
        {
            if(int.TryParse(replyContent, out int index))
            {
                index -= 1;
                if (index < 0 || index > this.Categories.Count)
                    return null;
                else
                    return Categories[index];
            }

            return null;
        }

        /// <summary>
        /// Prompt User to select languages based off category.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="selectedCategory"></param>
        /// <returns></returns>
        async Task SelectLanguages(DiscordMessage message, string selectedCategory)
        {
            var files = LoadSnippet.GetFilesInCategory(selectedCategory);
            var extensions = files.Select(x => x.Extension)
                                    .Distinct()
                                    .ToList();
            var groups = files
                             .GroupBy(x => x.Extension)
                             .ToDictionary(x => x.Key, x => x.ToList());

            List<string> options = new List<string>();
            foreach (var ext in extensions)
                options.Add(LoadSnippet.ExtensionToName(ext));

            string languageMenu = Basic + "Select the corresponding number(s) to select a language, or languages\n\n";
            for (int i = 0; i < options.Count; i++)
                languageMenu += $"[{i + 1}] - {options[i]}\n";

            message = await WizardReplyEdit(message, languageMenu, false);

            DiscordMessage reply = await GetUserReply();

            List<string> selectedLanaguges = new List<string>();
            foreach(var parameter in reply.Content.Replace(","," ").Split(" "))
            {
                if(int.TryParse(parameter, out int index))
                {
                    index -= 1;
                    if (index < 0 || index > options.Count)
                        continue;
                    else
                        selectedLanaguges.Add(options[index]);
                }
            }

            await Cleanup();

            foreach (var lang in selectedLanaguges)
                await DisplayCode(message.Channel, lang, groups[LoadSnippet.NameToExtension(lang)]);
        }

        /// <summary>
        /// Display Code Blocks for each file in <paramref name="files"/>
        /// </summary>
        /// <param name="channel">Discord Channel</param>
        /// <param name="lang">Language</param>
        /// <param name="files">Code Files</param>
        /// <returns></returns>
        async Task DisplayCode(DiscordChannel channel, string lang, List<FileInfo> files)
        {
            foreach(var file in files)
            {
                await Task.Delay(2500);
                string block = await LoadSnippet.CreateCodeBlock(file);

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                    .WithAuthor(AuthorName, null, AuthorIcon)
                    .WithTitle(lang)
                    .WithDescription(block)
                    .WithColor(LoadSnippet.GetColor(lang));

                await channel.SendMessageAsync(embed: builder.Build());
            }
        }

        public override async Task StartWizard(CommandContext context)
        {
            var categories = LoadSnippet.GetSnippetCategories();

            string categoryMenu = Basic + "Select the corresponding number to select a category\n\n";
            for (int i = 0; i < categories.Count; i++)
                categoryMenu += $"[{i + 1}] - {categories[i]}\n";

            DiscordMessage message = await WizardReply(context, categoryMenu, true);

            DiscordMessage reply = await GetUserReply();
            string category = GetCategory(reply.Content);

            if(string.IsNullOrEmpty(category))
            {
                await Cleanup();
                await context.RespondAsync(embed: new DiscordEmbedBuilder()
                    .WithAuthor(AuthorName, null, AuthorIcon)
                    .WithTitle($"Invalid Input")
                    .WithDescription($"Expected value between 1 - {categories.Count}")
                    .WithColor(DiscordColor.IndianRed)
                    .Build());
                return;
            }

            await SelectLanguages(message, category);
        }
    }
}
