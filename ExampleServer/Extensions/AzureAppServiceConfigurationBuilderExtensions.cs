using Microsoft.Extensions.Configuration;

namespace ExampleServer.Extensions
{
    public static class AzureAppServiceConfigurationBuilderExtensions
    {
        /// <summary>
        /// Add the Azure App Service Data Connections configuration to the configuration.  Given a
        /// SQL Azure connection called "MS_TableConnectionString", this will produce a configuration
        /// analogous to the following JSON sample:
        /// 
        /// {
        ///     "ConnectionStrings": {
        ///         "MS_TableConnectionString": "my-connection-string"
        ///     },
        ///     "Data": {
        ///         "MS_TableConnectionString": {
        ///             "Type": "SQLAZURE",
        ///             "ConnectionString": "my-connection-string"
        ///         }
        ///     }
        /// }
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> with the current configuration</param>
        /// <returns>The new <see cref="IConfigurationBuilder"/> for chaining</returns>
        public static IConfigurationBuilder AddAzureAppServiceDataConnections(this IConfigurationBuilder builder)
        {
            return builder.Add(new AzureAppServiceDataConnectionsSource());
        }
    }
}
