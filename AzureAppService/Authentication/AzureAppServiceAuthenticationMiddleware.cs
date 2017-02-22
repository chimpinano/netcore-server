using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Encodings.Web;

namespace Microsoft.Azure.AppService.Core.Authentication
{
    public class AzureAppServiceAuthenticationMiddleware : AuthenticationMiddleware<AzureAppServiceAuthenticationOptions>
    {
        public AzureAppServiceAuthenticationMiddleware(
            RequestDelegate next, 
            IOptions<AzureAppServiceAuthenticationOptions> options, 
            ILoggerFactory loggerFactory, 
            UrlEncoder encoder) 
            : base(next, options, loggerFactory, encoder)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
        }

        /// <summary>
        /// Called by the <see cref="AuthenticationMiddleware{TOptions}"/> base class to create a
        /// per-request handler.
        /// </summary>
        /// <returns>A new instance of the request handler</returns>
        protected override AuthenticationHandler<AzureAppServiceAuthenticationOptions> CreateHandler()
        {
            return new AzureAppServiceAuthenticationHandler();
        }
    }
}