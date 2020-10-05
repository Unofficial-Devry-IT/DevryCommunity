using DevryServiceBot.Util;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DevryServiceBot.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DevryServiceBot.Wizards
{
    [WizardInfo(CommandName = "",
                Description = "Community Interactivity with Polling",
                Name = "Interrogator Hat",
                IgnoreHelpWizard = true,
                IconUrl = "data:image/jpeg;base64,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")]
    public class QuestionaireWizard : Wizard
    {
        public TimeSpan DefaultTimespan = TimeSpan.FromSeconds(60);
        DateTime EndTime;

        const string multipleChoicePrefix = "regional_indicator_";
        DiscordMessage PrimaryMessage;
        Questionaire question;

        /// <summary>
        /// List of answers that pertain to this poll
        /// </summary>
        Dictionary<string, ReactionAnswer> ReactionAnswers = new Dictionary<string, ReactionAnswer>();

        /// <summary>
        /// Cache of participant answers that shall be tallied at the end of poll
        /// </summary>
        Dictionary<ulong, DiscordEmoji> ParticipantReactionAnswers = new Dictionary<ulong, DiscordEmoji>();

        /// <summary>
        /// Cache of participant answers (in messages) that shall be tallied at the end of the poll
        /// </summary>
        Dictionary<ulong, DiscordMessage> ParticipantMessageAnswers = new Dictionary<ulong, DiscordMessage>();

        public QuestionaireWizard(ulong userId, DiscordChannel channel) : base(userId, channel){}

        void InitializeReactionAnswers()
        {
            if (this.question.Type != QuestionaireType.Reaction) return;

            List<ReactionAnswer> answers = JsonConvert.DeserializeObject<List<ReactionAnswer>>(question.JSON);
            Random random = new Random();
            int ascii_a = 97;

            while(answers.Count > 0)
            {
                int index = random.Next(0, answers.Count);
                var item = answers[index];
                answers.RemoveAt(index);

                char letter = (char)(ascii_a + ReactionAnswers.Count);
                string emoji = $":{multipleChoicePrefix}{letter}:";

                ReactionAnswers.Add(emoji, item);
            }
        }

        public override async Task StartWizard(CommandContext context)
        {
            using (var database = new DevryDbContext())
            {
                Random random = new Random();
                if(database.Questions.Any(x=>!x.LastUsed.HasValue))
                {
                    var pool = await database.Questions.Where(x => !x.LastUsed.HasValue).ToListAsync();
                    int index = random.Next(0, pool.Count);
                    question = pool[index];

                    //question.LastUsed = DateTime.Now;
                    //database.Entry(question).State = EntityState.Modified;
                    //await database.SaveChangesAsync();
                }
                else
                    this.question = await database.Questions
                                                .OrderByDescending(x => x.LastUsed)
                                                .FirstAsync();   
            }

            InitializeReactionAnswers();
            string contents = question.Description + "\n";
            
            foreach(string emoji in ReactionAnswers.Keys)
                contents += $"{emoji} - {ReactionAnswers[emoji].Text}\n\n";

            EndTime = DateTime.Now.AddMinutes(5);

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithTitle(question.Title)
                .WithDescription(contents)
                .WithColor(DiscordColor.Purple)
                .WithFooter((question.Type == QuestionaireType.Reaction ? "React to the appropriate emoji to answer" : "Mention this bot, along with your answer") + $"\n\nCloses at {EndTime.ToString("MM/dd/yyyy HH:mm:ss")}");
            
            // Used for tracking purposes
            PrimaryMessage = await context.RespondAsync(embed: builder.Build());

            foreach (var emoji in this.ReactionAnswers.Keys)
            {
                await Task.Delay(500);
                await PrimaryMessage.CreateReactionAsync(DiscordEmoji.FromName(Bot.Discord,emoji));
            }

            

            switch(question.Type)
            {
                case QuestionaireType.Message:
                    this.AcceptAnyUser = true;
                    this.MessageRequireMention = true;
                    await LongPollWithMessages(context);
                    break;
                case QuestionaireType.Reaction:
                    await LongPollWithReactions(context);
                    break;
                default:
                    Console.WriteLine($"Questionaire Type was invalid...");
                    break;
            }
        }

        public override async Task LongPollWithMessages(CommandContext context)
        {
            HandleEvents();

            while (DateTime.Now < EndTime)
                await Task.Delay(1000);

            //HandleEvents(true);

            await FinalizePoll(context);
        }

        async Task UserReact(DiscordMessage message, DiscordUser user, DiscordEmoji emoji)
        {
            // Must be the message pertaining to THIS poll
            if (message.Id != this.PrimaryMessage.Id) 
                return;

#if DEBUG
            Debug.WriteLine($"{user.Username} reacted with {emoji.Name}");
#endif
            string name = emoji.GetDiscordName();

            if (!ReactionAnswers.ContainsKey(name))
                return;

            // Update the user's answer
            if (ParticipantReactionAnswers.ContainsKey(user.Id))
                ParticipantReactionAnswers.Add(user.Id, emoji);
            else
                ParticipantReactionAnswers[user.Id] = emoji;
        }

        async Task UserReactRemoved(DiscordMessage message, DiscordUser user, DiscordEmoji emoji)
        {
            if (message.Id != this.PrimaryMessage.Id)
                return;
#if DEBUG
            Debug.WriteLine($"{user.Username} has removed their reaction: {emoji.GetDiscordName()}");
#endif

            // update their response
            if (ParticipantReactionAnswers.ContainsKey(user.Id))
                ParticipantReactionAnswers.Remove(user.Id);
        }

        async Task UserReply(DiscordMessage message, DiscordUser user)
        {
            if (message.ChannelId != Channel.Id)
                return;

            // Does the message meet requirements?
            if (!ResponsePredicate(message))
                return;

            if (ParticipantMessageAnswers.ContainsKey(user.Id))
                ParticipantMessageAnswers[user.Id] = message;
            else
                ParticipantMessageAnswers.Add(user.Id, message);

            WizardMessages.Add(message);
        }

        void HandleEvents(bool remove=false)
        {
            switch(this.question.Type)
            {
                case QuestionaireType.Message:
                    if (remove)
                        Handler.Instance.OnUserReply -= this.UserReply;
                    else
                        Handler.Instance.OnUserReply += this.UserReply;
                    break;
                case QuestionaireType.Reaction:
                    if(remove)
                    {
                        Handler.Instance.OnUserReact -= this.UserReact;
                        Handler.Instance.OnUserReactRemoved -= this.UserReactRemoved;
                    }
                    else
                    {
                        Handler.Instance.OnUserReact += this.UserReact;
                        Handler.Instance.OnUserReactRemoved += this.UserReactRemoved;
                    }
                    break;
            }
        }

        public override async Task LongPollWithReactions(CommandContext context)
        {
            HandleEvents();
            Debug.WriteLine($"End time should be: {EndTime}");
            
            while(DateTime.Now < EndTime)
                await Task.Delay(1000);
            
            Debug.WriteLine("Poll should be closing");
            //HandleEvents(true);

            await FinalizePoll(context);
        }

        List<DiscordEmbed> Paginate(Dictionary<double, List<DiscordMember>> awards)
        {
            List<DiscordEmbed> embeds = new List<DiscordEmbed>();

            foreach(var pair in awards)
            {
                DiscordEmbedBuilder pageBuilder = new DiscordEmbedBuilder()
                    .WithTitle("Awards")
                    .WithDescription("The following were awarded\n\n")
                    .WithColor(DiscordColor.Yellow);

                pageBuilder.AddField($"{pair.Key} points", string.Join(", ", pair.Value.Select(x => x.Username)));

                embeds.Add(pageBuilder.Build());
            }

            return embeds;
        }

        public override async Task FinalizePoll(CommandContext context)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithTitle(question.Title)
                .WithDescription(question.AnswerDescription)
                .WithColor(DiscordColor.Yellow);

            await PrimaryMessage.ModifyAsync(embed: builder.Build());
            Dictionary<DiscordMember, double> awards = new Dictionary<DiscordMember, double>();

            using (var database = new DevryDbContext())
            {
                switch (question.Type)
                {
                    case QuestionaireType.Reaction:
                        foreach (var pair in ParticipantReactionAnswers)
                        {
                            string name = pair.Value.GetDiscordName();

                            if (!ReactionAnswers.ContainsKey(name))
                                continue;

                            var reaction = ReactionAnswers[name];
                            if (reaction.IsCorrect || reaction.PointsForAnswer > 0)
                            {
                                var member = await Channel.Guild.GetMemberAsync(pair.Key);
                                awards.Add(member, reaction.PointsForAnswer);

                                var stats = await database.Stats.FindAsync(member.Id);
                                if (stats == null)
                                {
                                    stats = new MemberStats
                                    {
                                        Id = member.Id,
                                        Points = reaction.PointsForAnswer
                                    };
                                    database.Stats.Add(stats);
                                }
                                else
                                {
                                    stats.Points += reaction.PointsForAnswer;
                                    database.Entry(stats).State = EntityState.Modified;
                                }

                                await database.SaveChangesAsync();
                            }
                        }
                        break;


                    case QuestionaireType.Message:
                        MessageAnswer messageAnswer = JsonConvert.DeserializeObject<MessageAnswer>(question.JSON);
                        Regex regex = new Regex($@"\b({string.Join("|", messageAnswer.CorrectPhrases)})\b");

                        foreach (var reply in this.ParticipantMessageAnswers)
                        {
                            var matches = regex.Matches(reply.Value.Content)
                                                .Distinct();

                            if (matches.Count() <= 0)
                                continue;

                            var stats = await database.Stats.FindAsync(reply.Key);
                            double points = 0;
                            if (matches.Count() < messageAnswer.CorrectPhrases.Count && messageAnswer.AllowPartialCredit)
                                points = ((double)matches.Count() / (double)messageAnswer.CorrectPhrases.Count) * (double)messageAnswer.CorrectPhrases.Count;
                            else if (matches.Count() >= messageAnswer.CorrectPhrases.Count)
                                points = messageAnswer.PointsForAnswer;

                            awards.Add(await context.Guild.GetMemberAsync(reply.Key), points);

                            if(stats == null)
                            {
                                stats = new MemberStats
                                {
                                    Id = reply.Key,
                                    Points = points
                                };
                                database.Stats.Add(stats);
                            }
                            else
                            {
                                stats.Points += points;
                                database.Entry(stats).State = EntityState.Modified;
                            }

                            await database.SaveChangesAsync();
                        }
                        break;
                }

                // Update the question's last used
                question.LastUsed = DateTime.Now;
                database.Entry(question).State = EntityState.Modified;
                await database.SaveChangesAsync();
            }

            var groups = awards
                .GroupBy(x => x.Value)
                .OrderBy(x=>x.Key)
                .ToDictionary(x => x.Key, x =>x.Select(y=>y.Key).ToList());

            // Cleanup all messages associated with this wizard
            await Cleanup();

            List<DiscordEmbed> pages = Paginate(groups);

            foreach(var page in pages)
            {
                await Task.Delay(500);
                await context.RespondAsync(embed: page);
            }
        }
    }
}
