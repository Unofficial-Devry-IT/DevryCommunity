using System;
using System.Collections.Generic;
using UnofficialDevryIT.Architecture.Models;

namespace DevryDomain.Models
{
    public class Challenge : EntityWithTypedId<ulong>
    {
        public string Question { get; set; }
        public string Title { get; set; }
        public ulong GamificationCategoryId { get; set; }
        public List<ChallengeResponse> Responses { get; set; } = new();
        public bool IsActive { get; set; } = false;
        public string Explanation { get; set; }

        /// <summary>
        /// Discord Message ID associated with the challenge (persistence)
        /// </summary>
        public ulong DiscordMessageId { get; set; }

        public override string ToString()
        {
            return $"Question: {Question}\n" +
                   $"Title: {Title}\n" +
                   $"Explanation: {Explanation}\n" +
                   $"IsActive: {IsActive}\n";
        }
    }
}