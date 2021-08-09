using System;
using System.Collections.Generic;
using System.IO;
using ChallengeAssistant.Models;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ChallengeAssistant
{
    public class Program
    {
        static void DeserializeTest()
        {
            var input = new StringReader(File.ReadAllText(@"C:\users\jonbr\desktop\daily-challenge.yml"));
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            var parser = new Parser(input);
            
            // consume the stream start event manually
            parser.Consume<StreamStart>();

            while (parser.TryConsume<DocumentStart>(out _))
            {
                // deserialize the document
                var challenge = deserializer.Deserialize<ChallengeConfig>(parser);

                Console.WriteLine("## Document");
                Console.WriteLine(challenge);
            } 
        }
        
        public static void Main(string[] args)
        {
            DeserializeTest();
        }
    }
}