using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Text.RegularExpressions;

namespace ExampleServer.Extensions
{
    internal class AzureAppServiceDataConnectionsProvider : ConfigurationProvider
    {
        /// <summary>
        /// The environment (key-value pairs of strings) that we are using to generate the configuration
        /// </summary>
        private IDictionary environment;

        /// <summary>
        /// The regular expression used to match the key in the environment for Data Connections.
        /// </summary>
        private Regex MagicRegex = new Regex(@"^([A-Z]+)CONNSTR_(.+)$");

        public AzureAppServiceDataConnectionsProvider(IDictionary environment)
        {
            this.environment = environment;
        }

        /// <summary>
        /// Loads the appropriate settings into the configuration.  The Data object is provided for us
        /// by the ConfigurationProvider
        /// </summary>
        /// <seealso cref="Microsoft.Extensions.Configuration.ConfigurationProvider"/>
        public override void Load()
        {
            foreach (string key in environment.Keys)
            {
                Match m = MagicRegex.Match(key);
                if (m.Success)
                {
                    var conntype = m.Groups[1].Value;
                    var connname = m.Groups[2].Value;
                    var connstr = environment[key] as string;

                    Data[$"Data:{connname}:Type"] = conntype;
                    Data[$"Data:{connname}:ConnectionString"] = connstr;
                    Data[$"ConnectionStrings:{connname}"] = connstr;
                }
            }
        }
    }
}