using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Mobile.Core.Server.Abstractions;
using Microsoft.Azure.Mobile.Core.Server.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Core.Server.Extensions
{
    /// <summary>
    /// The main Azure Mobile Apps middleware
    /// </summary>
    public class AzureMobileAppsMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Creates a new piece of middleware for handling Azure Mobile
        /// Apps.
        /// </summary>
        /// <param name="next">The next piece of middleware</param>
        /// <param name="tableBuilderOptions">The table builder options</param>
        /// <param name="loggerFactory">The logger factory</param>
        public AzureMobileAppsMiddleware(
            RequestDelegate next,
            IOptions<MobileTableCollection> tableCollection,
            ILoggerFactory loggerFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            TableCollection = tableCollection.Value ?? throw new ArgumentNullException(nameof(tableCollection));
            Logger = loggerFactory.CreateLogger(this.GetType().FullName) ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// The method that actually gets invoked when the middleware is
        /// invoked.  Handles anything under /tables that it can handle.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the reques</param>
        /// <returns>(async)</returns>
        public async Task Invoke(HttpContext context)
        {
            // Invoke the next piece of middleware
            await _next(context);
        }

        /// <summary>
        /// The table builder options
        /// </summary>
        public MobileTableCollection TableCollection { get; }

        /// <summary>
        /// The logger
        /// </summary>
        public ILogger Logger { get; }
    }
}