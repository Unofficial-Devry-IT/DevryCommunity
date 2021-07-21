using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryInfrastructure;
using DSharpPlusNextGen.Entities;
using DSharpPlusNextGen.SlashCommands;
using Microsoft.Extensions.Logging;
using SnippetAssistant;

namespace DevryBot.Discord.SlashCommands.Snippets
{
    public class CodeReview : SlashCommandModule
    {

        [SlashCommand("code-review", "Automated review of code")]
        public static async Task Command(InteractionContext context)
        {
            if (!await context.ValidateGuild())
                return;

            await context.ImThinking();

            #region Start Process of getting user input
            DiscordWebhookBuilder messageBuilder = new();
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Code Review")
                .WithDescription("Please respond to this message with an attached code file.")
                .WithColor(DiscordColor.Aquamarine);

            messageBuilder.AddEmbed(embedBuilder.Build());
            var inputInquiry = await context.EditResponseAsync(messageBuilder);
            #endregion

            #region Process User Inquiry Response
            var inquiryResponse = await Bot.Interactivity.WaitForMessageAsync(x =>
            {
                if (x.Author != context.User)
                    return false;

                if (x.Attachments.Count == 0)
                    return false;

                return true;
            }, TimeSpan.FromMinutes(5));

            if (inquiryResponse.TimedOut)
            {
                messageBuilder = new();
                embedBuilder.Description = "I'm sorry, either time ran out or you did not attach a file";
                embedBuilder.Color = DiscordColor.Red;
                embedBuilder.ImageUrl = Bot.Instance.Configuration.WarningImage();
                messageBuilder.AddEmbed(embedBuilder.Build());

                await context.EditResponseAsync(messageBuilder);
                return;
            }
            
            DiscordAttachment attachment = inquiryResponse.Result.Attachments.First();

            // Is the given attachment even supported by the snippet assistant yet?
            if (!CodeReviewService.SupportedLanguages.ContainsKey(attachment.FileName.Split(".").Last()))
            {
                messageBuilder = new();
                embedBuilder.Description =
                    "I'm sorry but we do not support that file extension. Currently we support the following: \n\t" +
                    string.Join("\n\t", CodeReviewService.SupportedLanguages.Select(x => x.Key));
                embedBuilder.Color = DiscordColor.Red;
                embedBuilder.ImageUrl = Bot.Instance.Configuration.WarningImage();
                messageBuilder.AddEmbed(embedBuilder.Build());
                await context.EditResponseAsync(messageBuilder);
                return;
            }

            CodeReviewService service = new();
            try
            {
                string attachmentPath = Path.Join(StorageHandler.TemporaryFileStorage, attachment.FileName);
                string language = CodeReviewService.SupportedLanguages[attachment.FileName.Split(".").Last()];
                await service.DownloadFile(attachment.Url, attachmentPath);
                var report = await service.AnalyzeResults(language, attachmentPath);

                string reportFile = Path.Join(StorageHandler.TemporaryFileStorage,
                    string.Join("_", context.User.Username, "report.html"));
                await File.WriteAllTextAsync(reportFile, await report.GenerateReport());
                
                messageBuilder = new();
                embedBuilder = new DiscordEmbedBuilder()
                    .WithTitle("Code Review")
                    .WithDescription("Here are some suggestions on your code!")
                    .WithColor(DiscordColor.Purple);

                using StreamReader reader = new StreamReader(reportFile);
                FileInfo reportInfo = new FileInfo(reportFile);
                messageBuilder.AddFile(reportInfo.Name, File.OpenRead(reportFile));

                messageBuilder.AddEmbed(embedBuilder.Build());

                await context.EditResponseAsync(messageBuilder);
            }
            catch (Exception ex)
            {
                Bot.Instance.Logger.LogError(ex, $"Something happened while processing {attachment.FileName}");
            }
            
            #endregion


        }
    }
}