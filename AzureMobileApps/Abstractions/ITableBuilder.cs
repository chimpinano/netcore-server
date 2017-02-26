using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;

namespace Microsoft.Azure.Mobile.Core.Server.Abstractions
{
    /// <summary>
    /// Defines a contract for the Azure Mobile Apps Table Builder in an
    /// application.  A table builder specifies the tables for an Azure
    /// Mobile Apps backend.
    /// </summary>
    public interface ITableBuilder
    {
        /// <summary>
        /// GTets the <see cref="IApplicationBuilder"/>
        /// </summary>
        IApplicationBuilder ApplicationBuilder { get; }

        /// <summary>
        /// Gets the list of tables configured in the builder
        /// </summary>
        IList<ITable> Tables { get; }
    }
}
