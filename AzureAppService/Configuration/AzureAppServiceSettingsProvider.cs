using System.Collections;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Microsoft.Azure.AppService.Core.Configuration
{
    internal class AzureAppServiceSettingsProvider : ConfigurationProvider
    {
        private IDictionary env;

        /// <summary>
        /// The regular expression used to match the key in the environment for Data Connections.
        /// </summary>
        private Regex DataConnectionsRegexp = new Regex(@"^([A-Z]+)CONNSTR_(.+)$");

        /// <summary>
        /// Where all the app settings should go in the configuration
        /// </summary>
        private const string SettingsPrefix = "AzureAppService";

        /// <summary>
        /// Mapping from environment variable to position in configuration - explicit cases
        /// </summary>
        private Dictionary<string, string> specialCases = new Dictionary<string, string>
        {
            { "WEBSITE_AUTH_CLIENT_ID",                 $"{SettingsPrefix}:Auth:AzureActiveDirectory:ClientId" },
            { "WEBSITE_AUTH_CLIENT_SECRET",             $"{SettingsPrefix}:Auth:AzureActiveDirectory:ClientSecret" },
            { "WEBSITE_AUTH_OPENID_ISSUER",             $"{SettingsPrefix}:Auth:AzureActiveDirectory:Issuer" },
            { "WEBSITE_AUTH_FACEBOOK_CLIENT_ID",        $"{SettingsPrefix}:Auth:Facebook:ClientId" },
            { "WEBSITE_AUTH_FACEBOOK_CLIENT_SECRET",    $"{SettingsPrefix}:Auth:Facebook:ClientSecret" },
            { "WEBSITE_AUTH_GOOGLE_CLIENT_ID",          $"{SettingsPrefix}:Auth:Google:ClientId" },
            { "WEBSITE_AUTH_GOOGLE_CLIENT_SECRET",      $"{SettingsPrefix}:Auth:Google:ClientSecret" },
            { "WEBSITE_AUTH_MSA_CLIENT_ID",             $"{SettingsPrefix}:Auth:MicrosoftAccount:ClientId" },
            { "WEBSITE_AUTH_MSA_CLIENT_SECRET",         $"{SettingsPrefix}:Auth:MicrosoftAccount:ClientSecret" },
            { "WEBSITE_AUTH_TWITTER_CONSUMER_ID",       $"{SettingsPrefix}:Auth:Twitter:ClientId" },
            { "WEBSITE_AUTH_TWITTER_CONSUMER_SECRET",   $"{SettingsPrefix}:Auth:Twitter:ClientSecret" },
            { "WEBSITE_AUTH_SIGNING_KEY",               $"{SettingsPrefix}:Auth:SigningKey" }
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
            { "WEBSITE_AUTH_FACEBOOK_",                 $"{SettingsPrefix}:Auth:Facebook" },
            { "WEBSITE_AUTH_GOOGLE_",                   $"{SettingsPrefix}:Auth:Google" },
            { "WEBSITE_AUTH_MSA_",                      $"{SettingsPrefix}:Auth:MicrosoftAccount" },
            { "WEBSITE_AUTH_TWITTER_",                  $"{SettingsPrefix}:Auth:Twitter" }
        };

        public AzureAppServiceSettingsProvider(IDictionary env)
        {
            this.env = env;
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
                    Data[$"Data:{m.Groups[2].Value}:Type"] = m.Groups[1].Value;
                    Data[$"Data:{m.Groups[2].Value}:ConnectionString"] = value;
                    Data[$"ConnectionStrings:{m.Groups[2].Value}"] = value;
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
                foreach (string start in authProviderMapping.Keys)
                {
                    if (key.StartsWith(start))
                    {
                        var setting = key.Substring(start.Length);
                        Data[$"{authProviderMapping[start]}:{setting}"] = value;
                        continue;
                    }
                }

                // Other scoped cases (not auth providers)
                foreach (string start in scopedCases.Keys)
                {
                    if (key.StartsWith(start))
                    {
                        var setting = key.Substring(start.Length);
                        Data[$"{scopedCases[start]}:{setting}"] = value;
                        continue;
                    }
                }

                // Other internal settings
                if (key.StartsWith("WEBSITE_"))
                {
                    var setting = key.Substring(19);
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

                // Add everything else into { "Environment" }
                Data[$"Environment:{key}"] = value;
            }
        }
    }
}