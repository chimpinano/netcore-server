using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace ExampleServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights();

            var regionName = Environment.GetEnvironmentVariable("REGION_NAME");
            if (regionName != null)
            {
                hostBuilder.UseAzureAppServices();
            }
                
            var host = hostBuilder.Build();

            host.Run();
        }
    }
}
