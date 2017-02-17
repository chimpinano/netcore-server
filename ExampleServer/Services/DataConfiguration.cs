using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ExampleServer.Services
{
    public class DataConfiguration : IDataConfiguration
    {
        List<DataConnectionSettings> providerSettings;

        public DataConfiguration(IConfiguration configuration)
        {
            providerSettings = new List<DataConnectionSettings>();
            foreach (var section in configuration.GetChildren())
            {
                providerSettings.Add(new DataConnectionSettings
                {
                    Name = section.Key,
                    Type = section["Type"],
                    ConnectionString = section["ConnectionString"]
                });
            }
        }

        public IEnumerable<DataConnectionSettings> GetDataConnections()
        {
            return providerSettings;
        }
    }
}
