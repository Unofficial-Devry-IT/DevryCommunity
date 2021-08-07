using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace ChallengeAssistant.Models
{
    public class QuizApiQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Answers { get; set; } = new();

        public Dictionary<string, string> Correct_Answers { get; set; } = new();
        public string Multiple_Correct_Answers { get; set; }

        public string Explanation { get; set; }
        public string Tip { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine("ID: " + Id);
            builder.AppendLine("Question: " + Question);
            builder.AppendLine("Answers: \n\t" + string.Join("\n\t", Answers.Select(x => x.Key + " " + x.Value)));
            builder.AppendLine("Correct Answers: \n\t" +
                               string.Join("\n\t", Correct_Answers.Select(x => x.Key + " " + x.Value)));
            builder.AppendLine("Multiple Correct Answers: " + Multiple_Correct_Answers);
            
            if(!string.IsNullOrEmpty(Explanation))
                builder.AppendLine("Explanation: " + Explanation);
            
            if(!string.IsNullOrEmpty(Tip))
                builder.AppendLine("Tip: " + Tip);
            
            builder.AppendLine("Category: " + Category);
            builder.AppendLine("Difficulty: " + Difficulty);

            return builder.ToString();
        }
    }
}