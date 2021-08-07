using ChallengeAssistant.Interfaces;
using ChallengeAssistant.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChallengeAssistant
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChallengeApis(this IServiceCollection services)
        {
            services.AddSingleton<IChallengeApi, QuizApiService>();

            return services;
        }
    }
}