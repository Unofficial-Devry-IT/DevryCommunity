using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChallengeAssistant.Models;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ChallengeAssistant
{
    public class Program
    {
        static void DeserializeTest(string value)
        {
            var input = new StringReader(File.ReadAllText(value));
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
            if (args == null || args.Length < 1)
                return;
            
            if(File.Exists(args.First()))
                DeserializeTest(args.First());
        }
    }
}