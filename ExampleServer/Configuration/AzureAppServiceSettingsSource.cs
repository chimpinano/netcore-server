using Microsoft.Extensions.Configuration;
using System;

namespace ExampleServer.Configuration
{
    internal class AzureAppServiceSettingsSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureAppServiceSettingsProvider(Environment.GetEnvironmentVariables());
        }
    }
}