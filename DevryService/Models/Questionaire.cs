using DevryService.Core;
using DevryService.Core.Questions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevryService.Models
{
    public class Questionaire : EntityWithTypedId<string>
    {
        public Questionaire()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Title { get; set; }
        public string Description { get; set; }

        public string AnswerDescription { get; set; }
        public QuestionaireType Type { get; set; }

        public DateTime? LastUsed { get; set; }

        /// <summary>
        /// Duration in minutes
        /// </summary>
        public double Duration { get; set; } = 5;

        /// <summary>
        /// JSON - Will either be a list of:
        /// <see cref="MessageAnswer"/> if <see cref="QuestionaireType"/> is <see cref="QuestionaireType.Message"/>
        /// <see cref="ReactionAnswer" /> if <see cref="QuestionaireType "/> is <see cref="QuestionaireType.Reaction"/>
        /// </summary>
        public string JSON { get; set; }
    }
}
