using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DevryServices.Common.Modules
{
    public interface IModuleInitializer
    {
        void ConfigureServices(IServiceCollection serviceCollection);
        void Configure(IApplicationBuilder app, IWebHostEnvironment env);
    }
}
