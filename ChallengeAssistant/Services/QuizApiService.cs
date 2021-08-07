using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ChallengeAssistant.Interfaces;
using ChallengeAssistant.Models;
using DevryDomain.Models;
using DevryInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UnofficialDevryIT.Architecture.Extensions;

namespace ChallengeAssistant.Services
{
    public class QuizApiService : IChallengeApi
    {
        public string Name => "Quiz API";
        public string Site => "https://quizapi.io";

        private readonly ILogger<QuizApiService> _logger;
        private readonly string _token;
        private readonly string _apiUrlFormat;
        private readonly IApplicationDbContext _context;
        
        public QuizApiService(ILogger<QuizApiService> logger, IConfiguration configuration, IApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            _token = configuration.GetValue<string>("ChallengeApis:QuizApiToken").FromBase64();
            _apiUrlFormat = configuration.GetValue<string>("ChallengeApis:QuizApiUrl");
        }

        Challenge ParseQuestion(QuizApiQuestion obj)
        {
            _logger.LogInformation(obj.ToString());
            
            Challenge challenge = new Challenge()
            {
                Question = obj.Question,
                Title = obj.Category,
                Explanation = obj.Explanation
            };

            double reward = 0;
            
            switch (obj.Category.ToLower())
            {
                default:
                    reward = 2;
                    break;
                case "medium":
                    reward = 5;
                    break;
                case "hard":
                    reward = 10;
                    break;
            }
            
            foreach (var answer in obj.Answers)
            {
                if (answer.Value == null)
                    continue;

                var correct = obj.Correct_Answers[answer.Key + "_correct"]
                    .Equals("true", StringComparison.InvariantCultureIgnoreCase);

                ChallengeResponse challengeResponse = new()
                {
                    IsCorrect = correct,
                    Value = answer.Value,
                    Reward = correct ? reward : 0
                };

                challenge.Responses.Add(challengeResponse);
            }

            return challenge;
        }
        
        public async Task<List<Challenge>> RetrieveQuestionsAsync()
        {
            List<Challenge> results = new List<Challenge>();

            string url = string.Format(_apiUrlFormat, _token);

            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error Code: {response.StatusCode} while retrieving data from {url}");
                return results;
            }

            var questions = Newtonsoft.Json.JsonConvert
                .DeserializeObject<QuizApiQuestion[]>(await response.Content.ReadAsStringAsync());
            
            if (questions != null)
                foreach (var question in questions)
                {
                    var category =
                        await _context.GamificationCategories
                            .FirstOrDefaultAsync(x => x.Name == question.Category);

                    if (category == null)
                    {
                        category = new GamificationCategory()
                        {
                            Name = question.Category
                        };
                        
                        _context.GamificationCategories.Add(category);
                        await _context.SaveChangesAsync();
                    }

                    var instance = ParseQuestion(question);

                    instance.GamificationCategoryId = category.Id;
                    
                    _context.Challenges.Add(instance);
                    _context.ChallengeResponses.AddRange(instance.Responses);

                    await _context.SaveChangesAsync();
                    
                    results.Add(instance);
                }

            return results;
        }
    }
}