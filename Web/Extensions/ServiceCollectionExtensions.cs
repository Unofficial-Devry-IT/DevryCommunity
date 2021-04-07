using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
    
namespace Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCertificate(this IServiceCollection services)
        {
            
            return services;
        }
    }
}