using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.AppService.Core.Authentication;
using Microsoft.Azure.Mobile.Core.Server.Extensions;
using Microsoft.Azure.Mobile.Core.Server.Managers;
using Microsoft.Azure.Mobile.Core.Server.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExampleServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddAzureAppServiceSettings();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Configuration as a service
            services.AddSingleton<IConfiguration>(Configuration);

            // Add Authentication Service
            services.AddAuthentication();

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAzureAppServiceAuthentication(new AzureAppServiceAuthenticationOptions
            {
                SigningKey = Configuration["AzureAppService:Auth:SigningKey"],
                AllowedAudiences = new[] { $"https://{Configuration["AzureAppService:Website:HOST_NAME"]}/" },
                AllowedIssuers = new[] { $"https://{Configuration["AzureAppService:Website:HOST_NAME"]}/" }
            });

            app.UseStaticFiles();

            app.UseAzureMobileApps(tables =>
            {
                tables.AddTable("todoitem", new InMemoryDomainManager());
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
