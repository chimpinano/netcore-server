using Microsoft.Extensions.Configuration;
using System;

namespace ExampleServer.Extensions
{
    public class AzureAppServiceDataConnectionsSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureAppServiceDataConnectionsProvider(Environment.GetEnvironmentVariables());
        }
    }
}
