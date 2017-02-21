using System.Collections;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Microsoft.Azure.AppService.Core.Configuration
{
    public class AzureAppServiceSettingsProvider : ConfigurationProvider
    {
        private IDictionary env;

        /// <summary>
        /// Where all the app settings should go in the configuration
        /// </summary>
        private const string SettingsPrefix = "AzureAppService";

        /// <summary>
        /// The regular expression used to match the key in the environment for Data Connections.
        /// </summary>
        private Regex DataConnectionsRegexp = new Regex(@"^([A-Z]+)CONNSTR_(.+)$");

        /// <summary>
        /// Mapping from environment variable to position in configuration - explicit cases
        /// </summary>
        private Dictionary<string, string> specialCases = new Dictionary<string, string>
        {
            { "WEBSITE_AUTH_CLIENT_ID",                 $"{SettingsPrefix}:Auth:AzureActiveDirectory:ClientId" },
            { "WEBSITE_AUTH_CLIENT_SECRET",             $"{SettingsPrefix}:Auth:AzureActiveDirectory:ClientSecret" },
            { "WEBSITE_AUTH_OPENID_ISSUER",             $"{SettingsPrefix}:Auth:AzureActiveDirectory:Issuer" },
            { "WEBSITE_AUTH_FB_APP_ID",                 $"{SettingsPrefix}:Auth:Facebook:ClientId" },
            { "WEBSITE_AUTH_FB_APP_SECRET",             $"{SettingsPrefix}:Auth:Facebook:ClientSecret" },
            { "WEBSITE_AUTH_GOOGLE_CLIENT_ID",          $"{SettingsPrefix}:Auth:Google:ClientId" },
            { "WEBSITE_AUTH_GOOGLE_CLIENT_SECRET",      $"{SettingsPrefix}:Auth:Google:ClientSecret" },
            { "WEBSITE_AUTH_MSA_CLIENT_ID",             $"{SettingsPrefix}:Auth:MicrosoftAccount:ClientId" },
            { "WEBSITE_AUTH_MSA_CLIENT_SECRET",         $"{SettingsPrefix}:Auth:MicrosoftAccount:ClientSecret" },
            { "WEBSITE_AUTH_TWITTER_CONSUMER_KEY",      $"{SettingsPrefix}:Auth:Twitter:ClientId" },
            { "WEBSITE_AUTH_TWITTER_CONSUMER_SECRET",   $"{SettingsPrefix}:Auth:Twitter:ClientSecret" },
            { "WEBSITE_AUTH_SIGNING_KEY",               $"{SettingsPrefix}:Auth:SigningKey" },
            { "MS_NotificationHubId",                   $"{SettingsPrefix}:Push:NotificationHubId" }
        };

        /// <summary>
        /// Mpping from environment variable to position in configuration - scoped cases
        /// </summary>
        private Dictionary<string, string> scopedCases = new Dictionary<string, string>
        {
            { "WEBSITE_AUTH_", $"{SettingsPrefix}:Auth" },
            { "WEBSITE_PUSH_", $"{SettingsPrefix}:Push" }
        };

        /// <summary>
        /// Authentication providers need to be done before the scoped cases, so their mapping
        /// is separate from the scoped cases
        /// </summary>
        private Dictionary<string, string> authProviderMapping = new Dictionary<string, string>
        {
            { "WEBSITE_AUTH_FB_",          $"{SettingsPrefix}:Auth:Facebook" },
            { "WEBSITE_AUTH_GOOGLE_",      $"{SettingsPrefix}:Auth:Google" },
            { "WEBSITE_AUTH_MSA_",         $"{SettingsPrefix}:Auth:MicrosoftAccount" },
            { "WEBSITE_AUTH_TWITTER_",     $"{SettingsPrefix}:Auth:Twitter" }
        };

        /// <summary>
        /// True if we want to include the Environment: configuration section
        /// </summary>
        private readonly bool includeEnvironment;

        /// <summary>
        /// Configure a new configuration provider
        /// </summary>
        /// <param name="env">The Azure App Service environment dictionary</param>
        /// <param name="includeEnvironment">Set to true to enable the Environment: configuration section</param>
        public AzureAppServiceSettingsProvider(IDictionary env, bool includeEnvironment = false)
        {
            this.env = env;
            this.includeEnvironment = includeEnvironment;
        }

        /// <summary>
        /// Loads the appropriate settings into the configuration.  The Data object is provided for us
        /// by the ConfigurationProvider
        /// </summary>
        /// <seealso cref="Microsoft.Extensions.Configuration.ConfigurationProvider"/>
        public override void Load()
        {
            foreach (DictionaryEntry e in env)
            {
                string key = e.Key as string;
                string value = e.Value as string;

                var m = DataConnectionsRegexp.Match(key);
                if (m.Success)
                {
                    var type = m.Groups[1].Value;
                    var name = m.Groups[2].Value;

                    if (!key.Equals("CUSTOMCONNSTR_MS_NotificationHubConnectionString"))
                    {
                        Data[$"Data:{name}:Type"] = type;
                        Data[$"Data:{name}:ConnectionString"] = value;
                    }
                    else
                    {
                        Data[$"{SettingsPrefix}:Push:ConnectionString"] = value;
                    }
                    Data[$"ConnectionStrings:{name}"] = value;
                    continue;
                }

                // If it is a special case, then handle it through the mapping and move on
                if (specialCases.ContainsKey(key))
                {
                    Data[specialCases[key]] = value;
                    continue;
                }

                // A special case for AUTO_AAD
                if (key.Equals("WEBSITE_AUTH_AUTO_AAD"))
                {
                    Data[$"{SettingsPrefix}:Auth:AzureActiveDirectory:Mode"] = value.Equals("True") ? "Express" : "Advanced";
                    continue;
                }

                // Scoped Cases for authentication providers
                if (dictionaryMappingFound(key, value, authProviderMapping))
                {
                    continue;
                }

                // Other scoped cases (not auth providers)
                if (dictionaryMappingFound(key, value, scopedCases))
                {
                    continue;
                }

                // Other internal settings
                if (key.StartsWith("WEBSITE_") && !containsMappedKey(key, scopedCases))
                {
                    var setting = key.Substring(8);
                    Data[$"{SettingsPrefix}:Website:{setting}"] = value;
                    continue;
                }

                // App Settings - anything not in the WEBSITE section
                if (key.StartsWith("APPSETTING_") && !key.StartsWith("APPSETTING_WEBSITE_"))
                {
                    var setting = key.Substring(11);
                    Data[$"{SettingsPrefix}:AppSetting:{setting}"] = value;
                    continue;
                }

                // Add everything else into { "Environment" } if required
                if (includeEnvironment)
                {
                    if (!key.StartsWith("APPSETTING_"))
                    {
                        Data[$"Environment:{key}"] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the key starts with any of the keys in the mapping
        /// </summary>
        /// <param name="key">The environment variable</param>
        /// <param name="mapping">The mapping dictionary</param>
        /// <returns></returns>
        private bool containsMappedKey(string key, Dictionary<string, string> mapping)
        {
            foreach (var start in mapping.Keys)
            {
                if (key.StartsWith(start))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Handler for a mapping dictionary
        /// </summary>
        /// <param name="key">The environment variable to check</param>
        /// <param name="value">The value of the environment variable</param>
        /// <param name="mapping">The mapping dictionary</param>
        /// <returns>true if a match was found</returns>
        private bool dictionaryMappingFound(string key, string value, Dictionary<string, string> mapping)
        {
            foreach (string start in mapping.Keys)
            {
                if (key.StartsWith(start))
                {
                    var setting = key.Substring(start.Length);
                    Data[$"{mapping[start]}:{setting}"] = value;
                    return true;
                }
            }
            return false;
        }
    }
}