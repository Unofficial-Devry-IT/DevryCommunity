using System;
using System.Collections.Generic;
using System.Text;

namespace DevryService.Core.Questions
{
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
}
