using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevryService.Core.Questions
{
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
}
