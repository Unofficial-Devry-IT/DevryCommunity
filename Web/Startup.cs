using Application;
using Application.Extensions;
using BotApp;
using DiscordOAuth;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            
            services.AddApplication();
            services.AddDiscordBot();
            services.AddInfrastructure<Startup>(Configuration);
            
            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<IdentityUser, ApplicationDbContext>();

            services.AddAuthentication()
                .AddDiscord(x =>
                {
                    /*
                        In Production:
                            Shall leverage secret a secret layer within the docker container
                            to safely store these values
                            
                        In Development:
                            User Secrets are leveraged
                                Avoids storing these sensitive items in appsettings which gets
                                committed to github
                        
                        View OAuth.md within the Web directory for more details                                                          
                     */
                    
                    #if DEBUG
                        x.AppId = Configuration["Discord:AppId"].FromBase64();
                        x.AppSecret = Configuration["Discord:AppSecret"].FromBase64();
                    #else
                        string appId = System.Environment.GetEnvironmentVariable("DISCORD_APPID");
                        string appSecret = System.Environment.GetEnvironmentVariable("DISCORD_APPSECRET");
                        x.AppId = appId.FromBase64();
                        x.AppSecret = appSecret.FromBase64();
                    #endif

                    x.Scope.Add("guilds");
                    x.Scope.Add("email");
                })
                .AddIdentityServerJwt();
        
            services.AddControllersWithViews();
            services.AddRazorPages();
            
            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}