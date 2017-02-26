using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Mobile.Core.Server.Abstractions;
using Microsoft.Azure.Mobile.Core.Server.Tables;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Azure.Mobile.Core.Server.Extensions
{
    public static class MobileAppsAppBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="AzureMobileAppsMiddleware"/> middleware to the specified
        /// <see cref="IApplicationBuilder"/>, which service of Azure Mobile Apps endpoints.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="configureTables">The <see cref="ITableBuilder"/> used to create the table list.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseAzureMobileApps(this IApplicationBuilder app, Action<ITableBuilder> configureTables)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (configureTables == null)
            {
                throw new ArgumentNullException(nameof(configureTables));
            }

            var tableBuilder = new MobileTableBuilder();
            configureTables(tableBuilder);
            if (tableBuilder.Tables.Count == 0)
            {
                throw new ArgumentException("At least one table is expected", nameof(configureTables));
            }

            var tables = new MobileTableCollection(tableBuilder);
            return app.UseMiddleware<AzureMobileAppsMiddleware>(Options.Create(tables));
        }
    }
}
