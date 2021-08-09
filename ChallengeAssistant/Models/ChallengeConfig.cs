using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ChallengeAssistant.Models
{
    [Serializable]
    [XmlRoot("Challenge")]
    public class ChallengeConfig
    {
        public string Question { get; set; }
        public string Explanation { get; set; }
        public string Category { get; set; }

        [XmlArray("Responses")]
        [XmlArrayItem("Response", typeof(ResponseConfig))]
        public List<ResponseConfig> Responses { get; set; } = new();

        public override string ToString()
        {
            return $"Question: {Question}\n" +
                   $"Explanation: {Explanation}\n" +
                   $"Category: {Category}\n" +
                   $"Responses: \n\t{string.Join("\n\t", Responses.Select(x => x.ToString()))}\n";
        }
    }
    
    [Serializable]
    [XmlRoot("Response")]
    public class ResponseConfig
    {
        public string Text { get; set; }
        public double Reward { get; set; }
        public bool Correct { get; set; }

        public override string ToString()
        {
            return $"\t\tText: {Text}\n" +
                   $"\t\tReward: {Reward}\n" +
                   $"\t\tCorrect: {Correct}" +
                   $"\t\t------------------\n";
        }
    }
}