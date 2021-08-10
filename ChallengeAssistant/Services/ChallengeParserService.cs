using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ChallengeAssistant.Interfaces;
using ChallengeAssistant.Models;
using DevryDomain.Models;
using DevryInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnofficialDevryIT.Architecture.Models;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ChallengeAssistant.Services
{
    public class ChallengeParserService : IChallengeParser
    {
        private readonly ILogger<ChallengeParserService> _logger;
        private readonly IApplicationDbContext _context;

        public ChallengeParserService(ILogger<ChallengeParserService> logger, IApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ResultObject> ParseFileAsync(MemoryStream stream, string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case "yaml":
                case "yml":
                    return await ParseYml(stream);
                case "json":
                    return await ParseJson(stream);
                default:
                    throw new NotSupportedException($"File with {fileExtension} is not yet supported");
            }
        }

        async Task ConsumeEntry(ChallengeConfig config)
        {
            var categoryEntry =
                await _context.GamificationCategories.FirstOrDefaultAsync(x => x.Name == config.Category);

            // Ensure the category exists so we can properly map it
            if (categoryEntry == null)
            {
                categoryEntry = new GamificationCategory()
                {
                    Name = config.Category
                };
                _context.GamificationCategories.Add(categoryEntry);
                await _context.SaveChangesAsync();
            }

            // Convert from config object into actual object
            Challenge challenge = new()
            {
                Explanation = config.Explanation,
                Question = config.Question,
                GamificationCategoryId = categoryEntry.Id
            };

            _context.Challenges.Add(challenge);
            await _context.SaveChangesAsync();

            foreach (ResponseConfig response in config.Responses)
            {
                ChallengeResponse entry = new()
                {
                    ChallengeId = challenge.Id,
                    Reward = response.Reward,
                    IsCorrect = response.Correct,
                    Value = response.Text
                };

                _context.ChallengeResponses.Add(entry);
            }

            await _context.SaveChangesAsync();
        }

        async Task<ResultObject> ParseYml(Stream stream)
        {
            using TextReader reader = new StreamReader(stream);
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            string input = await reader.ReadToEndAsync();
            var parser = new Parser(new StringReader(input));
            List<Challenge> results = new();
            // consume the stream start event manually
            parser.Consume<StreamStart>();

            while (parser.TryConsume<DocumentStart>(out _))
            {
                // deserialize the document
                try
                {
                    var config = deserializer.Deserialize<ChallengeConfig>(parser);
                    await ConsumeEntry(config);
                }
                catch (Exception ex)
                {
                    string error = "Error while processing Yml challenge configuration file.";
                    _logger.LogError(error, ex);
                    return ResultObject.Failure(error, ex.Message);
                }
                finally
                {
                    reader.Dispose();
                }
            }

            return ResultObject.Success();
        }

        async Task<ResultObject> ParseJson(Stream stream)
        {
            using StreamReader reader = new StreamReader(stream);
            string text = await reader.ReadToEndAsync();

            try
            {
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<ChallengeConfig>(text);
                await ConsumeEntry(config);
            }
            catch (Exception ex)
            {
                string error = "Error while processing JSON challenge configuration file.";
                _logger.LogError(error, ex);
                return ResultObject.Failure(error, ex.Message);
            }
            finally
            {
                reader.Dispose();
            }
            
            return ResultObject.Success();
        }
    }
}