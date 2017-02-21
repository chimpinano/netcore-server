using Microsoft.Azure.AppService.Core.Configuration;

namespace Microsoft.Extensions.Configuration
{
    public static class AzureAppServiceConfigurationBuilderExtensions
    {
        /// <summary>
        /// Add the Azure App Service Environment variables to the configuration.  This will produce a configuration
        /// akin to the following JSON specification
        /// 
        ///{
        ///    "ConnectionStrings": {
        ///        "MS_TableConnectionString": "my-connection-string"
        ///    },
        ///    "Data": {
        ///        "MS_TableConnectionString": {
        ///            "Type": "SQLAZURE",
        ///            "ConnectionString": "my-connection-string"
        ///        }
        ///    },
        ///    "AzureAppService": {
        ///        "AppSettings": {
        ///            "MobileAppsManagement_EXTENSION_VERSION": "latest"
        ///        }
        ///        "Auth": {
        ///            "Enabled": "True",
        ///            "SigningKey": "some-long-string",
        ///            "AzureActiveDirectory": {
        ///                "ClientId: "my-client-id",
        ///                "ClientSecret": "my-client-secret",
        ///                "Mode": "Express"
        ///            }
        ///        },
        ///        "Push": {
        ///            // ...
        ///        }
        ///    }
        ///}
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> with the current configuration</param>
        /// <param name="includeEnvironment">True if you want the Environment section to be included</param>
        /// <returns>The new <see cref="IConfigurationBuilder"/> for chaining</returns>
        public static IConfigurationBuilder AddAzureAppServiceSettings(this IConfigurationBuilder builder, bool includeEnvironment = false)
        {
            return builder.Add(new AzureAppServiceSettingsSource(includeEnvironment));
        }
    }
}
