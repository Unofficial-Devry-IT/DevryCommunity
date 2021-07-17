using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevryInfrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevryBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.ConfigureAppConfiguration((hostContext, context) =>
                    {
                        string configDirectory = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Data", "Configs");

                        foreach (string file in Directory.GetFiles(configDirectory))
                        {
                            FileInfo info = new FileInfo(file);
                            if (info.Extension.Contains("json"))
                            {
                                context.AddJsonFile(file);
                            }
                        }
                    });
                    builder.UseStartup<Startup>();
                });
    }
}