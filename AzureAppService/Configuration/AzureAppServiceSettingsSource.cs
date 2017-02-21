﻿using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Azure.AppService.Core.Configuration
{
    public class AzureAppServiceSettingsSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureAppServiceSettingsProvider(Environment.GetEnvironmentVariables());
        }
    }
}