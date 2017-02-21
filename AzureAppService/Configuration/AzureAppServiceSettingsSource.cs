using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Azure.AppService.Core.Configuration
{
    public class AzureAppServiceSettingsSource : IConfigurationSource
    {
        private readonly bool includeEnvironment;

        public AzureAppServiceSettingsSource(bool includeEnvironment = false)
        {
            this.includeEnvironment = includeEnvironment;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureAppServiceSettingsProvider(Environment.GetEnvironmentVariables(), includeEnvironment);
        }
    }
}