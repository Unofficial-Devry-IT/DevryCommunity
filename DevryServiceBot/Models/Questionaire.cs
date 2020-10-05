using System;
using System.Collections.Generic;
using System.Linq;

namespace DevryServiceBot.Models
{
    public enum QuestionaireType
    {
        Reaction,
        Message
    }

    public struct ReactionAnswer
    {
        public string Text { get; set; }

        /// <summary>
        /// Is this the correct answer?
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Total points for valid answer
        /// </summary>
        public int PointsForAnswer { get; set; }
    }

    public struct MessageAnswer
    {
        /// <summary>
        /// CSV FORMAT
        /// Regex will be applied to user's message
        /// to determine if they were correct
        /// </summary>
        public string CorrectPhrasesCSV { get; set; }

        /// <summary>
        /// If member gets a few phrases correct, do they get partial points?
        /// </summary>
        public bool AllowPartialCredit { get; set; }

        /// <summary>
        /// Total points for valid answer
        /// </summary>
        public int PointsForAnswer { get; set; }

        public List<string> CorrectPhrases
        {
            get
            {
                if (string.IsNullOrEmpty(CorrectPhrasesCSV)) 
                    return new List<string>();
                else
                    return CorrectPhrasesCSV.Split(",").ToList();
            }
        }
    }

    public class Questionaire
    {
        /// <summary>
        /// Primary Key, uniquely identifiy this question
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; }
        public string Description { get; set; }
        public string AnswerDescription { get; set; }
        public QuestionaireType Type { get; set; }

        /// <summary>
        /// Date this questionaire was last used
        /// </summary>
        public DateTime? LastUsed { get; set; }

        /// <summary>
        /// Total time (in minutes) that this questionaire will be active
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// JSON - Will either be a list of:
        /// <see cref="MessageAnswer"/> if <see cref="QuestionaireType"/> is <see cref="QuestionaireType.Message"/>
        /// <see cref="ReactionAnswer" /> if <see cref="QuestionaireType "/> is <see cref="QuestionaireType.Reaction"/>
        /// </summary>
        public string JSON { get; set; }
    }
}
