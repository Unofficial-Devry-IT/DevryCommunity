using System;
using System.Diagnostics;
using System.IO;
using DevryInfrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
                        foreach (string file in Directory.GetFiles(StorageHandler.ConfigsPath))
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