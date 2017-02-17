using System.Collections.Generic;

namespace ExampleServer.Services
{
    public interface IDataConfiguration
    {
        IEnumerable<DataConnectionSettings> GetDataConnections();
    }
}
