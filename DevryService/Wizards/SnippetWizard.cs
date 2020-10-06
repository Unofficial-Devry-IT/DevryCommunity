using DevryService.Core;
using DevryService.Core.Util;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Wizards
{
    [WizardInfo(Name = "Programming Hat",
        Description = "Provides a variety of code snippets for various topics and in multiple languages",
        Emoji = ":desktop:",
        Title = "Snippets")]
    public class SnippetWizard : Wizard
    {
        List<string> Categories = LoadFile.GetSnippetCategories();
        const string Basic = "Snippet - Wizard. Please following the instructions below\n";
        DiscordMessage WizardMessage;

        public SnippetWizard(ulong userId, DiscordChannel channel) : base(userId, channel)
        {
        }

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

        async Task SelectLanguage(string selectedCategory)
        {
            var files = LoadFile.GetFilesInCategory(selectedCategory);
            var extensions = files.Select(x => x.Extension)
                .Distinct()
                .ToList();

            var groups = files.GroupBy(x => x.Extension)
                .ToDictionary(x => x.Key, x => x.ToList());

            List<string> options = new List<string>();
            foreach (var ext in extensions)
                options.Add(LoadFile.ExtensionToName(ext));

            string languageMenu = Basic + "Select the corresponding number(s) to select a language, or languages\n\n";
            for (int i = 0; i < options.Count; i++)
                languageMenu += $"[{i + 1}] - {options[i]}\n";

            WizardMessage = await WizardReplyEdit(WizardMessage, languageMenu, false);
            DiscordMessage reply = await GetUserReply();

            List<string> selectedLanguages = new List<string>();
            foreach(var parameter in reply.Content.Replace(","," ").Split(" "))
            {
                if(int.TryParse(parameter, out int index))
                {
                    index -= 1;
                    if (index < 0 || index > options.Count)
                        continue;
                    else
                        selectedLanguages.Add(options[index]);
                }
            }

            await Cleanup();

            foreach (var lang in selectedLanguages)
                await DisplayCode(lang, groups[LoadFile.NameToExtension(lang)]);
        }

        async Task DisplayCode(string lang, List<FileInfo> files)
        {
            foreach(var file in files)
            {
                await Task.Delay(2500);
                string block = await LoadFile.CreateCodeBlock(file);

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                    .WithAuthor(AuthorName, null, AuthorIcon)
                    .WithTitle(lang)
                    .WithDescription(block)
                    .WithColor(LoadFile.GetColor(lang));

                await Channel.SendMessageAsync(embed: builder.Build());
            }
        }

        public override async Task StartWizard(CommandContext context)
        {
            string categoryMenu = Basic + "Select the corresponding number to selct a category\n\n";

            for (int i = 0; i < Categories.Count; i++)
                categoryMenu += $"[{i + 1}] - {Categories[i]}\n";

            WizardMessage = await WizardReply(context, categoryMenu, true);
            DiscordMessage reply = await GetUserReply();

            // Prevent execution to go further if user attempted to halt wizard
            if(reply == null || reply.Content == null)
            {
                await Cleanup();
                return;
            }

            string category = GetCategory(reply.Content);

            if(string.IsNullOrEmpty(category))
            {
                await Cleanup();
                await context.RespondAsync(embed: new DiscordEmbedBuilder()
                    .WithAuthor(AuthorName, null, AuthorIcon)
                    .WithTitle("Invalid Input")
                    .WithDescription($"Expected value between 1 - {Categories.Count}")
                    .WithColor(DiscordColor.IndianRed)
                    .Build());
                return;
            }

            await SelectLanguage(category);
        }
    }
}
